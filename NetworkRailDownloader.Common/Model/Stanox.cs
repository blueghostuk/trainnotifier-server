using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace TrainNotifier.Common.Model
{
    [DataContract]
    public sealed class Stanox
    {
        private ICollection<string> _trainIds;

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public IEnumerable<string> TrainIds
        {
            get
            {
                if (_trainIds == null)
                    return Enumerable.Empty<string>();

                return _trainIds;
            }
            set
            {
                if (value != null)
                {
                    _trainIds = new HashSet<string>(value);
                }
                else
                {
                    _trainIds = new HashSet<string>();
                }
            }
        }

        public Stanox()
        {
            _trainIds = new HashSet<string>();
        }

        public Stanox(string stanox) : this()
        {
            Name = stanox;
        }

        public void AddTrainId(string trainId)
        {
            _trainIds.Add(trainId);
        }
    }
}
