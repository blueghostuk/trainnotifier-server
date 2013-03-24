using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using TrainNotifier.Common;
using TrainNotifier.Common.Exceptions;
using TrainNotifier.Common.Model;
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
            var aRep = new AssociationRepository();

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
                                AddSchedule(tiprep, tiplocs, schedrep, rowData);
                            }
                            else if (rowData.JsonAssociationV1 != null)
                            {
                                AddAssociation(rowData, tiplocs, tiprep, aRep);
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
                if (options.Delete && delete)
                {
                    File.Delete(gzFile);
                    File.Delete(jsonFile);
                }
            }
        }

        private static void AddAssociation(dynamic rowData, List<TiplocCode> tiplocs, TiplocRepository tipRep, AssociationRepository aRep)
        {
            try
            {
                Association assoc = AssociationJsonMapper.ParseJsonAssociation(rowData.JsonAssociationV1, tiplocs);
                switch (assoc.TransactionType)
                {
                    case TransactionType.Create:
                        Trace.TraceInformation("Inserted Association From UID {0} -> {1}, Type {2}, Indicator {3}",
                            assoc.MainTrainUid, assoc.AssocTrainUid, assoc.AssociationType, assoc.STPIndicator);
                        break;
                    case TransactionType.Delete:
                        Trace.TraceInformation("Delete Association UID {0} For {1}",
                            assoc.MainTrainUid, assoc.StartDate);
                        break;
                }
                aRep.AddAssociation(assoc);
            }
            catch (TiplocNotFoundException tnfe)
            {
                TiplocCode t = new TiplocCode
                {
                    Tiploc = tnfe.Code
                };
                t.TiplocId = tipRep.InsertTiploc(t.Tiploc);
                tiplocs.Add(t);
                AddAssociation(rowData, tiplocs, tipRep, aRep);
            }
        }

        private static void AddSchedule(TiplocRepository tiprep, List<TiplocCode> tiplocs, ScheduleRepository schedrep, dynamic rowData, int retryCount = 0)
        {
            try
            {
                AddSchedule(tiplocs, schedrep, rowData);
            }
            catch (TiplocNotFoundException tnfe)
            {
                TiplocCode t = new TiplocCode
                {
                    Tiploc = tnfe.Code
                };
                t.TiplocId = tiprep.InsertTiploc(t.Tiploc);
                tiplocs.Add(t);
                AddSchedule(tiplocs, schedrep, rowData);
            }
            catch (DbException dbe)
            {
                Trace.TraceError(dbe.ToString());
                if (retryCount <= 3)
                {
                    Trace.TraceInformation("Retrying");
                    AddSchedule(tiprep, tiplocs, schedrep, rowData, ++retryCount);
                }
                else
                {
                    Trace.TraceError("Retry count exceeded");
                    throw;
                }
            }
        }

        private static void AddSchedule(IEnumerable<TiplocCode> tiplocs, ScheduleRepository schedrep, dynamic rowData)
        {
            ScheduleTrain train = ScheduleTrainMapper.ParseJsonTrain(rowData.JsonScheduleV1, tiplocs);
            switch (train.TransactionType)
            {
                case TransactionType.Create:
                    schedrep.InsertSchedule(train);
                    Trace.TraceInformation("Inserted Train UID {0}, Indicator {1}", train.TrainUid, train.STPIndicator);
                    break;
                case TransactionType.Delete:
                    schedrep.DeleteSchedule(train);
                    Trace.TraceInformation("Deleted Train UID {0}, Indicator {1}, Date {2:dd/MM/yyyy}", train.TrainUid, train.STPIndicator, train.StartDate);
                    break;
            }
        }
    }
}
