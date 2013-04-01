using System.Collections.Generic;
using System.ServiceModel;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.Schedule;
using TrainNotifier.Common.PPM;
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

        public void CacheVSTPSchedule(ScheduleTrain train)
        {
            base.Channel.CacheVSTPSchedule(train);
        }


        public void CachePPMData(RtppmData data)
        {
            base.Channel.CachePPMData(data);
        }
    }
}
