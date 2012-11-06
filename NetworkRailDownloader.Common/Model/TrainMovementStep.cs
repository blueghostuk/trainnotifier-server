using System;
using System.Runtime.Serialization;

namespace TrainNotifier.Common.Model
{
    [DataContract]
    public class TrainMovementStep
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

        [DataMember]
        public State State { get; set; }
    }

    [DataContract]
    public class CancelledTrainMovementStep : TrainMovementStep
    {
        [DataMember]
        public DateTime CancelledTime { get; set; }

        [DataMember]
        public string CancelledReasonCode { get; set; }

        [DataMember]
        public string CancelledType { get; set; }
    }

    [DataContract]
    public enum State
    {
        [EnumMember()]
        Normal,
        [EnumMember()]
        Terminated,
        [EnumMember()]
        Cancelled
    }

    public static class TrainMovementStepMapper
    {
        public static TrainMovementStep MapFromBody(dynamic body, bool cancellation = false)
        {
            DateTime? plannedTime = null;
            if (!string.IsNullOrEmpty((string)body.planned_timestamp))
            {
                plannedTime = UnixTsToDateTime(double.Parse((string)body.planned_timestamp));
            }

            TrainMovementStep tm = GetStep(cancellation);
            tm.ActualTimeStamp = UnixTsToDateTime(double.Parse((string)body.actual_timestamp));
            tm.EventType = (string)body.event_type;
            tm.Line = (string)body.line_ind;
            tm.PlannedTime = plannedTime;
            tm.Platform = (string)body.platform;
            tm.Stanox = (string)body.loc_stanox;
            tm.Terminated = ((string)body.train_teminated) == "true";

            if (cancellation)
            {
                ((CancelledTrainMovementStep)tm).CancelledTime = UnixTsToDateTime(double.Parse((string)body.canx_timestamp));
                ((CancelledTrainMovementStep)tm).CancelledReasonCode = body.canx_reason_code;
                ((CancelledTrainMovementStep)tm).CancelledType = body.canx_type;
                tm.State = State.Cancelled;
            }
            else
            {
                tm.State = GetState(body);
            }

            return tm;
        }

        private static TrainMovementStep GetStep(bool cancellation)
        {
            return cancellation ? new CancelledTrainMovementStep() : new TrainMovementStep();
        }

        private static State GetState(dynamic body)
        {
            if (((string)body.train_terminated).Equals("true", StringComparison.InvariantCultureIgnoreCase))
                return State.Terminated;

            return State.Normal;
        }

        private static readonly DateTime _epoch = new DateTime(1970, 1, 1);

        private static DateTime UnixTsToDateTime(double timeStamp)
        {
            return _epoch.AddMilliseconds(timeStamp);
        }
    }
}
