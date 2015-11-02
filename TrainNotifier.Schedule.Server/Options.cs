using CommandLine;
using System;
using TrainNotifier.Common.Model;

namespace TrainNotifier.Schedule.Server
{
    class Options
    {
        [Option("t", "toc", DefaultValue = Toc.All)]
        public Toc Toc { get; set; }

        [Option("d", "day", DefaultValue = null)]
        public DayOfWeek? Day { get; set; }

        [Option("s", "schedule", DefaultValue = ScheduleType.DailyUpdate)]
        public ScheduleType ScheduleType { get; set; }

        [Option("f", "force", DefaultValue = false)]
        public bool ForceDownload { get; set; }

        [Option("i", "ignoredate", DefaultValue = false)]
        public bool IgnoreWrongDate { get; set; }

        [Option("n", "delete", DefaultValue = true)]
        public bool Delete { get; set; }
    }
}
