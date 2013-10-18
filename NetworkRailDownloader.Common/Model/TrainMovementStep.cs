using System;
using System.Runtime.Serialization;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model
{
    [DataContract]
    public class TrainMovementStep : ITrainData
    {
        [IgnoreDataMember]
        public Guid? DatabaseId { get; set; }

        [DataMember]
        public string TrainId { get; set; }

        [DataMember]
        public string EventType { get; set; }

        [DataMember]
        public string VariationStatus { get; set; }

        [DataMember]
        public DateTime? PlannedTime { get; set; }

        [DataMember]
        public DateTime ActualTimeStamp { get; set; }

        [DataMember]
        public string Stanox { get; set; }

        [DataMember]
        public string Line { get; set; }

        [DataMember]
        public string Platform { get; set; }

        [DataMember]
        public State State { get; set; }

        [IgnoreDataMember]
        public bool Terminated { get; set; }

        [DataMember]
        public byte? ScheduleStopNumber { get; set; }

        [DataMember]
        public bool OffRoute { get; set; }

        [DataMember]
        public string NextStanox { get; set; }

        [DataMember]
        public TimeSpan? ExpectedAtNextStanox { get; set; }
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
            TrainMovementStep tm = GetStep(cancellation);
            tm.Stanox = (string)body.loc_stanox;

            if (cancellation)
            {
                ((CancelledTrainMovementStep)tm).CancelledTime = UnixTsToDateTime(double.Parse((string)body.canx_timestamp));
                ((CancelledTrainMovementStep)tm).CancelledReasonCode = body.canx_reason_code;
                ((CancelledTrainMovementStep)tm).CancelledType = body.canx_type;
                tm.ActualTimeStamp = DateTime.UtcNow;
                tm.State = State.Cancelled;
            }
            else
            {
                DateTime? plannedTime = null;
                if (!string.IsNullOrEmpty((string)body.planned_timestamp))
                {
                    plannedTime = UnixTsToDateTime(double.Parse((string)body.planned_timestamp));
                }
                tm.PlannedTime = plannedTime;

                tm.EventType = (string)body.event_type;
                tm.VariationStatus = (string)body.variation_status;
                tm.Line = (string)body.line_ind;
                tm.ActualTimeStamp = UnixTsToDateTime(double.Parse((string)body.actual_timestamp));
                tm.Platform = (string)body.platform;
                tm.State = GetState(body);
                tm.OffRoute = ((string)body.offroute_ind).Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
                tm.NextStanox = (string)body.next_report_stanox;

                string nextReportTime = (string)body.next_report_run_time;
                if (!string.IsNullOrWhiteSpace(nextReportTime))
                {
                    tm.ExpectedAtNextStanox = TimeSpan.FromMinutes(double.Parse(nextReportTime));
                }
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
