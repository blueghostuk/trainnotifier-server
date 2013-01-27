using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainNotifier.Common.Model.Schedule;
using TrainNotifier.ScheduleLibrary;

namespace TrainNotifier.Schedule.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            string tempDir = Path.GetTempPath();
            string gzFile = Path.Combine(tempDir, string.Format("{0:ddmmyyyy}.gz", DateTime.UtcNow));
            string jsonFile = Path.Combine(tempDir, string.Format("{0:ddmmyyyy}.json", DateTime.UtcNow));
            try
            {
                ScheduleService.DownloadSchedule(gzFile);
                using (FileStream originalFileStream = File.OpenRead(gzFile))
                {
                    using (FileStream decompressedFileStream = File.Create(jsonFile))
                    {
                        using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                        {
                            Console.WriteLine("Decompressing {0} to {1}", gzFile, jsonFile);
                            decompressionStream.CopyTo(decompressedFileStream);
                            Console.WriteLine("Decompressed: {0}", jsonFile);
                        }
                    }
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
                                ScheduleTrain train = ScheduleService.ParseJsonTrain(rowData.JsonScheduleV1);
                            }
                        }
                        catch { }
                    }
                }
            }
            finally
            {
                File.Delete(gzFile);
                File.Delete(jsonFile);
            }
        }
    }
}
