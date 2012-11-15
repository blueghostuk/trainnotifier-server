using System.Collections.Generic;
using System.ServiceModel;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Services;

namespace TrainNotifier.Console.WebSocketServer
{
    internal sealed class CacheServiceClient : ClientBase<ICacheService>, ICacheService
    {
        public CacheServiceClient() : base("NetTcpBinding_ICacheService") { }

        public bool TryGetStanox(string stanoxName, out Stanox stanox)
        {
            return base.Channel.TryGetStanox(stanoxName, out stanox);
        }

        public void CacheStation(string stanoxName, string trainId)
        {
            base.Channel.CacheStation(stanoxName, trainId);
        }

        public void CacheTrainData(IEnumerable<ITrainData> trainData)
        {
            base.Channel.CacheTrainData(trainData);
        }

        public void CacheTrainDescriberData(IEnumerable<TrainDescriber> trainData)
        {
            base.Channel.CacheTrainDescriberData(trainData);
        }
    }
}
