using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using TrainNotifier.Common;
using TrainNotifier.Common.Model.Schedule;
using TrainNotifier.ScheduleLibrary;
using TrainNotifier.Service;

namespace TrainNotifier.Schedule.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            TraceHelper.SetupTrace();
            var options = new Options();
            CommandLine.CommandLineParser.Default.ParseArguments(args, options);
            bool delete = true;
            var tiprep = new TiplocRepository();
            var tiplocs = tiprep.GetTiplocs().ToList();
            var schedrep = new ScheduleRepository();

            string tempDir = Path.GetTempPath();
            string gzFile = Path.Combine(tempDir, string.Format("{0:ddMMyyyy}.gz", DateTime.UtcNow));
            string jsonFile = Path.Combine(tempDir, string.Format("{0:ddMMyyyy}.json", DateTime.UtcNow));
            try
            {
                if (options.Force || !File.Exists(gzFile))
                {
                    ScheduleService.DownloadSchedule(gzFile,
                        // defaults to all
                        options.Toc, 
                        // defaults to daily
                        options.ScheduleType,
                        // defaults to previous day (e.g. on monday download sunday - http://nrodwiki.rockshore.net/index.php/Schedule)
                        options.Day.HasValue ? options.Day.Value : DateTime.Today.AddDays(-1).DayOfWeek);
                }
                else
                {
                    Trace.TraceInformation("File {0} already exists", gzFile);
                }
                if (options.Force || !File.Exists(jsonFile))
                {
                    using (FileStream originalFileStream = File.OpenRead(gzFile))
                    {
                        using (FileStream decompressedFileStream = File.Create(jsonFile))
                        {
                            using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                            {
                                Trace.TraceInformation("Decompressing {0} to {1}", gzFile, jsonFile);
                                decompressionStream.CopyTo(decompressedFileStream);
                                Trace.TraceInformation("Decompressed: {0}", jsonFile);
                            }
                        }
                    }
                }
                else
                {
                    Trace.TraceInformation("File {0} already exists", jsonFile);
                }
                if (File.Exists(jsonFile))
                {
                    foreach (string row in File.ReadLines(jsonFile))
                    {
                        var rowData = JsonConvert.DeserializeObject<dynamic>(row);
                        try
                        {
                            if (rowData.JsonScheduleV1 != null)
                            {
                                ScheduleTrain train = ScheduleService.ParseJsonTrain(rowData.JsonScheduleV1, tiplocs);
                                schedrep.InsertSchedule(train);
                                Trace.TraceInformation("Inserted Train UID {0}, Indicator {1}", train.TrainUid, train.STPIndicator);
                            }
                        }
                        catch (Exception e)
                        {
                            Trace.TraceError(e.ToString());
                            delete = false;
                            throw;
                        }
                    }
                }
            }
            finally
            {
                if (delete)
                {
                    File.Delete(gzFile);
                    File.Delete(jsonFile);
                }
            }
        }
    }
}
