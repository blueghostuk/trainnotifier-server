using System;
using System.Runtime.Serialization;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model.Api
{
    [DataContract]
    public class RunningTrainActualStop
    {
        [IgnoreDataMember]
        public Guid TrainId { get; set; }
        [DataMember]
        public TrainMovementEventType EventType { get; set; }
        [DataMember]
        public DateTime? PlannedTimestamp { get; set; }
        [DataMember]
        public DateTime ActualTimestamp { get; set; }
        [DataMember]
        public string Line { get; set; }
        [DataMember]
        public string Platform { get; set; }
        [DataMember]
        public byte? ScheduleStopNumber { get; set; }

        [IgnoreDataMember]
        public StationTiploc Tiploc { get; set; }
        [DataMember]
        public string TiplocStanoxCode { get; set; }
    }
}
