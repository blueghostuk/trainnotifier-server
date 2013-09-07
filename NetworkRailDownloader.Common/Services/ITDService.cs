using System;
using System.Collections.Generic;
using System.ServiceModel;
using TrainNotifier.Common.Model;

namespace TrainNotifier.Common.Services
{
    [ServiceContract]
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
        Tuple<DateTime, string> GetTrainLocation(string trainDescriber);

        [OperationContract]
        Tuple<DateTime, string> GetBerthContents(string berth);
    }
}
