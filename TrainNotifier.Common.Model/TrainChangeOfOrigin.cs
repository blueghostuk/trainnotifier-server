using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TrainNotifier.Common.Model
{
    [DataContract]
    public class TrainChangeOfOrigin : ITrainData
    {
        [DataMember]
        public string TrainId { get; set; }

        [DataMember]
        public DateTime NewDepartureTime { get; set; }

        [DataMember]
        public DateTime? ChangedTime { get; set; }

        [DataMember]
        public string ReasonCode { get; set; }

        [DataMember]
        public string Stanox { get; set; }
    }

    public static class TrainChangeOfOriginMapper
    {
        public static TrainChangeOfOrigin MapFromBody(dynamic body)
        {
            TrainChangeOfOrigin change = new TrainChangeOfOrigin();
            change.Stanox = (string)body.loc_stanox;
            change.NewDepartureTime = UnixTsToDateTime((string)body.dep_timestamp).Value;
            change.ChangedTime = UnixTsToDateTime((string)body.coo_timestamp);
            change.ReasonCode = (string)body.reason_code;

            return change;
        }

        private static readonly DateTime _epoch = new DateTime(1970, 1, 1);

        private static DateTime? UnixTsToDateTime(string timeStamp)
        {
            if (string.IsNullOrEmpty(timeStamp))
                return null;
            double ts = double.Parse(timeStamp);
            return _epoch.AddMilliseconds(ts);
        }
    }
}
