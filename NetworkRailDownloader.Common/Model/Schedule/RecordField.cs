using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainNotifier.Common.Model.Schedule
{
    public class IgnoredField
    {
        public byte FieldLength { get; private set; }

        public IgnoredField(byte fieldLength = 1)
        {
            FieldLength = fieldLength;
        }

        public void ParseString(string data)
        { }
    }

    public abstract class RecordField<T>
    {
        public byte FieldLength { get; private set; }

        protected RecordField(byte fieldLength)
        {
            FieldLength = fieldLength;
        }

        public abstract void ParseString(string data);

        public T Value { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public sealed class CharField : RecordField<char>
    {
        public CharField()
            : base(1) { }

        public override void ParseString(string data)
        {
            Value = data[0];
        }
    }

    public class StringField : RecordField<string>
    {
        public static readonly StringField Default = new StringField(0);

        public static string ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        private readonly bool _trimValue;

        public StringField(byte fieldLength, bool trimValue = true)
            : base(fieldLength)
        {
            _trimValue = trimValue;
        }

        public override void ParseString(string data)
        {
            if (string.IsNullOrEmpty(data))
                Value = null;
            else if (_trimValue)
                Value = data.Trim();
            else
                Value = data;
        }
    }

    public class ByteField : RecordField<byte?>
    {
        public static readonly ByteField Default = new ByteField(0);

        public static byte? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        private readonly byte? _defaultValue;

        public ByteField(byte fieldLength, byte? defaultValue = null)
            : base(fieldLength)
        {
            _defaultValue = defaultValue;
        }

        public override void ParseString(string data)
        {
            byte b;
            if (byte.TryParse(data, out b))
                Value = b;
            else
                Value = _defaultValue;
        }
    }


    public class EnumField<T> : RecordField<T>
    {
        private readonly IDictionary<string, T> _converters;
        private readonly bool _trimData;
        private readonly T _defaultValue;

        public EnumField(byte fieldLength, IDictionary<string, T> converters, bool trimData = true, T defaultValue = default(T))
            : base(fieldLength)
        {
            _converters = converters;
            _trimData = trimData;
            _defaultValue = defaultValue;
        }

        public override void ParseString(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                Value = _defaultValue;
                return;
            }

            if (_trimData)
                data = data.Trim();

            T value;
            if (_converters.TryGetValue(data, out value))
                Value = value;
            else
                Value = _defaultValue;
        }
    }

    public abstract class DateTimeBaseField<T> : RecordField<T>
    {
        private readonly string _format;
        private readonly bool _lastCharHalf;

        protected DateTimeBaseField(byte fieldLength = 11, string format = "yyyy-MM-dd", bool lastCharHalf = true)
            : base(fieldLength)
        {
            _format = format;
            _lastCharHalf = lastCharHalf;
        }

        public override void ParseString(string data)
        {
            if (string.IsNullOrEmpty(data) || data.Trim() == Constants.OngoingEndDate)
            {
                HandleInvalidValue();
                return;
            }

            string dataToParse = _lastCharHalf ?
                new string(data.Take(FieldLength - 1).ToArray()) :
                data;

            DateTime dt;
            if (!DateTime.TryParseExact(dataToParse, _format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                HandleInvalidValue();
                return;
            }
            else
            {
                if (_lastCharHalf)
                {
                    if (data.Skip(FieldLength - 1).Take(1).FirstOrDefault().Equals(Constants.HalfMinute))
                    {
                        SetValue(dt.Add(Constants.HalfMinuteAmount));
                        return;
                    }
                }
                SetValue(dt);
                return;
            }
        }

        protected abstract void HandleInvalidValue();

        protected abstract void SetValue(DateTime d);
    }

    public class NullableDateTimeField : DateTimeBaseField<DateTime?>
    {
        public static readonly NullableDateTimeField Default = new NullableDateTimeField();

        public static DateTime? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public NullableDateTimeField(byte fieldLength = 11, string format = "yyyy-MM-dd", bool lastCharHalf = true)
            : base(fieldLength, format, lastCharHalf) { }

        protected override void HandleInvalidValue()
        {
            Value = null;
        }

        protected override void SetValue(DateTime d)
        {
            Value = d;
        }
    }

    public class NullableDateField : NullableDateTimeField
    {
        public static readonly NullableDateField Default = new NullableDateField();

        public static DateTime? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public NullableDateField()
            : base(10, lastCharHalf: false) { }
    }

    public class DateTimeField : DateTimeBaseField<DateTime>
    {
        public static readonly DateTimeField Default = new DateTimeField();

        public static DateTime ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public DateTimeField(byte fieldLength = 11, string format = "yyyy-MM-dd", bool lastCharHalf = true)
            : base(fieldLength, format, lastCharHalf) { }

        protected override void HandleInvalidValue()
        {
            throw new ArgumentNullException();
        }

        protected override void SetValue(DateTime d)
        {
            Value = d;
        }
    }

    public class DateField : DateTimeField
    {
        public static readonly DateField Default = new DateField();

        public static DateTime ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        public DateField()
            : base(10, lastCharHalf: false) { }
    }

    public sealed class TimeSpanField : RecordField<TimeSpan?>
    {
        public static readonly TimeSpanField Default = new TimeSpanField();

        public static TimeSpan? ParseDataString(string data)
        {
            Default.ParseString(data);
            return Default.Value;
        }

        private readonly string _format;
        private readonly bool _lastCharHalf;

        public TimeSpanField(byte fieldLength = 5, string format = "hhmm", bool lastCharHalf = true)
            : base(fieldLength)
        {
            _format = format;
            _lastCharHalf = lastCharHalf;
        }

        public override void ParseString(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                Value = null;
                return;
            }

            string dataToParse = _lastCharHalf ?
                new string(data.Take(FieldLength - 1).ToArray()) :
                data;

            TimeSpan ts;
            if (!TimeSpan.TryParseExact(dataToParse, _format, CultureInfo.InvariantCulture, out ts))
            {
                Value = null;
                return;
            }
            else
            {
                if (_lastCharHalf)
                {
                    if (data.Skip(FieldLength - 1).Take(1).FirstOrDefault().Equals(Constants.HalfMinute))
                    {
                        Value = ts.Add(Constants.HalfMinuteAmount);
                        return;
                    }
                }
                Value = ts;
                return;
            }
        }
    }

    public sealed class DayOfWeekField : RecordField<DayOfWeek?>
    {
        public static readonly DayOfWeekField Monday = new DayOfWeekField(DayOfWeek.Monday);
        public static readonly DayOfWeekField Tuesday = new DayOfWeekField(DayOfWeek.Tuesday);
        public static readonly DayOfWeekField Wednesday = new DayOfWeekField(DayOfWeek.Wednesday);
        public static readonly DayOfWeekField Thursday = new DayOfWeekField(DayOfWeek.Thursday);
        public static readonly DayOfWeekField Friday = new DayOfWeekField(DayOfWeek.Friday);
        public static readonly DayOfWeekField Saturday = new DayOfWeekField(DayOfWeek.Saturday);
        public static readonly DayOfWeekField Sunday = new DayOfWeekField(DayOfWeek.Sunday);

        private readonly DayOfWeek _value;

        public DayOfWeekField(DayOfWeek value)
            : base(1)
        {
            _value = value;
        }

        public DayOfWeek? ParseDataString(string data)
        {
            ParseString(data);
            return Value;
        }

        public override void ParseString(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                Value = null;
            }
            else if (data == "1")
            {
                Value = _value;
            }

            else
            {
                Value = null;
            }
        }
    }

    public sealed class HeadCodeField : StringField
    {
        public HeadCodeField()
            : base(4) { }
    }

    public sealed class SpeedField : ByteField
    {
        public SpeedField()
            : base(3, 0) { }
    }

    public sealed class TrainIdentityField : StringField
    {
        public TrainIdentityField()
            : base(4) { }
    }

    public sealed class CourseIndicatorField : IgnoredField
    {
        public CourseIndicatorField()
            : base(1) { }
    }

    public sealed class TrainServiceCodeField : StringField
    {
        public TrainServiceCodeField()
            : base(8) { }
    }

    public sealed class TimingLoadField : StringField
    {
        public TimingLoadField()
            : base(4) { }
    }

    public sealed class UICCodeField : StringField
    {
        public UICCodeField()
            : base(5) { }
    }

    public sealed class LocationField : StringField
    {
        public LocationField()
            : base(8) { }

        public string Tiploc { get; set; }
        public string Suffix { get; set; }

        public override void ParseString(string data)
        {
            Tiploc = new string(data.Take(7).ToArray()).Trim();
            Suffix = new string(data.Skip(7).Take(1).ToArray()).Trim();

            Value = Tiploc;
        }
    }

    public sealed class PlatformField : StringField
    {
        public PlatformField()
            : base(3) { }
    }

    public sealed class PathField : StringField
    {
        public PathField()
            : base(3) { }
    }

    public sealed class LineField : StringField
    {
        public LineField()
            : base(3) { }
    }

    public sealed class TIPLOCField : StringField
    {
        public TIPLOCField()
            : base(7) { }
    }

    public sealed class NalcoField : StringField
    {
        public NalcoField()
            : base(6) { }
    }

    public sealed class TPSDescriptionField : StringField
    {
        public TPSDescriptionField()
            : base(26) { }
    }

    public sealed class TOPSLocationCodeField : StringField
    {
        public TOPSLocationCodeField()
            : base(5) { }
    }

    public sealed class CRSCodeField : StringField
    {
        public CRSCodeField(bool extraEmptyChar = false)
            : base((byte)(extraEmptyChar ? 4 : 3)) { }
    }

    public sealed class StationNameField : StringField
    {
        public StationNameField()
            : base(30) { }
    }

    public sealed class EastingField : StringField
    {
        public EastingField()
            : base(5) { }
    }

    public sealed class NorthingField : StringField
    {
        public NorthingField()
            : base(5) { }
    }

    public sealed class EstimatedField : RecordField<bool>
    {
        public EstimatedField()
            : base(1) { }

        public override void ParseString(string data)
        {
            Value = data.Equals("E", StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public sealed class StationTableNumberField : StringField
    {
        public StationTableNumberField()
            : base(4) { }
    }

    public sealed class StationGroupNameField : StringField
    {
        public StationGroupNameField()
            : base(30) { }
    }
}
