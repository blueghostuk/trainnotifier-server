using System;
using System.Runtime.Serialization;

namespace TrainNotifier.Common.Model.Schedule
{
    [DataContract]
    public class TiplocCode
    {
        [IgnoreDataMember]
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
    }

    [DataContract]
    public sealed class ScheduleTiploc : TiplocCode
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
}
