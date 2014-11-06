using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using TrainNotifier.IIS.WebApi.OpenLDBWS;

namespace TrainNotifier.IIS.WebApi.Controllers
{
    [RoutePrefix("darwin")]
    public class DarwinController : ApiController
    {
        private static readonly AccessToken DarwinToken = new AccessToken
        {
            TokenValue = WebConfigurationManager.AppSettings["darwinToken"]
        };

        [HttpGet]
        [Route("station/{crsCode}/{toCrsCode}")]
        public async Task<IHttpActionResult> GetServices(string crsCode, string toCrsCode)
        {
            using (var darwin = new LDBServiceSoapClient())
            {
                var toResult = darwin.GetArrivalDepartureBoardAsync(DarwinToken, 20, crsCode, toCrsCode, FilterType.to, -30, 60);
                var fromResult = darwin.GetArrivalDepartureBoardAsync(DarwinToken, 20, crsCode, toCrsCode, FilterType.from, -30, 60);

                await Task.WhenAll(new[] { toResult, fromResult });

                return Ok(new[] { 
                    toResult.Result.GetStationBoardResult,
                    fromResult.Result.GetStationBoardResult
                });
            }
        }

        [HttpGet]
        [Route("services/")]
        public async Task<IHttpActionResult> GetService(string serviceId)
        {
            using (var darwin = new LDBServiceSoapClient())
            {
                var result = await darwin.GetServiceDetailsAsync(DarwinToken, serviceId);

                if (result == null)
                    return NotFound();

                return Ok(result.GetServiceDetailsResult);
            }
        }
    }
}
