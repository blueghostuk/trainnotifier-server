using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkRailDownloader.Console.Model
{
    public sealed class TrainMovementStep
    {
        public string EventType { get; set; }

        public DateTime? PlannedTime { get; set; }

        public DateTime ActualTimeStamp { get; set; }

        public bool Terminated { get; set; }

        public string Stanox { get; set; }

        public string Line { get; set; }

        public string Platform { get; set; }
    }
}
