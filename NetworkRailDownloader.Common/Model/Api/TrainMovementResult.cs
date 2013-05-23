using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TrainNotifier.Common.Model.Api
{
    [DataContract]
    public class TrainMovementResult
    {
        [DataMember]
        public RunningTrainActual Actual { get; set; }
        [DataMember]
        public RunningScheduleTrain Schedule { get; set; }

        [DataMember]
        public IEnumerable<ExtendedCancellation> Cancellations { get; set; }
        [DataMember]
        public IEnumerable<Reinstatement> Reinstatements { get; set; }
        [DataMember]
        public IEnumerable<ChangeOfOrigin> ChangeOfOrigins { get; set; }
    }
}
