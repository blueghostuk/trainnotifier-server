using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.SmartExtract;
using TrainNotifier.Common.Model.TDCache;
using TrainNotifier.Common.Services;

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

        private void UpdateTrain(CaTD caTD)
        {
            TDElement element;
            _smartData.TryGetValue(string.Format("{0}-{1}", caTD.AreaId, caTD.To), out element);
            TDBerth berth = new TDBerth(caTD.AreaId, caTD.To, element);
            TDTrain train = GetTrainDescriber(caTD.Description);
            if (train == null)
            {
                _tdCache.Set(caTD.Description, new TDTrain(caTD.Description, berth), _tdCachePolicy);
            }
            else
            {
                _smartData.TryGetValue(string.Format("{0}-{1}", caTD.AreaId, caTD.From), out element);
                train.Exited(new TDBerth(caTD.AreaId, caTD.From, element));
                train.UpdatePosition(berth);
            }
        }

        private void UpdateTrain(CbTD cbTD)
        {
            TDElement element;
            _smartData.TryGetValue(string.Format("{0}-{1}", cbTD.AreaId, cbTD.From), out element);
            TDBerth berth = new TDBerth(cbTD.AreaId, cbTD.From, element);
            TDTrain train = GetTrainDescriber(cbTD.Description);
            if (train != null)
            {
                train.Exited(berth);
            }
        }

        private void UpdateTrain(CcTD ccTd)
        {
            TDElement element;
            _smartData.TryGetValue(string.Format("{0}-{1}", ccTd.AreaId, ccTd.To), out element);
            TDBerth berth = new TDBerth(ccTd.AreaId, ccTd.To, element);
            TDTrain train = GetTrainDescriber(ccTd.Description);
            if (train == null)
            {
                _tdCache.Set(ccTd.Description, new TDTrain(ccTd.Description, berth), _tdCachePolicy);
            }
            else
            {
                train.UpdatePosition(berth);
            }
        }

        private TDTrain GetTrainDescriber(string describer)
        {
            return _tdCache[describer] as TDTrain;
        }

        public TDTrain GetTrain(string describer)
        {
            return GetTrainDescriber(describer);
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
