using System;
using System.Runtime.Serialization;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model.Api
{
    [DataContract]
    public class Delay
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Headcode { get; set; }
        //[DataMember]
        //public string Uid { get; set; }
        [DataMember]
        public int DelayTime { get; set; }
        [DataMember]
        public string OriginStanox { get; set; }
        [DataMember]
        public string DestStanox { get; set; }
        [DataMember]
        public AtocCode Operator { get; set; }
        [DataMember]
        public StationStop From { get; set; }
        [DataMember]
        public StationStop To { get; set; }
    }

    [DataContract]
    public class StationStop
    {
        [DataMember]
        public string Platform { get; set; }
        [DataMember]
        public DateTime Expected { get; set; }
        [DataMember]
        public DateTime Actual { get; set; }
    }
}
