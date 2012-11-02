using System;
using System.Runtime.Serialization;

namespace NetworkRailDownloader.Common.Model
{
    [DataContract]
    public sealed class TrainMovementStep
    {
        [DataMember]
        public string EventType { get; set; }

        [DataMember]
        public DateTime? PlannedTime { get; set; }

        [DataMember]
        public DateTime ActualTimeStamp { get; set; }

        [DataMember]
        public bool Terminated { get; set; }

        [DataMember]
        public string Stanox { get; set; }

        [DataMember]
        public string Line { get; set; }

        [DataMember]
        public string Platform { get; set; }
    }
}
