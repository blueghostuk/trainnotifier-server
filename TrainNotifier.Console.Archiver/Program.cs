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

            try
            {
                IEnumerable<Guid> trains = Enumerable.Empty<Guid>();
                DataArchiveRepository dar = new DataArchiveRepository();
                DateTime period = DateTime.UtcNow.AddDays(-1 * Convert.ToInt32(ConfigurationManager.AppSettings["archiveDays"]));
                string filePath = ConfigurationManager.AppSettings["FileArchivePath"];
                if (!Directory.Exists(filePath))
                {
                    throw new DirectoryNotFoundException(filePath);
                }
                FileIOPermission ioPermission = new FileIOPermission(FileIOPermissionAccess.Write, filePath);
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
                            Trace.TraceInformation("Getting train movements for train id {0}", train);
                            var tms = dar.GetTrainMovements(train);
                            Trace.TraceInformation("Archiving {0} train movements for train id {1}", tms.Count(), train);
                            dar.ArchiveTrainMovement(train, tms, filePath);
                            Trace.TraceInformation("Archived train id {0}", train);
                        }
                    }
                } while (trains.Any());
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                Trace.Flush();
                throw;
            }

            Trace.TraceInformation("Completed Archive");
        }
    }
}
