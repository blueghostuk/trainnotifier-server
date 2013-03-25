﻿using System.ServiceModel;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model
{
    [ServiceContract]
    [ServiceKnownType(typeof(TrainMovement))]
    [ServiceKnownType(typeof(ExtendedTrainMovement))]
    [ServiceKnownType(typeof(CancelledTrainMovementStep))]
    [ServiceKnownType(typeof(TrainMovementStep))]
    [ServiceKnownType(typeof(TrainChangeOfOrigin))]
    public interface ITrainData
    {
        string TrainId { get; set; }
    }
}
