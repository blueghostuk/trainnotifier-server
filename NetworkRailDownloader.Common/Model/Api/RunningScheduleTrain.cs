using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TrainNotifier.Common.Model.Schedule;
using System.Linq;

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
        public string Headcode { get; set; }
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
        public PowerType? PowerTypeId { get; set; }
        [DataMember]
        public TrainCategory? CategoryTypeId { get; set; }
        [DataMember]
        public byte? Speed { get; set; }

        [DataMember]
        public IEnumerable<RunningScheduleRunningStop> Stops { get; set; }

        [IgnoreDataMember]
        public TimeSpan? DepartureTime
        {
            get
            {
                if (Stops.Any())
                {
                    var firstStop = Stops.ElementAt(0);
                    return firstStop.PublicDeparture ?? firstStop.Departure ?? firstStop.Pass ?? default(TimeSpan?);
                }
                return null;
            }
        }

        [IgnoreDataMember]
        public DateTime DateFor { get; set; }
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
