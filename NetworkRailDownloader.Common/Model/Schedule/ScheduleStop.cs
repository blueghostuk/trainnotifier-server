using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TrainNotifier.Common.Model.Schedule
{
    [DataContract]
    public class ScheduleStop
    {
        [DataMember]
        public Guid ScheduleId { get; set; }
        [DataMember]
        public string TiplocCode { get; set; }
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
