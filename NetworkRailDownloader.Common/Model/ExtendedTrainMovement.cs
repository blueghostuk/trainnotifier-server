using System;
using System.Runtime.Serialization;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model
{
    [DataContract]
    public class OriginTrainMovement : TrainMovement
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

    [DataContract]
    public sealed class CallingAtTrainMovement : OriginTrainMovement
    {
        [DataMember]
        public TimeSpan? Pass { get; set; }
    }
}
