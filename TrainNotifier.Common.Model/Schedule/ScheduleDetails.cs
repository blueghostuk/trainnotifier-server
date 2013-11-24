using System;

namespace TrainNotifier.Common.Model.Schedule
{
    public class ScheduleDetails
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Deleted { get; set; }
        public bool RunsMonday { get; set; }
        public bool RunsTuesday { get; set; }
        public bool RunsWednesday { get; set; }
        public bool RunsThursday { get; set; }
        public bool RunsFriday { get; set; }
        public bool RunsSaturday { get; set; }
        public bool RunsSunday { get; set; }
    }
}
