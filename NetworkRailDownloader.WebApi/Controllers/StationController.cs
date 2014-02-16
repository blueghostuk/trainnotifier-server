using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TrainNotifier.Common.Model.Schedule;
using TrainNotifier.Console.WebApi.ActionFilters;
using TrainNotifier.Service;

namespace TrainNotifier.Console.WebApi.Controllers
{
    public class StationController : ApiController
    {
        private static readonly TiplocRepository _tiplocRepo = new TiplocRepository();

        [CachingActionFilterAttribute(604800)]
        public IHttpActionResult Get()
        {
            IEnumerable<StationTiploc> results = _tiplocRepo.Get();

            var filtered = results
                .Where(r => !string.IsNullOrEmpty(r.StationName))
                .Where(r => !string.IsNullOrEmpty(r.CRS))
                .ToList();

            if (filtered.Any())
            {
                return Ok(filtered);
            }
            else
            {
                return NotFound();
            }
        }

        [CachingActionFilterAttribute(604800)]
        public IHttpActionResult Get(string id)
        {
            StationTiploc tiploc = _tiplocRepo.GetByStationName(id);
            if (tiploc != null)
            {
                return Ok(tiploc);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [CachingActionFilterAttribute(604800)]
        public IHttpActionResult GeoLookup(double lat, double lon, int limit = 5)
        {
            IEnumerable<StationTiploc> results = _tiplocRepo.GetByLocation(lat, lon, limit);
            if (results.Any())
                return Ok(results);
            else
                return NotFound();
        }
    }
}