using System;
using System.Runtime.Serialization;

namespace TrainNotifier.Common.Model.Schedule
{
    [DataContract]
    public class ScheduleStop
    {
        [IgnoreDataMember]
        public Guid ScheduleId { get; set; }
        [DataMember]
        public TiplocCode Tiploc { get; set; }
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
