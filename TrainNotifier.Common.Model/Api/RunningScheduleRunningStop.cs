using System;
using System.Runtime.Serialization;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model.Api
{
    [DataContract]
    public class RunningScheduleRunningStop
    {
        [IgnoreDataMember]
        public Guid ScheduleId { get; set; }

        [IgnoreDataMember]
        public StationTiploc Tiploc { get; set; }
        [DataMember]
        public string TiplocStanoxCode { get; set; }

        [DataMember]
        public byte StopNumber { get; set; }
        [DataMember]
        public TimeSpan? Arrival { get; set; }
        [DataMember]
        public TimeSpan? Departure { get; set; }
        [DataMember]
        public TimeSpan? Pass { get; set; }
        [DataMember]
        public TimeSpan? PublicArrival { get; set; }
        [DataMember]
        public TimeSpan? PublicDeparture { get; set; }
        [DataMember]
        public string Line { get; set; }
        [DataMember]
        public string Path { get; set; }
        [DataMember]
        public string Platform { get; set; }
        [DataMember]
        public byte? EngineeringAllowance { get; set; }
        [DataMember]
        public byte? PathingAllowance { get; set; }
        [DataMember]
        public byte? PerformanceAllowance { get; set; }
        [DataMember]
        public bool Origin { get; set; }
        [DataMember]
        public bool Intermediate { get; set; }
        [DataMember]
        public bool Terminate { get; set; }
    }
}
