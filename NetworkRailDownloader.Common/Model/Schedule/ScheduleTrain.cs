using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TrainNotifier.Common.Model.Schedule
{
    [DataContract]
    public sealed class ScheduleTrain
    {
        [DataMember]
        public Guid ScheduleId { get; set; }
        [DataMember]
        public string TrainUid { get; set; }
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime? EndDate { get; set; }
        [DataMember]
        public bool Active { get; set; }
        [DataMember]
        public AtocCode AtocCode { get; set; }
        [DataMember]
        public Schedule Schedule { get; set; }
        [DataMember]
        public ScheduleStatus Status { get; set; }
        [DataMember]
        public TiplocCode Origin { get; set; }
        [DataMember]
        public TiplocCode Destination { get; set; }

        [DataMember]
        public IEnumerable<ScheduleStop> Stops { get; set; }
    }

    [DataContract]
    public enum ScheduleStatus
    {
        [EnumMember()]
        Permanent = 1,
        [EnumMember()]
        STP = 2,
        [EnumMember()]
        Overlay = 3,
        [EnumMember()]
        Cancellation = 4
    }

    [DataContract]
    public sealed class Schedule
    {
        [DataMember]
        public bool Monday { get; set; }
        [DataMember]
        public bool Tuesday { get; set; }
        [DataMember]
        public bool Wednesday { get; set; }
        [DataMember]
        public bool Thursday { get; set; }
        [DataMember]
        public bool Friday { get; set; }
        [DataMember]
        public bool Saturday { get; set; }
        [DataMember]
        public bool Sunday { get; set; }
        [DataMember]
        public bool BankHoliday { get; set; }

        public bool RunsOnDay(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday:
                    return Monday;
                case DayOfWeek.Tuesday:
                    return Tuesday;
                case DayOfWeek.Wednesday:
                    return Wednesday;
                case DayOfWeek.Thursday:
                    return Thursday;
                case DayOfWeek.Friday:
                    return Friday;
                case DayOfWeek.Saturday:
                    return Saturday;
                case DayOfWeek.Sunday:
                    return Sunday;
                default:
                    return false;
            }
        }

    }
}
