using System;
using System.Runtime.Serialization;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model
{
    [DataContract]
    public class Reinstatement
    {
        [IgnoreDataMember]
        public Guid TrainId { get; set; }

        [DataMember]
        public TiplocCode NewOrigin { get; set; }

        [DataMember]
        public DateTime PlannedDepartureTime { get; set; }
    }
}
