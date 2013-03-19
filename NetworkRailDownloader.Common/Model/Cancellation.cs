using System;
using System.Runtime.Serialization;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model
{
    [DataContract]
    public class Cancellation
    {
        [DataMember]
        public string CancelledStanox { get; set; }

        [DataMember]
        public DateTime? CancelledTimestamp { get; set; }

        [DataMember]
        public string ReasonCode { get; set; }

        [DataMember]
        public string Type { get; set; }
    }

    [DataContract]
    public class ExtendedCancellation : Cancellation
    {
        [DataMember]
        public TiplocCode CancelledAt { get; set; }

        [DataMember]
        public ScheduleTrain Schedule { get; set; }
    }
}
