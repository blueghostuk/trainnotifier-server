using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TrainNotifier.Common.Model.SmartExtract
{
    public class TDContainer
    {
        public IEnumerable<TDElement> BERTHDATA { get; set; }
    }

    public class TDElementEqualityComparer : IEqualityComparer<TDElement>
    {
        public bool Equals(TDElement x, TDElement y)
        {
            return 
                x.TD.Equals(y.TD, StringComparison.CurrentCultureIgnoreCase) && 
                x.TOBERTH.Equals(y.TOBERTH, StringComparison.CurrentCultureIgnoreCase);
        }

        public int GetHashCode(TDElement obj)
        {
            return obj.TD.GetHashCode() ^ obj.TOBERTH.GetHashCode();
        }
    }

    [DataContract]
    public class TDElement
    {
        public override string ToString()
        {
            return TD.PadRight(4) +
                FROMBERTH.PadRight(6) +
                TOBERTH.PadRight(6) +
                FROMLINE.PadRight(4) +
                TOLINE.PadRight(4) +
                STANME.PadRight(10) +
                PLATFORM.PadRight(4) +
                EventType.ToString().PadRight(12) +
                StepType.ToString().PadRight(10) + 
                BERTHOFFSET.ToString().PadRight(6);
        }

        [DataMember]
        public string TD { get; set; }
        [DataMember]
        public string FROMBERTH { get; set; }
        [DataMember]
        public string TOBERTH { get; set; }
        [DataMember]
        public string FROMLINE { get; set; }
        [DataMember]
        public string TOLINE { get; set; }
        [DataMember]
        public string BERTHOFFSET { get; set; }
        [DataMember]
        public string PLATFORM { get; set; }
        [DataMember]
        public string EVENT { get; set; }
        [DataMember]
        public string ROUTE { get; set; }
        [DataMember]
        public string STANOX { get; set; }
        [DataMember]
        public string STANME { get; set; }
        [DataMember]
        public string STEPTYPE { get; set; }
        [DataMember]
        public string COMMENT { get; set; }

        [DataMember]
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
            set
            {
                switch (value)
                {
                    case SmartExtract.EventType.ArriveUp:
                        EVENT = "A";
                        break;
                    case SmartExtract.EventType.DepartUp:
                        EVENT = "B";
                        break;
                    case SmartExtract.EventType.ArriveDown:
                        EVENT = "C";
                        break;
                    case SmartExtract.EventType.DepartDown:
                        EVENT = "D";
                        break;
                    default:
                        EVENT = "";
                        break;
                }
            }
        }

        [DataMember]
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
            set
            {
                switch (value)
                {
                    case SmartExtract.StepType.Between:
                        STEPTYPE = "B";
                        break;
                    case SmartExtract.StepType.From:
                        STEPTYPE = "F";
                        break;
                    case SmartExtract.StepType.To:
                        STEPTYPE = "T";
                        break;
                    case SmartExtract.StepType.IntermediateFirst:
                        STEPTYPE = "D";
                        break;
                    case SmartExtract.StepType.Clearout:
                        STEPTYPE = "C";
                        break;
                    case SmartExtract.StepType.Interpose:
                        STEPTYPE = "I";
                        break;
                    case SmartExtract.StepType.Intermediate:
                        STEPTYPE = "E";
                        break;
                    default:
                        STEPTYPE = "";
                        break;
                }
            }
        }
    }

    [DataContract]
    public enum EventType
    {
        [EnumMember]
        Unknown,
        [EnumMember]
        ArriveUp,
        [EnumMember]
        DepartUp,
        [EnumMember]
        ArriveDown,
        [EnumMember]
        DepartDown
    }

    [DataContract]
    public enum StepType
    {
        [EnumMember]
        Unknown,
        [EnumMember]
        Between,
        [EnumMember]
        From,
        [EnumMember]
        To,
        [EnumMember]
        IntermediateFirst,
        [EnumMember]
        Clearout,
        [EnumMember]
        Interpose,
        [EnumMember]
        Intermediate
    }
}
