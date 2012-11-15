using System.Collections.Generic;
using System.ServiceModel;
using TrainNotifier.Common.Model;

namespace TrainNotifier.Common.Services
{
    [ServiceContract]
    [ServiceKnownType(typeof(TrainMovement))]
    [ServiceKnownType(typeof(TrainMovementStep))]
    [ServiceKnownType(typeof(CancelledTrainMovementStep))]
    [ServiceKnownType(typeof(CaTD))]
    [ServiceKnownType(typeof(CbTD))]
    [ServiceKnownType(typeof(CcTD))]
    [ServiceKnownType(typeof(CtTD))]
    public interface ICacheService
    {
        [OperationContract]
        bool TryGetStanox(string stanoxName, out Stanox stanox);

        [OperationContract]
        void CacheStation(string stanoxName, string trainId);

        [OperationContract]
        void CacheTrainData(IEnumerable<ITrainData> trainData);

        [OperationContract]
        void CacheTrainDescriberData(IEnumerable<TrainDescriber> trainData);
    }
}
