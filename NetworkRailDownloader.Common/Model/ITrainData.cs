using System.ServiceModel;

namespace TrainNotifier.Common.Model
{
    [ServiceContract]
    [ServiceKnownType(typeof(TrainMovement))]
    [ServiceKnownType(typeof(CancelledTrainMovementStep))]
    [ServiceKnownType(typeof(TrainMovementStep))]
    public interface ITrainData
    {
        string TrainId { get; set; }
    }
}
