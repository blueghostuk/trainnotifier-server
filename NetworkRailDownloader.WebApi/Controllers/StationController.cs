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
        public IEnumerable<StationTiploc> Get()
        {
            IEnumerable<StationTiploc> results = _tiplocRepo.Get();

            return results
                .Where(r => !string.IsNullOrEmpty(r.StationName));
        }

        [CachingActionFilterAttribute(604800)]
        public StationTiploc Get(string id)
        {
            return _tiplocRepo.GetByStationName(id);
        }

        [HttpGet]
        [CachingActionFilterAttribute(604800)]
        public IEnumerable<StationTiploc> GeoLookup(double lat, double lon, int limit = 5)
        {
            return _tiplocRepo.GetByLocation(lat, lon, limit);
        }
    }
}