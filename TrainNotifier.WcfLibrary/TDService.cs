using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Timers;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.CorpusExtract;
using TrainNotifier.Common.Model.Schedule;
using TrainNotifier.Common.Model.SmartExtract;
using TrainNotifier.Common.Model.TDCache;
using TrainNotifier.Common.Services;
using TrainNotifier.Service;

namespace TrainNotifier.WcfLibrary
{
    public class TDCacheService : ITDService
    {
        private static readonly ObjectCache _tdCache = MemoryCache.Default;
        private static readonly CacheItemPolicy _tdCachePolicy = new CacheItemPolicy
        {
            SlidingExpiration = TimeSpan.FromHours(2)
        };

        private static readonly ILookup<string, TDElement> _tdElementsByArea;
        private static readonly ILookup<string, TiplocCode> _tiplocByStanox;

        private static readonly TiplocRepository _tiplocRepo = new TiplocRepository();
        private static readonly TrainMovementRepository _tmRepo = new TrainMovementRepository();
        private static readonly LiveTrainRepository _liveTrainRepo = new LiveTrainRepository();

        private static readonly ConcurrentBag<Tuple<TrainDescriber, TDElement, TiplocCode>> _missedEntries
            = new ConcurrentBag<Tuple<TrainDescriber, TDElement, TiplocCode>>();

        private static readonly Timer _missedEntryTimer = new Timer(TimeSpan.FromMinutes(3).TotalMilliseconds);

        static TDCacheService()
        {
            // this data is fetchable via http://nrodwiki.rockshore.net/index.php/ReferenceData

            List<TDElement> allSmartData = new List<TDElement>();

            foreach (string smartExtract in Directory.GetFiles("..\\", "SMARTExtract*.json"))
            {
                Trace.TraceInformation("Loading SMART data from {0}", smartExtract);
                string smartData = File.ReadAllText(smartExtract);
                allSmartData.AddRange(JsonConvert.DeserializeObject<TDContainer>(smartData).BERTHDATA);
            }
            _tdElementsByArea = allSmartData
                .Where(td => !string.IsNullOrEmpty(td.STANOX))
                .ToLookup(td => td.TD);

            var dbTiplocs = _tiplocRepo.Get();

            string tiplocData = File.ReadAllText("..\\CORPUSExtract.json");
            _tiplocByStanox = JsonConvert.DeserializeObject<TiplocContainer>(tiplocData).TIPLOCDATA
                .Select(t => t.ToTiplocCode())
                .ToLookup(t => t.Stanox);

            _missedEntryTimer.Elapsed += _missedEntryTimer_Elapsed;
            _missedEntryTimer.Start();
        }

        static void _missedEntryTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Tuple<TrainDescriber, TDElement, TiplocCode> current = null;
            Trace.TraceInformation("Processing failed items queue. Item Count - {0}", _missedEntries.Count);
            while (_missedEntries.TryTake(out current))
            {
                ProcessTdResult(current, false);
            }
        }

        public void CacheTrainDescriberData(IEnumerable<TrainDescriber> trainData)
        {
            Task.Run(() =>
            {
                CacheTdCaData(trainData.OfType<CaTD>());
                CacheTdCbData(trainData.OfType<CbTD>());
                CacheTdCCData(trainData.OfType<CcTD>());
            });
            Task.Run(() =>
            {
                UpdateTrainMovements(trainData);
            });
        }

        private void UpdateTrainMovements(IEnumerable<TrainDescriber> trainData)
        {
            // filter out anything with no area
            var trainDescribers = trainData
                .Where(tdesc => _tdElementsByArea.Contains(tdesc.AreaId))
                .ToList();

            ICollection<Tuple<TrainDescriber, TDElement, TiplocCode>> data = new List<Tuple<TrainDescriber, TDElement, TiplocCode>>();
            foreach (var tdesc in trainDescribers)
            {
                // get all matching elements in that area
                var tdElements = _tdElementsByArea[tdesc.AreaId].Where(td => td.Equals(tdesc)).ToList();
                if (tdElements.Any())
                {
                    foreach (var area in tdElements)
                    {
                        //find the matching location
                        TiplocCode tiploc = _tiplocByStanox[area.STANOX].FirstOrDefault();
                        if (tiploc != null)
                        {
                            data.Add(Tuple.Create(tdesc, area, tiploc));
                        }
                    }
                }
            }

            foreach (var td in data)
            {
                ProcessTdResult(td, true);
            }
        }

        private static void ProcessTdResult(Tuple<TrainDescriber, TDElement, TiplocCode> td, bool doRetry)
        {
            try
            {
                CachedTrainDetails tmr = GetTrainSchedule(td.Item1.Description, td.Item3);
                if (tmr != null)
                {
                    TDCacheService.SetCacheContents(IndexValue(td.Item1, td.Item1.GetIndex()), Tuple.Create(td.Item1.Time, td.Item1.Description, tmr));
                    switch (td.Item2.EventType)
                    {
                        case EventType.ArriveDown:
                        case EventType.ArriveUp:
                            if (!_liveTrainRepo.UpdateMovement(
                                tmr.Id,
                                td.Item2,
                                _tiplocRepo.GetTiplocsByStanox(td.Item3.Stanox).Select(st => st.TiplocId).ToArray(),
                                TrainMovementEventType.Arrival,
                                td.Item1.Time.AddSeconds(int.Parse(td.Item2.BERTHOFFSET))) && doRetry)
                            {
                                _missedEntries.Add(td);
                            }
                            break;
                        case EventType.DepartDown:
                        case EventType.DepartUp:
                            if (!_liveTrainRepo.UpdateMovement(
                                tmr.Id,
                                td.Item2,
                                _tiplocRepo.GetTiplocsByStanox(td.Item3.Stanox).Select(st => st.TiplocId).ToArray(),
                                TrainMovementEventType.Departure,
                                td.Item1.Time.AddSeconds(int.Parse(td.Item2.BERTHOFFSET))) && doRetry)
                            {
                                _missedEntries.Add(td);
                            }
                            break;
                    }
                }
                else
                {
                    if (doRetry)
                        _missedEntries.Add(td);
                }
            }
            catch (SqlException e)
            {
                // if timeout then add back queue
                if (e.Number == -2)
                {
                    _missedEntries.Add(td);
                }
            }
        }

        private static CachedTrainDetails GetTrainSchedule(string describer, TiplocCode tiploc)
        {
            if (string.IsNullOrEmpty(describer) || tiploc == null)
                return null;

            return _tmRepo.GetActivatedTrainMovementByHeadcodeAndStop(describer, DateTime.UtcNow, tiploc.Stanox);
        }

        private static string IndexValue(TrainDescriber td, string post)
        {
            return string.Format("{0}-{1}", td.AreaId, post);
        }

        private void CacheTdCaData(IEnumerable<CaTD> trainData)
        {
            foreach (var ca in trainData)
            {
                TDCacheService.SetCacheContents(IndexValue(ca, ca.To), Tuple.Create(ca.Time, ca.Description, default(CachedTrainDetails)));
                TDCacheService.SetCacheContents(IndexValue(ca, ca.From), null);
            }
        }

        private void CacheTdCbData(IEnumerable<CbTD> trainData)
        {
            foreach (var cb in trainData)
            {
                TDCacheService.SetCacheContents(IndexValue(cb, cb.From), null);
            }
        }

        private void CacheTdCCData(IEnumerable<CcTD> trainData)
        {
            foreach (var cc in trainData)
            {
                TDCacheService.SetCacheContents(IndexValue(cc, cc.To), Tuple.Create(cc.Time, cc.Description, default(CachedTrainDetails)));
            }
        }

        public Tuple<DateTime, string, CachedTrainDetails> GetBerthContents(string index)
        {
            return _tdCache[index] as Tuple<DateTime, string, CachedTrainDetails>;
        }

        public static void SetCacheContents(string index, Tuple<DateTime, string, CachedTrainDetails> item)
        {
            if (item == null)
            {
                _tdCache.Remove(index);
            }
            else
            {
                _tdCache.Set(index, item, _tdCachePolicy);
            }
        }
    }
}
