using System.ServiceModel;
using TrainNotifier.Common.Model;

namespace TrainNotifier.Common.Services
{
    [ServiceContract]
    public interface ICacheService
    {
        [OperationContract]
        void CacheTrainMovement(TrainMovement trainMovement);

        [OperationContract]
        void CacheTrainStep(string trainId, string serviceCode, TrainMovementStep step);

        [OperationContract]
        void CacheTrainCancellation(string trainId, string serviceCode, CancelledTrainMovementStep step);

        [OperationContract]
        bool TryGetTrainMovement(string trainId, out TrainMovement trainMovement);

        [OperationContract]
        bool TryGetStanox(string stanoxName, out Stanox stanox);

        [OperationContract]
        void CacheStation(string stanoxName, string trainId);
    }
}
