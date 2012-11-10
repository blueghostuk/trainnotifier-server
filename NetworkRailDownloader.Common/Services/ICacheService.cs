using System.Collections.Generic;
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
        void CacheTrainStep(string trainId, TrainMovementStep step);

        [OperationContract]
        void CacheTrainCancellation(string trainId, CancelledTrainMovementStep step);

        [OperationContract]
        bool TryGetStanox(string stanoxName, out Stanox stanox);

        [OperationContract]
        void CacheStation(string stanoxName, string trainId);
    }
}
