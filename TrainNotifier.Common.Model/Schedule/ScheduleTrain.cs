using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TrainNotifier.Common.Model.Schedule
{
    [DataContract]
    public sealed class ScheduleTrain
    {
        [IgnoreDataMember]
        public Guid ScheduleId { get; set; }
        [DataMember]
        public string TrainUid { get; set; }
        [DataMember]
        public string Headcode { get; set; }
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime? EndDate { get; set; }
        [DataMember]
        public AtocCode AtocCode { get; set; }
        [DataMember]
        public Schedule Schedule { get; set; }
        [DataMember]
        public Status? Status { get; set; }
        [DataMember]
        public STPIndicator STPIndicator { get; set; }
        [DataMember]
        public TiplocCode Origin { get; set; }
        [DataMember]
        public TiplocCode Destination { get; set; }

        [DataMember]
        public PowerType? PowerType { get; set; }
        [DataMember]
        public TrainCategory? TrainCategory { get; set; }
        [DataMember]
        public byte? Speed { get; set; }

        [IgnoreDataMember]
        public TransactionType TransactionType { get; set; }

        [DataMember]
        public IEnumerable<ScheduleStop> Stops { get; set; }
    }

    [DataContract]
    public enum ScheduleSource : byte
    {
        [EnumMember]
        CIF = 0,
        [EnumMember]
        VSTP = 1
    }

}
