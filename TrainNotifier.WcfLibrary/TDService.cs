using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.Api;
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

        private static readonly IDictionary<string, TDElement> _smartData;
        private static readonly ConcurrentDictionary<string, TiplocCode> _tiplocCache = new ConcurrentDictionary<string, TiplocCode>();

        private static readonly TiplocRepository _tiplocRepo = new TiplocRepository();
        private static readonly TrainMovementRepository _tmRepo = new TrainMovementRepository();

        static TDCacheService ()
	    {            
            // this data is fetchable via http://nrodwiki.rockshore.net/index.php/ReferenceData
            string jsonData = File.ReadAllText("..\\SMARTExtract.json");

            TDContainer container = JsonConvert.DeserializeObject<TDContainer>(jsonData);

            try
            {
                _smartData = container.BERTHDATA
                    .Where(bd => !string.IsNullOrEmpty(bd.TD))
                    .Where(bd => !string.IsNullOrEmpty(bd.TOBERTH))
                    .Distinct(new TDElementEqualityComparer())
                    .ToDictionary(bd => string.Format("{0}-{1}", bd.TD, bd.TOBERTH));
            }
            catch (Exception e)
            {
                Trace.TraceError("{0}", e);
            }

            const string sql = @"
                SELECT 
                    [Id]
                    ,[TrainId]
                    ,[ScheduleTrain]
                FROM [LiveTrain] 
                WHERE [TrainStateId] = @activated
                    AND [OriginDepartTimestamp] >= (GETDATE() - 0.5)";

            var activeTrains = Query<dynamic>(sql, new { TrainState.Activated });

            Trace.TraceInformation("Pre loading {0} trains", activeTrains.Count());

            Trace.Flush();

            foreach (var activeTrain in activeTrains)
            {
                _trainActivationCache.Add(activeTrain.TrainId, new TrainMovementSchedule
                {
                    Id = activeTrain.Id,
                    Schedule = activeTrain.ScheduleTrain,
                    StopNumber = 0
                }, _trainActivationCachePolicy);
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
        }

        private static string IndexValue(TrainDescriber td, string post)
        {
            return string.Format("{0}-{1}", td.AreaId, post);
        }

        private void CacheTdCaData(IEnumerable<CaTD> trainData)
        {
            foreach (var ca in trainData)
            {
                UpdateTrain(ca);
                this[IndexValue(ca, ca.To)] = new Tuple<DateTime,string>(ca.Time, ca.Description);
                this[IndexValue(ca, ca.From)] = null;
            }
        }

        private void CacheTdCbData(IEnumerable<CbTD> trainData)
        {
            foreach (var cb in trainData)
            {
                UpdateTrain(cb);
                this[IndexValue(cb, cb.From)] = null;
            }
        }

        private void CacheTdCCData(IEnumerable<CcTD> trainData)
        {
            foreach (var cc in trainData)
            {
                UpdateTrain(cc);
                this[cc.Description] = new Tuple<DateTime, string>(cc.Time, IndexValue(cc, cc.To));
                this[IndexValue(cc, cc.To)] = new Tuple<DateTime, string>(cc.Time, cc.Description);
            }
        }

        private static void UpdateTrain(CaTD caTD)
        {
            TDElement element;
            _smartData.TryGetValue(string.Format("{0}-{1}", caTD.AreaId, caTD.To), out element);
            TiplocCode tiploc = GetTiplocCode(element);
            TDBerth berth = new TDBerth(caTD.AreaId, caTD.To, element, tiploc);
            TDTrain train = GetTrainDescriber(caTD.Description, berth, tiploc);
            _smartData.TryGetValue(string.Format("{0}-{1}", caTD.AreaId, caTD.From), out element);
            train.Exited(new TDBerth(caTD.AreaId, caTD.From, element, tiploc));
            train.UpdatePosition(berth);
        }

        private static void UpdateTrain(CbTD cbTD)
        {
            TDElement element;
            _smartData.TryGetValue(string.Format("{0}-{1}", cbTD.AreaId, cbTD.From), out element);
            TiplocCode tiploc = GetTiplocCode(element);
            TDBerth berth = new TDBerth(cbTD.AreaId, cbTD.From, element, tiploc);
            TDTrain train = GetTrainDescriber(cbTD.Description, berth, tiploc);
            if (train != null)
            {
                train.Exited(berth);
            }
        }

        private static void UpdateTrain(CcTD ccTd)
        {
            TDElement element;
            _smartData.TryGetValue(string.Format("{0}-{1}", ccTd.AreaId, ccTd.To), out element);
            TiplocCode tiploc = GetTiplocCode(element);
            TDBerth berth = new TDBerth(ccTd.AreaId, ccTd.To, element, tiploc);
            TDTrain train = GetTrainDescriber(ccTd.Description, berth, tiploc);
            train.UpdatePosition(berth);
        }
        private static TiplocCode GetTiplocCode(TDElement element)
        {
            if (element == null)
                return null;

            TiplocCode code = null;
            if (!_tiplocCache.TryGetValue(element.STANOX, out code))
            {
                code = _tiplocRepo.GetTiplocByStanox(element.STANOX);
                _tiplocCache.AddOrUpdate(element.STANOX, code, (key, tiploc) => { return code; });
            }
            return code;
        }

        private static TDTrain GetTrainDescriber(string describer, TDBerth berth, TiplocCode tiploc)
        {
            TDTrains trains = _tdCache[describer] as TDTrains;
            TrainMovementResult schedule = GetTrainSchedule(describer, tiploc);
            if (trains == null)
            {
                trains = new TDTrains { Describer = describer };
                TDTrain train = new TDTrain(describer, berth);
                train.Schedule = schedule;
                trains.Trains.Add(train);
                _tdCache.Add(describer, trains, _tdCachePolicy);
                return train;
            }
            else
            {
                var matching = trains.Trains.Where(t => t.Berths.ContainsKey(berth.AreaId));
                if (matching.Count() > 1)
                {
                    if (schedule == null)
                    {
                        TDTrain train = matching.OrderByDescending(t => t.LastAt(berth.AreaId))
                            .First();

                        if (train.Schedule == null)
                            train.Schedule = GetTrainSchedule(describer, tiploc);
                        return train;
                    }
                    else
                    {
                        TDTrain train = trains.Trains.FirstOrDefault(t => t.Schedule == schedule);
                        if (train != null)
                            return train;

                        train = new TDTrain(describer, berth);
                        train.Schedule = schedule;
                        trains.Trains.Add(train);
                        return train;
                    }
                }
                else if (matching.Count() == 1)
                {
                    TDTrain train = matching.Single();
                    if (train.Schedule == null)
                        train.Schedule = GetTrainSchedule(describer, tiploc);
                    return train;
                }
                else
                {
                    TDTrain train = trains.Trains.FirstOrDefault(t => t.Schedule == schedule);
                    if (train != null)
                        return train;

                    train = new TDTrain(describer, berth);
                    train.Schedule = schedule;
                    trains.Trains.Add(train);
                    return train;
                }
            }
        }

        private static TrainMovementResult GetTrainSchedule(string describer, TiplocCode tiploc)
        {
            if (tiploc == null)
                return null;

            IEnumerable<TrainMovementResult> results = _tmRepo.GetTrainMovementByHeadcode(describer, DateTime.UtcNow);
            if (!results.Any())
                return null;

            var filteredResults = results
                .Where(r => r.Schedule.Stops.Any(s => s.Tiploc.Tiploc == tiploc.Tiploc));

            if (!filteredResults.Any())
                return null;

            if (filteredResults.Count() == 0)
                return filteredResults.Single();

            var currentResults = results
                .Where(r => r.Actual != null)
                .Where(r => r.Actual.State == TrainState.Activated);

            return currentResults.ElementAtOrDefault(0);
        }

        public TDTrains GetTrain(string describer)
        {
            return _tdCache[describer] as TDTrains;
        }


        public Tuple<DateTime, string> GetBerthContents(string berth)
        {
            return _tdCache[berth] as Tuple<DateTime, string>;
        }

        public Tuple<DateTime, string> this[string index]
        {
            set
            {
                if (value == null)
                {
                    _tdCache.Remove(index);
                }
                else
                {
                    _tdCache.Set(index, value, _tdCachePolicy);
                }
            }
        }
    }
}
