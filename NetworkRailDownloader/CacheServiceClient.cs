using System.Collections.Generic;
using System.ServiceModel;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Services;

namespace TrainNotifier.Console.WebSocketServer
{
    internal sealed class CacheServiceClient : ClientBase<ICacheService>, ICacheService
    {
        public CacheServiceClient() : base("NetTcpBinding_ICacheService") { }

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
