using System.Collections.Generic;
using System.ServiceModel;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.PPM;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Services
{
    /// <summary>
    /// A Cache service interface
    /// </summary>
    [ServiceContract]
    [ServiceKnownType(typeof(ScheduleTrain))]
    [ServiceKnownType(typeof(ScheduleStop))]
    [ServiceKnownType(typeof(TrainActivation))]
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
        /// <summary>
        /// Cache train data
        /// </summary>
        /// <param name="trainData">train data to cache</param>
        [OperationContract]
        void CacheTrainData(IEnumerable<ITrainData> trainData);

        /// <summary>
        /// Cache a VSTP Schedule
        /// </summary>
        /// <param name="train">the VSTP schedule</param>
        [OperationContract]
        void CacheVSTPSchedule(ScheduleTrain train);

        /// <summary>
        /// Cache PPM Data
        /// </summary>
        /// <param name="data">PPM data</param>
        [OperationContract]
        void CachePPMData(RtppmData data);
    }
}
