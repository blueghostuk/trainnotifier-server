using System.Collections.Generic;
using System.ServiceModel;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.PPM;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Services
{
    [ServiceContract]
    [ServiceKnownType(typeof(ScheduleTrain))]
    [ServiceKnownType(typeof(ScheduleStop))]
    [ServiceKnownType(typeof(TrainMovement))]
    [ServiceKnownType(typeof(TrainMovementStep))]
    [ServiceKnownType(typeof(CancelledTrainMovementStep))]
    [ServiceKnownType(typeof(TrainChangeOfOrigin))]
    [ServiceKnownType(typeof(TrainReinstatement))]
    [ServiceKnownType(typeof(RtppmData))]
    [ServiceKnownType(typeof(PPMRecord))]
    [ServiceKnownType(typeof(CaTD))]
    [ServiceKnownType(typeof(CbTD))]
    [ServiceKnownType(typeof(CcTD))]
    [ServiceKnownType(typeof(CtTD))]
    public interface ICacheService
    {
        [OperationContract]
        void CacheTrainData(IEnumerable<ITrainData> trainData);

        [OperationContract]
        void CacheVSTPSchedule(ScheduleTrain train);

        [OperationContract]
        void CachePPMData(RtppmData data);
    }
}
