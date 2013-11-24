using System;
using System.Runtime.Serialization;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model
{
    [DataContract]
    public class Cancellation
    {
        [IgnoreDataMember]
        public Guid TrainId { get; set; }

        [DataMember]
        public string CancelledStanox { get; set; }

        [DataMember]
        public DateTime? CancelledTimestamp { get; set; }

        [DataMember]
        public string ReasonCode { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Type { get; set; }
    }

    [DataContract]
    public class ExtendedCancellation : Cancellation
    {
        [IgnoreDataMember]
        public StationTiploc CancelledAt { get; set; }
        [DataMember]
        public string CancelledAtStanoxCode { get; set; }
    }
}
