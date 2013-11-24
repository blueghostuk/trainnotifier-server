using System;
using System.Runtime.Serialization;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model
{
    [DataContract]
    public class ChangeOfOrigin
    {
        [IgnoreDataMember]
        public Guid TrainId { get; set; }

        [IgnoreDataMember]
        public StationTiploc NewOrigin { get; set; }
        [DataMember]
        public string NewOriginStanoxCode { get; set; }

        [DataMember]
        public string ReasonCode { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public DateTime NewDepartureTime { get; set; }
    }
}
