using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TrainNotifier.Common.Model
{
    [DataContract]
    public sealed class Stanox
    {
        private readonly ICollection<string> _trainIds;
        private readonly string _stanox;

        [DataMember]
        public string Name { get { return _stanox; } }

        [DataMember]
        public IEnumerable<string> TrainIds { get { return _trainIds; } }

        public Stanox(string stanox)
        {
            _stanox = stanox;
            _trainIds = new HashSet<string>();
        }

        public void AddTrainId(string trainId)
        {
            _trainIds.Add(trainId);
        }
    }
}
