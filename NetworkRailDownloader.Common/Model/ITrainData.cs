using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

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
