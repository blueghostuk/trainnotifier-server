using System;
using System.Collections.Generic;
using System.ServiceModel;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.Schedule;
using TrainNotifier.Common.Model.SmartExtract;

namespace TrainNotifier.Common.Services
{
    [ServiceContract]
    [ServiceKnownType(typeof(TiplocCode))]
    [ServiceKnownType(typeof(TDElement))]
    [ServiceKnownType(typeof(EventType))]
    [ServiceKnownType(typeof(StepType))]
    [ServiceKnownType(typeof(TrainDescriber))]
    [ServiceKnownType(typeof(CaTD))]
    [ServiceKnownType(typeof(CbTD))]
    [ServiceKnownType(typeof(CcTD))]
    [ServiceKnownType(typeof(CtTD))]
    public interface ITDService
    {
        [OperationContract]
        void CacheTrainDescriberData(IEnumerable<TrainDescriber> trainData);

        [OperationContract]
        Tuple<DateTime, string> GetBerthContents(string berth);
    }
}
