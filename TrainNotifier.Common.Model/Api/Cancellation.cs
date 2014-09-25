using System;
using System.Runtime.Serialization;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model.Api
{
    [DataContract]
    public class Cancellation
    {
        [DataMember]
        public string Headcode { get; set; }
        [DataMember]
        public string Uid { get; set; }
        [DataMember]
        public string OriginStanox { get; set; }
        [DataMember]
        public string DestStanox { get; set; }
        [DataMember]
        public AtocCode Operator { get; set; }
        [DataMember]
        public TimeSpan FromExpected { get; set; }
        [DataMember]
        public TimeSpan ToExpected { get; set; }
    }
}
