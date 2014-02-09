using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.CorpusExtract;
using TrainNotifier.Common.Model.PPM;
using TrainNotifier.Common.Model.Schedule;
using TrainNotifier.Common.Model.SmartExtract;
using TrainNotifier.Common.Services;
using TrainNotifier.Service;

namespace TrainNotifier.WcfLibrary
{
    public class CacheService : ICacheService
    {
        private static readonly LiveTrainRepository _cacheDb = new LiveTrainRepository();
        private static readonly ScheduleRepository _scheduleRepository = new ScheduleRepository();
        private static readonly PPMDataRepository _ppmRepository = new PPMDataRepository();
        private static readonly IEnumerable<PPMSector> _sectors;

        static CacheService()
        {
            _cacheDb.PreLoadActivations();
            _cacheDb.StartTimer();
            _sectors = _ppmRepository.GetSectors();
        }

        public void CacheTrainData(IEnumerable<ITrainData> trainData)
        {
            Task.Run(() =>
            {
                try
                {
                    _cacheDb.BatchInsertTrainData(trainData);
                }
                catch (Exception e)
                {
                    Trace.TraceError("Could not save Train Data: {0}", e);
                }
            });
        }

        public void CacheVSTPSchedule(ScheduleTrain train)
        {
            Task.Run(() =>
            {
                try
                {
                    _scheduleRepository.InsertSchedule(train, ScheduleSource.VSTP);
                }
                catch (Exception e)
                {
                    Trace.TraceError("Could not save VSTP Train: {0}", e);
                }
            });
        }

        public void CachePPMData(RtppmData data)
        {
            Task.Run(() =>
            {
                if (data == null)
                    return;
                Trace.TraceInformation("Saving PPM Data for {0}", data.Timestamp);

                if (data.NationalPPM != null)
                {
                    var nationalPPMId = _sectors
                        .Where(s => s.OperatorCode == null)
                        .Where(s => s.SectorCode == null)
                        .Select(s => s.PPMSectorId)
                        .SingleOrDefault();
                    if (nationalPPMId != null && nationalPPMId != Guid.Empty)
                    {
                        SavePPMData(data.NationalPPM, nationalPPMId, data.Timestamp);
                    }
                    else
                    {
                        Trace.TraceError("PPM: Could not find National PPM in Database");
                    }
                }
                if (data.Sectors != null)
                {
                    foreach (var nationalSector in data.Sectors)
                    {
                        var id = _sectors
                            .Where(s => s.OperatorCode == null)
                            .Where(s => s.SectorCode == nationalSector.Code)
                            .Select(s => s.PPMSectorId)
                            .SingleOrDefault();
                        if (id != null && id != Guid.Empty)
                        {
                            SavePPMData(nationalSector, id, data.Timestamp);
                        }
                        else
                        {
                            Trace.TraceError("PPM: Could not find Sector {0} in Database", nationalSector.Code);
                        }
                    }
                }
                if (data.Operators != null)
                {
                    foreach (var toc in data.Operators)
                    {
                        var id = _sectors
                            .Where(s => Convert.ToByte(s.OperatorCode) == Convert.ToByte(toc.Code))
                            .Where(s => s.SectorCode == null)
                            .Select(s => s.PPMSectorId)
                            .SingleOrDefault();
                        if (id != null && id != Guid.Empty)
                        {
                            SavePPMData(toc, id, data.Timestamp);
                        }
                        else
                        {
                            Trace.TraceError("PPM: Could not find TOC {0} in Database", toc.Code);
                        }
                        foreach (var tocSector in toc.ServiceGroups)
                        {
                            var sectorId = _sectors
                                .Where(s => Convert.ToByte(s.OperatorCode) == Convert.ToByte(toc.Code))
                                .Where(s => s.SectorCode != null)
                                .Where(s => s.SectorCode.Equals(tocSector.Code, StringComparison.InvariantCultureIgnoreCase))
                                .Where(s => s.Description.Equals(tocSector.Name, StringComparison.InvariantCultureIgnoreCase))
                                .Select(s => s.PPMSectorId)
                                .SingleOrDefault();
                            if (sectorId != null && sectorId != Guid.Empty)
                            {
                                SavePPMData(tocSector, sectorId, data.Timestamp);
                            }
                            else
                            {
                                Trace.TraceError("PPM: Could not find TOC {0} Sector {1}-{2} in Database", toc.Code, tocSector.Name, tocSector.Code);
                            }
                        }
                    }
                }
            });
        }

        private static void SavePPMData(PPMRecord record, Guid id, DateTime ts)
        {
            record.PPMSectorId = id;
            record.Timestamp = ts;
            _ppmRepository.AddPPMData(record);
        }
    }
}
