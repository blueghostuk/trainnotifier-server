using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TrainNotifier.Console.WebApi.ActionFilters;
using TrainNotifier.Console.WebApi.ViewModels;
using TrainNotifier.ServiceLayer;

namespace TrainNotifier.Console.WebApi.Controllers
{
    public class StationController : ApiController
    {
        private static readonly TiplocRepository _tiplocRepo = new TiplocRepository();

        [CachingActionFilterAttribute(86400)]
        public IEnumerable<string> Get()
        {
            IEnumerable<dynamic> results = _tiplocRepo.Get();

            return results
                .Where(r => !string.IsNullOrEmpty(r.StationName))
                .Select(r => r.StationName + " (" + r.CRS + ")")
                .Cast<string>();
        }

        [CachingActionFilterAttribute(3600)]
        public Stanox Get(string id)
        {
            dynamic result = _tiplocRepo.GetByStationName(id);

            if (result != null)
            {
                return MapStanox(result);
            }

            return null;
        }

        private static Stanox MapStanox(dynamic result)
        {
            return new Stanox
            {
                Name = result.Stanox,
                Tiploc = result.Tiploc,
                Nalco = result.Nalco,
                Description = result.Description,
                CRS = result.CRS,
                StationName = result.StationName,
                Lat = result.lat,
                Lon = result.lon
            };
        }
    }
}