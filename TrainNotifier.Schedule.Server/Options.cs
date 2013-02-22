using CommandLine;
using System;

namespace TrainNotifier.Schedule.Server
{
    class Options
    {
        [Option("t", "toc", DefaultValue = Common.Toc.All)]
        public Common.Toc Toc { get; set; }

        [Option("d", "day", DefaultValue = null)]
        public DayOfWeek? Day { get; set; }

        [Option("s", "schedule", DefaultValue = Common.ScheduleType.DailyUpdate)]
        public Common.ScheduleType ScheduleType { get; set; }

        [Option("f", "force", DefaultValue = false)]
        public bool Force { get; set; }

        [Option("n", "delete", DefaultValue = false)]
        public bool Delete { get; set; }
    }
}
