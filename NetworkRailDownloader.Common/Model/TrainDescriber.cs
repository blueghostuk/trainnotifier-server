using System;
using System.Runtime.Serialization;

namespace TrainNotifier.Common.Model
{
    [DataContract]
    public abstract class TrainDescriber
    {
        [DataMember]
        public DateTime Time { get; set; }
        [DataMember]
        public string AreaId { get; set; }
        [DataMember]
        public string Description { get; set; }
    }

    [DataContract]
    public sealed class CaTD : TrainDescriber
    {
        [DataMember]
        public string From { get; set; }
        [DataMember]
        public string To { get; set; }
    }

    [DataContract]
    public sealed class CbTD : TrainDescriber
    {
        [DataMember]
        public string From { get; set; }
    }

    [DataContract]
    public sealed class CcTD : TrainDescriber
    {
        [DataMember]
        public string To { get; set; }
    }

    [DataContract]
    public sealed class CtTD : TrainDescriber
    {
        [DataMember]
        public string ReportTime { get; set; }
    }

    public static class TrainDescriberMapper
    {
        public static TrainDescriber MapFromBody(dynamic body)
        {
            DateTime time = UnixTsToDateTime(double.Parse((string)body.time));
            string areaId = (string)body.area_id;
            string msg_type = (string)body.msg_type;

            switch (msg_type)
            {
                case "CA":
                    return new CaTD
                    {
                        Time = time,
                        AreaId = areaId,
                        Description = body.descr,
                        To = body.to,
                        From = body.from
                    };
                case "CB":
                    return new CbTD
                    {
                        Time = time,
                        AreaId = areaId,
                        Description = body.descr,
                        From = body.from
                    };
                case "CC":
                    return new CcTD
                    {
                        Time = time,
                        AreaId = areaId,
                        Description = body.descr,
                        To = body.to
                    };
                case "CT":
                    return new CtTD
                    {
                        Time = time,
                        AreaId = areaId,
                        ReportTime = body.report_time
                    };
                default:
                    return null;
            }
        }

        private static readonly DateTime _epoch = new DateTime(1970, 1, 1);

        private static DateTime UnixTsToDateTime(double timeStamp)
        {
            return _epoch.AddMilliseconds(timeStamp);
        }
    }
}
