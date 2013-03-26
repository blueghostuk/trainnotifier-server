using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TrainNotifier.Common.Model
{
    [DataContract]
    public class TrainReinstatement : ITrainData
    {
        [DataMember]
        public string TrainId { get; set; }

        [DataMember]
        public DateTime NewDepartureTime { get; set; }

        [DataMember]
        public DateTime? ReinstatementTime { get; set; }

        [DataMember]
        public string Stanox { get; set; }
    }

    public static class TrainReinstatementMapper
    {
        public static TrainReinstatement MapFromBody(dynamic body)
        {
            TrainReinstatement tr = new TrainReinstatement();
            tr.Stanox = (string)body.loc_stanox;
            tr.NewDepartureTime = UnixTsToDateTime((string)body.dep_timestamp).Value;
            tr.ReinstatementTime = UnixTsToDateTime((string)body.reinstatement_timestamp);

            return tr;
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
