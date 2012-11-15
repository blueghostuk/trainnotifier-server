using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace TrainNotifier.Common.Model
{
    [DataContract]
    [ServiceKnownType(typeof(CaTD))]
    [ServiceKnownType(typeof(CbTD))]
    [ServiceKnownType(typeof(CcTD))]
    [ServiceKnownType(typeof(CtTD))]
    public abstract class TrainDescriber
    {
        [DataMember]
        public DateTime Time { get; set; }
        [DataMember]
        public string AreaId { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Type { get; set; }
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
            body = GetBody(body);

            if (body != null)
            {
                try
                {
                    DateTime time = UnixTsToDateTime(double.Parse((string)body.time));
                    string msg_type = (string)body.msg_type;

                    switch (msg_type)
                    {
                        case "CA":
                            return new CaTD
                            {
                                Time = time,
                                AreaId = body.area_id,
                                Description = body.descr,
                                To = body.to,
                                From = body.from,
                                Type = msg_type
                            };
                        case "CB":
                            return new CbTD
                            {
                                Time = time,
                                AreaId = body.area_id,
                                Description = body.descr,
                                From = body.from,
                                Type = msg_type
                            };
                        case "CC":
                            return new CcTD
                            {
                                Time = time,
                                AreaId = body.area_id,
                                Description = body.descr,
                                To = body.to,
                                Type = msg_type
                            };
                        case "CT":
                            return new CtTD
                            {
                                Time = time,
                                AreaId = body.area_id,
                                ReportTime = body.report_time,
                                Type = msg_type
                            };
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError("Error constructing TrainDescriber: {0}", e);
                }
            }
            return null;
        }

        private static dynamic GetBody(dynamic body)
        {
            try
            {
                return body.CA_MSG;
            }
            catch { }
            try
            {
                return body.CB_MSG;
            }
            catch { }
            try
            {
                return body.CC_MSG;
            }
            catch { }
            try
            {
                return body.CT_MSG;
            }
            catch { }
            return null;
        }

        private static readonly DateTime _epoch = new DateTime(1970, 1, 1);

        private static DateTime UnixTsToDateTime(double timeStamp)
        {
            return _epoch.AddMilliseconds(timeStamp);
        }
    }
}
