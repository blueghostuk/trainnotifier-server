using System;
using System.Collections.Generic;
using System.ServiceModel;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.PPM;
using TrainNotifier.Common.Model.Schedule;
using TrainNotifier.Common.Model.TDCache;
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

        public void CacheVSTPSchedule(ScheduleTrain train)
        {
            base.Channel.CacheVSTPSchedule(train);
        }

        public void CachePPMData(RtppmData data)
        {
            base.Channel.CachePPMData(data);
        }
    }

    internal sealed class TDCacheServiceClient : ClientBase<ITDService>, ITDService
    {
        public TDCacheServiceClient() : base("NetTcpBinding_ITDService") { }

        public void CacheTrainDescriberData(IEnumerable<TrainDescriber> trainData)
        {
            base.Channel.CacheTrainDescriberData(trainData);
        }

        public TDTrains GetTrain(string trainDescriber)
        {
            return base.Channel.GetTrain(trainDescriber);
        }

        public Tuple<DateTime, string> GetBerthContents(string berth)
        {
            return base.Channel.GetBerthContents(berth);
        }
    }

}
