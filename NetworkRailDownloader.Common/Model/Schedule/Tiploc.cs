using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace TrainNotifier.Common.Model.Schedule
{
    [DataContract]
    public class TiplocCode : IEquatable<TiplocCode>
    {
        [DataMember]
        [JsonIgnore]
        public short TiplocId { get; set; }
        [DataMember]
        public string Tiploc { get; set; }
        [DataMember]
        public string Nalco { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Stanox { get; set; }
        [DataMember]
        public string CRS { get; set; }

        public bool Equals(TiplocCode other)
        {
            return other != null && this.Stanox == other.Stanox;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TiplocCode);
        }

        public override int GetHashCode()
        {
            return
                !string.IsNullOrEmpty(this.Stanox) ? this.Stanox.GetHashCode() :
                !string.IsNullOrEmpty(this.CRS) ? this.CRS.GetHashCode() :
                !string.IsNullOrEmpty(this.Tiploc) ? this.Tiploc.GetHashCode() :
                -1;
        }
    }

    [DataContract]
    public sealed class ScheduleTiploc : StationTiploc
    {
        [DataMember]
        public string Platform { get; set; }

        [DataMember]
        public TimeSpan? Departure { get; set; }

        [DataMember]
        public TimeSpan? PublicDeparture { get; set; }

        [DataMember]
        public TimeSpan? Arrival { get; set; }

        [DataMember]
        public TimeSpan? PublicArrival { get; set; }
    }

    [DataContract]
    public class StationTiploc : TiplocCode
    {
        [DataMember]
        public string StationName { get; set; }
        [DataMember]
        public double? Lat { get; set; }
        [DataMember]
        public double? Lon { get; set; }
    }
}
