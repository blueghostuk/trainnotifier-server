using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model
{
    [DataContract]
    public class TrainActivation : ITrainData
    {
        private ICollection<TrainMovementStep> _steps;

        [IgnoreDataMember]
        public Guid UniqueId { get; set; }

        [DataMember]
        public DateTime? Activated { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string TrainId { get { return Id; } set { Id = value; } }

        [DataMember]
        public string HeadCode { get; set; }

        [DataMember]
        public string ServiceCode { get; set; }
        [DataMember]
        public string ScheduleOriginStanox { get; set; }
        [DataMember]
        public DateTime ScheduleOriginDeparture { get; set; }
        [DataMember]
        public DateTime ScheduleStartDate { get; set; }
        [DataMember]
        public DateTime ScheduleEndDate { get; set; }
        [DataMember]
        public string ScheduleSource { get; set; }
        [DataMember]
        public string ScheduleType { get; set; }
        [DataMember]
        public string TocId { get; set; }
        [DataMember]
        public string WorkingTTId { get; set; }
        [DataMember]
        public string TrainUid { get; set; }
    }

    public static class TrainActivationMapper
    {
        public static TrainActivation MapFromJson(dynamic body)
        {
            string wttId = null;
            try
            {
                wttId = (string)body.schedule_wtt_id;
            }
            catch { }
            string uid = null;
            try
            {
                uid = ((string)body.train_uid).Trim();
            }
            catch { }

            return new TrainActivation
            {
                Activated = UnixTsToDateTime(double.Parse((string)body.creation_timestamp)),
                Id = (string)body.train_id,
                ScheduleOriginDeparture = UnixTsToDateTime(double.Parse((string)body.origin_dep_timestamp)),
                ScheduleStartDate = (DateTime)body.schedule_start_date,
                ScheduleEndDate = (DateTime)body.schedule_end_date,
                ScheduleOriginStanox = (string)body.sched_origin_stanox,
                ScheduleSource = (string)body.schedule_source,
                ScheduleType = (string)body.schedule_type,
                ServiceCode = (string)body.train_service_code,
                TocId = (string)body.toc_id,
                WorkingTTId = wttId,
                TrainUid = uid
            };
        }

        private static readonly DateTime _epoch = new DateTime(1970, 1, 1);

        private static DateTime UnixTsToDateTime(double timeStamp)
        {
            return _epoch.AddMilliseconds(timeStamp);
        }
    }
}
