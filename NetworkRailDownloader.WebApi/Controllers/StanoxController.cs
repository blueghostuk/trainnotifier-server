using System.Collections.Generic;
using System.Web.Http;
using TrainNotifier.Common.Model.Schedule;
using TrainNotifier.Console.WebApi.ActionFilters;
using TrainNotifier.Service;
using System.Linq;

namespace TrainNotifier.Console.WebApi.Controllers
{
    public class StanoxController : ApiController
    {
        private static readonly TiplocRepository _tiplocRepo = new TiplocRepository();

        [CachingActionFilterAttribute(604800)]
        public IHttpActionResult Get()
        {
            IEnumerable<StationTiploc> tiplocs = _tiplocRepo.Get();
            if (tiplocs.Any())
            {
                return Ok(tiplocs);
            }
            else
            {
                return NotFound();
            }
        }

        [CachingActionFilterAttribute(604800)]
        public IHttpActionResult Get(string id)
        {
            StationTiploc tiploc = _tiplocRepo.GetByStanox(id);
            if (tiploc != null)
            {
                return Ok(tiploc);
            }
            else
            {
                return NotFound();
            }
        }

        [CachingActionFilterAttribute(604800)]
        [HttpGet]
        public IHttpActionResult GetByCrs(string crsCode)
        {
            StationTiploc tiploc = _tiplocRepo.GetByCRSCode(crsCode);
            if (tiploc != null)
            {
                return Ok(tiploc);
            }
            else
            {
                return NotFound();
            }
        }

        [CachingActionFilterAttribute(604800)]
        [HttpGet]
        public IHttpActionResult AllByCrs(string crsCode)
        {
            IEnumerable<StationTiploc> tiplocs = _tiplocRepo.GetAllByCRSCode(crsCode);
            if (tiplocs.Any())
            {
                return Ok(tiplocs);
            }
            else
            {
                return NotFound();
            }
        }
    }
}