using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainNotifier.Console.SmartExtract
{
    public class TDContainer
    {
        public IEnumerable<TDElement> BERTHDATA { get; set; }
    }

    public class TDElement
    {
        public override string ToString()
        {
            return TD.PadRight(4) +
                FROMBERTH.PadRight(6) +
                TOBERTH.PadRight(6) +
                FROMLINE.PadRight(4) +
                TOLINE.PadRight(4) +
                STANOX.PadRight(8) +
                PLATFORM.PadRight(4) +
                EventType.ToString().PadRight(12) +
                StepType.ToString().PadRight(18);
        }

        public string TD { get; set; }
        public string FROMBERTH { get; set; }
        public string TOBERTH { get; set; }
        public string FROMLINE { get; set; }
        public string TOLINE { get; set; }
        public string BERTHOFFSET { get; set; }
        public string PLATFORM { get; set; }
        public string EVENT { get; set; }
        public string ROUTE { get; set; }
        public string STANOX { get; set; }
        public string STANME { get; set; }
        public string STEPTYPE { get; set; }
        public string COMMENT { get; set; }

        [JsonIgnore]
        public EventType EventType
        {
            get
            {
                switch (EVENT)
                {
                    case "A":
                        return SmartExtract.EventType.ArriveUp;
                    case "B":
                        return SmartExtract.EventType.DepartUp;
                    case "C":
                        return SmartExtract.EventType.ArriveDown;
                    case "D":
                        return SmartExtract.EventType.DepartDown;
                }
                return SmartExtract.EventType.Unknown;
            }
        }

        [JsonIgnore]
        public StepType StepType
        {
            get
            {
                switch (STEPTYPE)
                {
                    case "B":
                        return SmartExtract.StepType.Between;
                    case "F":
                        return SmartExtract.StepType.From;
                    case "T":
                        return SmartExtract.StepType.To;
                    case "D":
                        return SmartExtract.StepType.IntermediateFirst;
                    case "C":
                        return SmartExtract.StepType.Clearout;
                    case "I":
                        return SmartExtract.StepType.Interpose;
                    case "E":
                        return SmartExtract.StepType.Intermediate;
                }
                return SmartExtract.StepType.Unknown;
            }
        }
    }

    public enum EventType
    {
        Unknown,
        ArriveUp,
        DepartUp,
        ArriveDown,
        DepartDown
    }

    public enum StepType
    {
        Unknown,
        Between,
        From,
        To,
        IntermediateFirst,
        Clearout,
        Interpose,
        Intermediate
    }
}
