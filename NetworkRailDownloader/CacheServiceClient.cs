using System.Collections.Generic;
using System.ServiceModel;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Services;

namespace TrainNotifier.Console.WebSocketServer
{
    internal sealed class CacheServiceClient : ClientBase<ICacheService>, ICacheService
    {
        public CacheServiceClient() : base("NetTcpBinding_ICacheService") { }

        public void CacheTrainMovement(TrainMovement trainMovement)
        {
            base.Channel.CacheTrainMovement(trainMovement);
        }

        public void CacheTrainStep(string trainId, string serviceCode, TrainMovementStep step)
        {
            base.Channel.CacheTrainStep(trainId, serviceCode, step);
        }

        public void CacheTrainCancellation(string trainId, string serviceCode, CancelledTrainMovementStep step)
        {
            base.Channel.CacheTrainCancellation(trainId, serviceCode, step);
        }

        public bool TryGetTrainMovement(string trainId, out TrainMovement trainMovement)
        {
            return base.Channel.TryGetTrainMovement(trainId, out trainMovement);
        }

        public bool TryGetStanox(string stanoxName, out Stanox stanox)
        {
            return base.Channel.TryGetStanox(stanoxName, out stanox);
        }

        public bool TryGetService(string headCode, out IEnumerable<string> trainIds)
        {
            return base.Channel.TryGetService(headCode, out trainIds);
        }

        public void CacheStation(string stanoxName, string trainId)
        {
            base.Channel.CacheStation(stanoxName, trainId);
        }
    }
}
