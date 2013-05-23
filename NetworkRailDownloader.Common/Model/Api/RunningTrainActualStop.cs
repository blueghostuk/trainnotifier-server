using System;
using System.Runtime.Serialization;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model.Api
{
    [DataContract]
    public class RunningTrainActualStop
    {
        [IgnoreDataMember]
        public Guid TrainStopId { get; set; }
        [IgnoreDataMember]
        public Guid TrainId { get; set; }
        [DataMember]
        public TrainMovementEventType EventType { get; set; }
        [DataMember]
        public DateTime? PlannedTime { get; set; }
        [DataMember]
        public DateTime ActualTimeStamp { get; set; }
        [DataMember]
        public string Line { get; set; }
        [DataMember]
        public string Platform { get; set; }
        [DataMember]
        public byte? ScheduleStopNumber { get; set; }

        [DataMember]
        public StationTiploc Tiploc { get; set; }
    }
}
