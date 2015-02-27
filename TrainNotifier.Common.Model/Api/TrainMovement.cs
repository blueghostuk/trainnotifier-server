using System;
using System.Runtime.Serialization;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model.Api
{
    [DataContract]
    public class TrainMovement
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Uid { get; set; }
        [DataMember]
        public string AtocCode { get; set; }
        [DataMember]
        public TimingPoint Arrival { get; set; }
        [DataMember]
        public TimingPoint Departure { get; set; }
        [DataMember]
        public Station From { get; set; }
        [DataMember]
        public Station To { get; set; }
        [DataMember]
        public string Platform { get; set; }
        [DataMember]
        public STPIndicator STPIndicator { get; set; }
        [DataMember]
        public bool Activated { get; set; }
        [DataMember]
        public bool Cancelled { get; set; }
    }

    [DataContract]
    public class AtTrainMovement : TrainMovement
    {
        [DataMember]
        public bool StartsHere { get; set; }
        [DataMember]
        public bool TerminatesHere { get; set; }
    }

    [DataContract]
    public class TimingPoint
    {
        [DataMember]
        public TimeSpan? Wtt { get; set; }
        [DataMember]
        public TimeSpan? Public { get; set; }
        [DataMember]
        public TimeSpan? Actual { get; set; }
    }

    [DataContract]
    public class Station
    {
        [DataMember]
        public string ExpectedStation { get; set; }
        [DataMember]
        public string ActualStation { get; set; }
    }
}
