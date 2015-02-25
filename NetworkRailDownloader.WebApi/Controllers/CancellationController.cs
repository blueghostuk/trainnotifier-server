using System;
using System.Threading.Tasks;
using System.Web.Http;
using TrainNotifier.Console.WebApi.Attributes;
using TrainNotifier.Service;

namespace TrainNotifier.Console.WebApi.Controllers
{
    [RoutePrefix("cancellations")]
    public class CancellationController : ApiController
    {
        private static readonly CancellationRepository _cancellationRepository = new CancellationRepository();

        [Route("{fromCrs}/{toCrs}")]
        [CacheControlAttribute(MaxAge = 60)]
        public async Task<IHttpActionResult> GetDelays(string fromCrs, string toCrs, DateTime startDate, DateTime endDate)
        {
            var results = await _cancellationRepository.GetCancellations(fromCrs, toCrs, startDate, endDate);

            return Ok(results);
        }
    }
}
