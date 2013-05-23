using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model.Api
{
    [DataContract]
    public sealed class RunningScheduleTrain
    {
        [IgnoreDataMember]
        public Guid ScheduleId { get; set; }
        [DataMember]
        public string TrainUid { get; set; }
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime? EndDate { get; set; }
        [DataMember]
        public AtocCode AtocCode { get; set; }
        [DataMember]
        public Schedule.Schedule Schedule { get; set; }
        [DataMember]
        public Status? ScheduleStatusId { get; set; }
        [DataMember]
        public STPIndicator STPIndicatorId { get; set; }

        [DataMember]
        public IEnumerable<RunningScheduleRunningStop> Stops { get; set; }
    }

    [DataContract]
    public enum ScheduleSource : byte
    {
        [EnumMember]
        CIF = 0,
        [EnumMember]
        VSTP = 1
    }
}
