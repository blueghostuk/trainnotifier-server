using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace TrainNotifier.Common.Model.TDCache
{
    [DataContract]
    public class TDTrain
    {
        private readonly ConcurrentDictionary<string, ConcurrentBag<TDBerth>> _berths = new ConcurrentDictionary<string, ConcurrentBag<TDBerth>>();

        private Dictionary<string, List<TDBerth>> _berthsLookup;
        [DataMember]
        public Dictionary<string, List<TDBerth>> Berths
        {
            get
            {
                return _berthsLookup ?? _berths.ToDictionary(
                    (kvp) =>
                    {
                        return kvp.Key;
                    },
                    (kvp) =>
                    {
                        return kvp.Value.OrderBy(b => b.FirstSeen).ToList();
                    });
            }
            set
            {
                _berthsLookup = value;
            }
        }

        [DataMember]
        public string Describer { get; set; }

        [DataMember]
        public DateTime FirstSeen { get; set; }

        public TDTrain(string describer, TDBerth initialBerth)
        {
            Describer = describer;
            FirstSeen = DateTime.UtcNow;
            UpdatePosition(initialBerth);
        }

        public TDBerth LastAt(string areaId)
        {
            return !_berths.ContainsKey(areaId) ? null : _berths[areaId].OrderByDescending(td => td.FirstSeen).LastOrDefault();
        }

        public void Exited(TDBerth berth)
        {
            if (_berths.ContainsKey(berth.AreaId))
            {
                var berthsToUpdate = _berths[berth.AreaId]
                    .Where(td => td.Name == berth.Name)
                    .Where(td => !td.Exited.HasValue);

                foreach (var b in berthsToUpdate)
                    b.Exited = DateTime.UtcNow;
            }
        }

        public void UpdatePosition(TDBerth berth)
        {
            _berths.AddOrUpdate(berth.AreaId, new ConcurrentBag<TDBerth> { berth }, (key, coll) =>
            {
                coll.Add(berth);
                return coll;
            });
        }
        
    }
}
