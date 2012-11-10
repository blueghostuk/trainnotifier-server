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

        public void CacheTrainStep(string trainId, TrainMovementStep step)
        {
            base.Channel.CacheTrainStep(trainId, step);
        }

        public void CacheTrainCancellation(string trainId, CancelledTrainMovementStep step)
        {
            base.Channel.CacheTrainCancellation(trainId, step);
        }

        public bool TryGetStanox(string stanoxName, out Stanox stanox)
        {
            return base.Channel.TryGetStanox(stanoxName, out stanox);
        }

        public void CacheStation(string stanoxName, string trainId)
        {
            base.Channel.CacheStation(stanoxName, trainId);
        }
    }
}
