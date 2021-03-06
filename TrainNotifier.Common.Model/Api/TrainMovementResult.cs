﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model.Api
{
    [DataContract]
    public class TrainMovementResult : IEquatable<TrainMovementResult>
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

        public bool Equals(TrainMovementResult other)
        {
            return other != null && 
                other.Schedule.TrainUid == this.Schedule.TrainUid;
        }

        public override int GetHashCode()
        {
            return Schedule.TrainUid.GetHashCode();
        }
    }

    [DataContract]
    public class TrainMovementResults
    {
        [DataMember]
        public IEnumerable<TrainMovementResult> Movements { get; set; }

        [DataMember]
        public IEnumerable<StationTiploc> Tiplocs { get; set; }
    }

    [DataContract]
    public class SingleTrainMovementResult
    {
        [DataMember]
        public TrainMovementResult Movement { get; set; }

        [DataMember]
        public IEnumerable<StationTiploc> Tiplocs { get; set; }
    }
}
