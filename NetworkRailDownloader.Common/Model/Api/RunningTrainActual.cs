using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model.Api
{
    [DataContract]
    public class RunningTrainActual 
    {
        [IgnoreDataMember]
        public Guid Id { get; set; }
        [IgnoreDataMember]
        public Guid ScheduleId { get; set; }
        [DataMember]
        public DateTime? Activated { get; set; }
        [DataMember]
        public string TrainId { get; set; }
        [DataMember]
        public string HeadCode { get; set; }
        [DataMember]
        public string TrainServiceCode { get; set; }
        [DataMember]
        public DateTime? OriginDepartTimestamp { get; set; }
        [DataMember]
        public TrainState State { get; private set; }

        [IgnoreDataMember]
        public StationTiploc ScheduleOrigin { get; set; }
        [DataMember]
        public string ScheduleOriginStanoxCode { get; set; }

        [DataMember]
        public IEnumerable<RunningTrainActualStop> Stops { get; set; }
    }
}
