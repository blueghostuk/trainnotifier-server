using System;
using System.Runtime.Serialization;

namespace TrainNotifier.Common.Archive
{
    [DataContract]
    public class TrainStopArchive
    {
        [DataMember]
        public string E { get; set; }
        [DataMember]
        public DateTime? P { get; set; }
        [DataMember]
        public DateTime A { get; set; }
        [DataMember]
        public string R { get; set; }
        [DataMember]
        public string Pl { get; set; }
        [DataMember]
        public string L { get; set; }
        [DataMember]
        public bool T { get; set; }
        [DataMember]
        public byte? SSN { get; set; }

        [IgnoreDataMember]
        public string EventType { get { return E; } set { E = value; } }
        [IgnoreDataMember]
        public DateTime? PlannedTimestamp { get { return P; } set { P = value; } }
        [IgnoreDataMember]
        public DateTime ActualTimestamp { get { return A; } set { A = value; } }
        [IgnoreDataMember]
        public string ReportinStanox { get { return R; } set { R = value; } }
        [IgnoreDataMember]
        public string Platform { get { return Pl; } set { Pl = value; } }
        [IgnoreDataMember]
        public string Line { get { return L; } set { L = value; } }
        [IgnoreDataMember]
        public bool TrainTerminated { get { return T; } set { T = value; } }
        [IgnoreDataMember]
        public byte? ScheduleStopNumber { get { return SSN; } set { SSN = value; } }
    }
}
