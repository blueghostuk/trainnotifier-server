using System;
using System.Runtime.Serialization;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model
{
    [DataContract]
    public class ScheduleViewModel
    {
        [DataMember]
        public string TrainUid { get; set; }
        [DataMember]
        public AtocCode AtocCode { get; set; }
        [DataMember]
        public Status? ScheduleStatusId { get; set; }
        [DataMember]
        public STPIndicator STPIndicatorId { get; set; }
        [DataMember]
        public TimeSpan Departure { get; set; }
        [DataMember]
        public TimeSpan? PublicDeparture { get; set; }
        [DataMember]
        public TiplocCode Origin { get; set; }
        [DataMember]
        public TiplocCode Destination { get; set; }
    }
}
