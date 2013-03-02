using System;
using System.Runtime.Serialization;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model
{
    [DataContract]
    public sealed class OriginTrainMovement : TrainMovement
    {
        [DataMember]
        public TiplocCode Origin { get; set; }
        [DataMember]
        public TiplocCode Destination { get; set; }
        [DataMember]
        public Guid? ScheduleId { get; set; }
        [DataMember]
        public AtocCode AtocCode { get; set; }
        [DataMember]
        public TimeSpan? OriginDeparture { get; set; }
        [DataMember]
        public TimeSpan? OriginPublicDeparture { get; set; }
        [DataMember]
        public TimeSpan? DestinationArrival { get; set; }
        [DataMember]
        public TimeSpan? DestinationPublicArrival { get; set; }
    }
}
