using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TrainNotifier.Common.Model.PPM
{
    [DataContract]
    public class RtppmData
    {
        [DataMember]
        public DateTime Timestamp { get; set; }

        [DataMember]
        public List<PPMRecord> Sectors { get; set; }

        [DataMember]
        public List<PPMRecord> Operators { get; set; }

        [DataMember]
        public PPMRecord NationalPPM { get; set; }

        public RtppmData()
        {
            Sectors = new List<PPMRecord>();
            Operators = new List<PPMRecord>();
        }
    }

    [DataContract]
    public class PPMRecord
    {
        public PPMRecord()
            : this(false)
        {

        }

        public PPMRecord(bool isServiceGroup)
        {
            ServiceGroups = isServiceGroup ? null : new List<PPMRecord>();
        }

        [IgnoreDataMember]
        public Guid PPMSectorId { get; set; }

        [DataMember]
        public DateTime Timestamp { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public short Total { get; set; }

        [DataMember]
        public short OnTime { get; set; }

        [DataMember]
        public short Late { get; set; }

        [DataMember]
        public short CancelVeryLate { get; set; }

        [DataMember]
        public PpmTrendIndicator Trend { get; set; }

        [DataMember]
        public List<PPMRecord> ServiceGroups { get; set; }
    }

    [DataContract]
    public enum PpmTrendIndicator : byte
    {
        [EnumMember]
        Negative = 0,
        [EnumMember]
        Equal = 1,
        [EnumMember]
        Positive = 2
    }

}
