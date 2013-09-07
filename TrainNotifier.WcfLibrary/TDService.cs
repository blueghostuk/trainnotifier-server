using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using TrainNotifier.Common.Model;
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
                this[ca.Description] = new Tuple<DateTime, string>(ca.Time, IndexValue(ca, ca.To));
                this[IndexValue(ca, ca.To)] = new Tuple<DateTime,string>(ca.Time, ca.Description);
                this[IndexValue(ca, ca.From)] = null;
            }
        }

        private void CacheTdCbData(IEnumerable<CbTD> trainData)
        {
            foreach (var cb in trainData)
            {
                this[cb.Description] = null;
                this[IndexValue(cb, cb.From)] = null;
            }
        }

        private void CacheTdCCData(IEnumerable<CcTD> trainData)
        {
            foreach (var cc in trainData)
            {
                this[cc.Description] = new Tuple<DateTime, string>(cc.Time, IndexValue(cc, cc.To));
                this[IndexValue(cc, cc.To)] = new Tuple<DateTime, string>(cc.Time, cc.Description);
            }
        }

        public Tuple<DateTime, string> GetTrainLocation(string trainDescriber)
        {
            return this[trainDescriber];
        }

        public Tuple<DateTime, string> GetBerthContents(string berth)
        {
            return this[berth];
        }

        public Tuple<DateTime, string> this[string index]
        {
            get
            {
                return _tdCache[index] as Tuple<DateTime, string>;
            }
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
