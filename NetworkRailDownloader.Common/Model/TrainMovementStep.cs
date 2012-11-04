using System;
using System.Runtime.Serialization;

namespace TrainNotifier.Common.Model
{
    [DataContract]
    public sealed class TrainMovementStep
    {
        [DataMember]
        public string EventType { get; set; }

        [DataMember]
        public DateTime? PlannedTime { get; set; }

        [DataMember]
        public DateTime ActualTimeStamp { get; set; }

        [DataMember]
        public bool Terminated { get; set; }

        [DataMember]
        public string Stanox { get; set; }

        [DataMember]
        public string Line { get; set; }

        [DataMember]
        public string Platform { get; set; }
    }

    public static class TrainMovementStepMapper
    {
        public static TrainMovementStep MapFromBody(dynamic body)
        {
            DateTime? plannedTime = null;
            if (!string.IsNullOrEmpty((string)body.planned_timestamp))
            {
                plannedTime = UnixTsToDateTime(double.Parse((string)body.planned_timestamp));
            }
            return new TrainMovementStep
            {
                ActualTimeStamp = UnixTsToDateTime(double.Parse((string)body.actual_timestamp)),
                EventType = (string)body.event_type,
                Line = (string)body.line_ind,
                PlannedTime = plannedTime,
                Platform = (string)body.platform,
                Stanox = (string)body.loc_stanox,
                Terminated = ((string)body.train_teminated) == "true"
            };
        }

        private static readonly DateTime _epoch = new DateTime(1970, 1, 1);

        private static DateTime UnixTsToDateTime(double timeStamp)
        {
            return _epoch.AddMilliseconds(timeStamp);
        }
    }
}
