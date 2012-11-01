using System.Collections.Generic;

namespace NetworkRailDownloader.Console.Model
{
    public sealed class Stanox
    {
        private readonly ICollection<string> _trainIds;
        private readonly string _stanox;

        public string Name { get { return _stanox; } }

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
