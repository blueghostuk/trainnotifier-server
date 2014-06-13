using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Web.Http;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Services;

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

            public Tuple<DateTime, string> GetBerthContents(string berth)
            {
                return base.Channel.GetBerthContents(berth);
            }
        }

        [HttpGet]
        public IHttpActionResult GetBerthDescription(string berth)
        {
            TDCacheServiceClient cacheService = null;
            try
            {
                cacheService = new TDCacheServiceClient();
                cacheService.Open();
                Tuple<DateTime, string> result = cacheService.GetBerthContents(berth);
                if (result != null)
                    return Ok(result);
                else
                    return Ok((object)null);
            }
            catch (Exception e)
            {
                Trace.TraceError("Error In Cache Connection: {0}", e);
                return InternalServerError(e);
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
        }
    }
}
