using System;
using System.Runtime.Serialization;

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
}
