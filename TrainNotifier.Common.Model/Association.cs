using System;
using System.Runtime.Serialization;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model
{
    [DataContract]
    public class Association
    {
        [IgnoreDataMember]
        public Guid AssociationId { get; set; }
        [IgnoreDataMember]
        public TransactionType TransactionType { get; set; }
        [DataMember]
        public AssociationType AssociationType { get; set; }
        [DataMember]
        public string MainTrainUid { get; set; }
        [DataMember]
        public string AssocTrainUid { get; set; }
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime? EndDate { get; set; }
        [DataMember]
        public AssociationDate DateType { get; set; }
        [DataMember]
        public Schedule.Schedule Schedule { get; set; }
        [DataMember]
        public STPIndicator? STPIndicator { get; set; }
        [IgnoreDataMember]
        public bool Deleted { get; set; }
        [DataMember]
        public TiplocCode Location { get; set; }
    }

    [DataContract]
    public enum AssociationDate : byte
    {
        [EnumMember]
        SameDay = 0,
        [EnumMember]
        PreviousDay = 1,
        [EnumMember]
        NextDay = 2
    }

    [DataContract]
    public enum AssociationType : byte
    {
        [EnumMember]
        NextTrain = 0,
        [EnumMember]
        Join = 1,
        [EnumMember]
        Split = 2
    }
}
