using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using TrainNotifier.Common;
using TrainNotifier.Service;

namespace TrainNotifier.Console.Archiver
{
    class Program
    {
        static void Main(string[] args)
        {
            TraceHelper.SetupTrace();

            DataArchiveRepository dar = new DataArchiveRepository();
            try
            {
                IEnumerable<ArchiveTrain> trains = Enumerable.Empty<ArchiveTrain>();
                DateTime period = DateTime.UtcNow.AddDays(-1 * Convert.ToInt32(ConfigurationManager.AppSettings["archiveDays"]));
                string filePath = ConfigurationManager.AppSettings["FileArchivePath"];
                if (!Directory.Exists(filePath))
                {
                    throw new DirectoryNotFoundException(filePath);
                }
                FileIOPermission ioPermission = new FileIOPermission(FileIOPermissionAccess.Write, filePath);
                Trace.TraceInformation("Archiving to path: {0}", filePath);
                ioPermission.Demand();
                uint amount = Convert.ToUInt32(ConfigurationManager.AppSettings["trainsPerRun"]);
                do
                {
                    trains = dar.GetTrainsToArchive(period, amount);
                    if (trains.Any())
                    {
                        Trace.TraceInformation("Got {0} Trains to archive", trains.Count());
                        foreach (var train in trains)
                        {
                            Trace.TraceInformation("Getting train movements for train id {0}", train.Id);
                            var tms = dar.GetTrainMovements(train.Id);
                            Trace.TraceInformation("Archiving {0} train movements for train id {1}", tms.Count(), train.Id);
                            dar.ArchiveTrainMovement(train, tms, filePath);
                            Trace.TraceInformation("Archived train id {0}", train.Id);
                        }
                    }
                } while (trains.Any());

                Trace.TraceInformation("Archiving unused schedules");
                dar.CleanSchedules(period);
                Trace.TraceInformation("Archiving unused associations");
                dar.CleanAssociations(period);
                Trace.TraceInformation("Archiving old ppm");
                dar.CleanPPMRecords(period);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                Trace.Flush();
                throw;
            }
            finally
            {
                Trace.TraceInformation("Cleaning Indexes");
                dar.UpdateIndexes();
                Trace.Flush();
            }

            Trace.TraceInformation("Completed Archive");
            Trace.Flush();
        }
    }
}
