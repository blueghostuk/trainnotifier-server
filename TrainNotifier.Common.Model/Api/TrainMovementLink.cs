using System;
using System.Runtime.Serialization;

namespace TrainNotifier.Common.Model.Api
{
    [DataContract]
    public class TrainMovementLink
    {
        [DataMember]
        public string TrainUid { get; set; }
        [DataMember]
        public DateTime OriginDepartTimestamp { get; set; }
    }
}
