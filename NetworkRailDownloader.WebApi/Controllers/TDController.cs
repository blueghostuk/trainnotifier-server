using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Web.Http;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.TDCache;
using TrainNotifier.Common.Services;
using System.Linq;

namespace TrainNotifier.Console.WebApi.Controllers
{
    public class TDController : ApiController
    {
        private sealed class TDCacheServiceClient : ClientBase<ITDService>, ITDService
        {
            public TDCacheServiceClient() : base("NetTcpBinding_ITDService") { }

            public void CacheTrainDescriberData(IEnumerable<TrainDescriber> trainData)
            {
                throw new NotImplementedException();
            }

            public TDTrain GetTrain(string trainDescriber)
            {
                return base.Channel.GetTrain(trainDescriber);
            }

            public Tuple<DateTime, string> GetBerthContents(string berth)
            {
                return base.Channel.GetBerthContents(berth);
            }
        }

        [HttpGet]
        public TDTrain GetTrain(string describer)
        {
            TDCacheServiceClient cacheService = null;
            try
            {
                cacheService = new TDCacheServiceClient();
                cacheService.Open();
                return cacheService.GetTrain(describer);
            }
            catch (Exception e)
            {
                Trace.TraceError("Error In Cache Connection: {0}", e);
            }
            finally
            {
                try
                {
                    if (cacheService != null)
                        cacheService.Close();
                }
                catch (CommunicationObjectFaultedException e)
                {
                    Trace.TraceError("Error Closing Cache Connection: {0}", e);
                }
            }
            return null;
        }

        [HttpGet]
        public Tuple<DateTime, string> GetBerthDescription(string berth)
        {
            TDCacheServiceClient cacheService = null;
            try
            {
                cacheService = new TDCacheServiceClient();
                cacheService.Open();
                return cacheService.GetBerthContents(berth);
            }
            catch (Exception e)
            {
                Trace.TraceError("Error In Cache Connection: {0}", e);
            }
            finally
            {
                try
                {
                    if (cacheService != null)
                        cacheService.Close();
                }
                catch (CommunicationObjectFaultedException e)
                {
                    Trace.TraceError("Error Closing Cache Connection: {0}", e);
                }
            }
            return null;
        }
    }
}
