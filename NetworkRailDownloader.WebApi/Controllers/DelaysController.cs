using System;
using System.Threading.Tasks;
using System.Web.Http;
using TrainNotifier.Console.WebApi.Attributes;
using TrainNotifier.Service;

namespace TrainNotifier.Console.WebApi.Controllers
{
    [RoutePrefix("delays")]
    public class DelaysController : ApiController
    {
        private static readonly DelaysRepository _delaysRepository = new DelaysRepository();

        [Route("{fromCrs}/{toCrs}")]
        [CacheControlAttribute(MaxAge = 60)]
        public async Task<IHttpActionResult> GetDelays(string fromCrs, string toCrs, DateTime startDate, DateTime endDate)
        {
            var results = await _delaysRepository.GetDelays(fromCrs, toCrs, startDate, endDate);

            return Ok(results);
        }
    }
}
