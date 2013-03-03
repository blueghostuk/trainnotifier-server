using System;
using System.Runtime.Serialization;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model
{
    [DataContract]
    public sealed class OriginTrainMovement : TrainMovement
    {
        [DataMember]
        public ScheduleTiploc Origin { get; set; }
        [DataMember]
        public ScheduleTiploc Destination { get; set; }
        [IgnoreDataMember]
        public Guid? ScheduleId { get; set; }
        [DataMember]
        public AtocCode AtocCode { get; set; }
    }
}
