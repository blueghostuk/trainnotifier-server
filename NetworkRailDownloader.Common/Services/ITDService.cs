using System;
using System.Collections.Generic;
using System.ServiceModel;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.Schedule;
using TrainNotifier.Common.Model.SmartExtract;
using TrainNotifier.Common.Model.TDCache;

namespace TrainNotifier.Common.Services
{
    /// <summary>
    /// TD Data Service Interface
    /// </summary>
    [ServiceContract]
    [ServiceKnownType(typeof(TiplocCode))]
    [ServiceKnownType(typeof(TDElement))]
    [ServiceKnownType(typeof(EventType))]
    [ServiceKnownType(typeof(StepType))]
    [ServiceKnownType(typeof(TrainDescriber))]
    [ServiceKnownType(typeof(CachedTrainDetails))]
    [ServiceKnownType(typeof(CaTD))]
    [ServiceKnownType(typeof(CbTD))]
    [ServiceKnownType(typeof(CcTD))]
    [ServiceKnownType(typeof(CtTD))]
    public interface ITDService
    {
        /// <summary>
        /// Cache TD data
        /// </summary>
        /// <param name="trainData">TD data</param>
        [OperationContract]
        void CacheTrainDescriberData(IEnumerable<TrainDescriber> trainData);

        /// <summary>
        /// Get the current contents of a berth location
        /// </summary>
        /// <param name="berth">berth to request data for</param>
        /// <returns>the contents (if any) with the last date updated, the id of what is in the berth and the train details (if any)</returns>
        [OperationContract]
        Tuple<DateTime, string, CachedTrainDetails> GetBerthContents(string berth);
    }
}
