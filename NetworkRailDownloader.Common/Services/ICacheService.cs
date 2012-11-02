using NetworkRailDownloader.Common.Model;
using System.ServiceModel;

namespace NetworkRailDownloader.Common.Services
{
    [ServiceContract]
    public interface ICacheService
    {
        [OperationContract]
        void CacheTrainMovement(TrainMovement trainMovement);

        [OperationContract]
        void CacheTrainStep(string trainId, string serviceCode, TrainMovementStep step);

        [OperationContract]
        bool TryGetTrainMovement(string trainId, out TrainMovement trainMovement);

        [OperationContract]
        bool TryGetStanox(string stanoxName, out Stanox stanox);

        [OperationContract]
        void CacheStation(string stanoxName, string trainId);
    }
}
