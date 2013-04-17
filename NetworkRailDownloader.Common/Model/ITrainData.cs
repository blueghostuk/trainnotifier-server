using System.ServiceModel;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model
{
    [ServiceContract]
    [ServiceKnownType(typeof(TrainMovement))]
    [ServiceKnownType(typeof(ViewModelTrainMovement))]
    [ServiceKnownType(typeof(CancelledTrainMovementStep))]
    [ServiceKnownType(typeof(TrainMovementStep))]
    [ServiceKnownType(typeof(TrainChangeOfOrigin))]
    [ServiceKnownType(typeof(TrainReinstatement))]
    public interface ITrainData
    {
        string TrainId { get; set; }
    }
}
