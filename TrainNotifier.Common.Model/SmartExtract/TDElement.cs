using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

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
    public class TDElement : IEquatable<TrainDescriber>
    {
        public override string ToString()
        {
            return TD.PadRight(4) +
                FromBerth.PadRight(6) +
                ToBerth.PadRight(6) +
                FROMLINE.PadRight(4) +
                TOLINE.PadRight(4) +
                STANME.PadRight(10) +
                PLATFORM.PadRight(4) +
                EventType.ToString().PadRight(12) +
                StepType.ToString().PadRight(10) + 
                BERTHOFFSET.ToString().PadRight(6);
        }

        public TDElement()
        {
            ROUTE = string.Empty;
            TD = string.Empty;
            FROMBERTH = string.Empty;
            TOBERTH = string.Empty;
            FROMLINE = string.Empty;
            TOLINE = string.Empty;
            BERTHOFFSET = "0";
            PLATFORM = string.Empty;
            EVENT = string.Empty;
            ROUTE = string.Empty;
            STANOX = string.Empty;
            STANME = string.Empty;
            STEPTYPE = string.Empty;
            COMMENT = string.Empty;
        }

        [DataMember]
        public string TD { get; set; }
        [DataMember]
        public string FROMBERTH { get; set; }

        [JsonIgnore]
        [DataMember]
        public string FromBerth
        {
            get
            {
                return FROMBERTH.PadLeft(4, '0');
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    FROMBERTH = value.PadLeft(4, '0');
            }
        }

        [DataMember]
        public string TOBERTH { get; set; }

        [JsonIgnore]
        [DataMember]
        public string ToBerth
        {
            get
            {
                return TOBERTH.PadLeft(4, '0');
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    TOBERTH = value.PadLeft(4, '0');
            }
        }
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

        [JsonIgnore]
        [IgnoreDataMember]
        public Platform Platform
        {
            get
            {
                try
                {
                    uint number = uint.Parse(new string(this.PLATFORM.ToCharArray().TakeWhile(c => char.IsDigit(c)).ToArray()));
                    string section = new string(this.PLATFORM.ToCharArray().SkipWhile(c => char.IsDigit(c)).ToArray());

                    return new Platform
                    {
                        Number = number,
                        Section = section
                    };
                }
                catch
                {
                    return Platform.Empty;
                }
            }
        }

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

        private bool FromEquals(IFrom otherFrom)
        {
            return otherFrom != null &&
                otherFrom.From != null &&
                FromEquals(otherFrom.From);
        }

        private bool FromEquals(string from)
        {
            return from.PadLeft(4, '0').Equals(this.FromBerth, StringComparison.CurrentCultureIgnoreCase);
        }

        private bool ToEquals(ITo otherTo)
        {
            return otherTo != null &&
                otherTo.To != null &&
                ToEquals(otherTo.To);
        }

        private bool ToEquals(string to)
        {
            return to.PadLeft(4, '0').Equals(this.ToBerth, StringComparison.CurrentCultureIgnoreCase);
        }

        public bool Equals(TrainDescriber other)
        {
            if (!other.AreaId.Equals(this.TD, StringComparison.CurrentCultureIgnoreCase))
                return false;

            if (other is IFrom)
            {
                if (!FromEquals((IFrom)other))
                {
                    return false;
                }
            }
            else
            {
                if (!FromEquals(string.Empty))
                {
                    return false;
                }
            }

            if (other is ITo)
            {
                if (!ToEquals((ITo)other))
                {
                    return false;
                }
            }
            else
            {
                if (!ToEquals(string.Empty))
                {
                    return false;
                }
            }

            return true;
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

    public static class EventTypeHelper
    {
        public static string ToDisplayString(this EventType et)
        {
            switch (et)
            {
                case EventType.ArriveDown:
                case EventType.ArriveUp:
                    return "Arrival";
                case EventType.DepartDown:
                case EventType.DepartUp:
                    return "Departure";
                default:
                    return string.Empty;
            }
        }
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

    public class Platform
    {
        public static readonly Platform Empty = new Platform();

        public uint Number { get; set; }

        public string Section { get; set; }
    }

    public class PlatformComparer : IComparer<Platform>
    {
        public int Compare(Platform x, Platform y)
        {
            if (x.Number == y.Number)
                return x.Section.CompareTo(y.Section);
            return x.Number.CompareTo(y.Number);
        }
    }
}
