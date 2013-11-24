using System;
using System.Runtime.Serialization;

namespace TrainNotifier.Common.Model.PPM
{
    [DataContract]
    public class PPMSector
    {
        [IgnoreDataMember]
        public Guid PPMSectorId { get; set; }

        [DataMember]
        public string OperatorCode { get; set; }

        [DataMember]
        public string SectorCode { get; set; }

        [DataMember]
        public string Description { get; set; }
    }
}
