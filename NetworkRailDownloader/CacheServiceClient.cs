using NetworkRailDownloader.Common.Services;
using System.ServiceModel;

namespace NetworkRailDownloader.Common
{
    internal sealed class CacheServiceClient : ClientBase<ICacheService>, ICacheService
    {
        public CacheServiceClient() : base("NetTcpBinding_ICacheService") { }

        public void CacheTrainMovement(Model.TrainMovement trainMovement)
        {
            base.Channel.CacheTrainMovement(trainMovement);
        }

        public void CacheTrainStep(string trainId, string serviceCode, Model.TrainMovementStep step)
        {
            base.Channel.CacheTrainStep(trainId, serviceCode, step);
        }

        public bool TryGetTrainMovement(string trainId, out Model.TrainMovement trainMovement)
        {
            return base.Channel.TryGetTrainMovement(trainId, out trainMovement);
        }

        public bool TryGetStanox(string stanoxName, out Model.Stanox stanox)
        {
            return base.Channel.TryGetStanox(stanoxName, out stanox);
        }

        public void CacheStation(string stanoxName, string trainId)
        {
            base.Channel.CacheStation(stanoxName, trainId);
        }
    }
}
