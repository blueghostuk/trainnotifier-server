using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TrainNotifier.Console.WebApi.ActionFilters;
using TrainNotifier.Console.WebApi.ViewModels;
using TrainNotifier.ServiceLayer;

namespace TrainNotifier.Console.WebApi.Controllers
{
    public class StanoxController : ApiController
    {
        private static readonly TiplocRepository _tiplocRepo = new TiplocRepository();

        [CachingActionFilterAttribute(86400)]
        public IEnumerable<Stanox> Get()
        {
            IEnumerable<dynamic> results = _tiplocRepo.Get();

            return results.Select(r => MapStanox(r))
                .Cast<Stanox>();
        }

        [CachingActionFilterAttribute(86400)]
        public Stanox Get(string id)
        {
            dynamic result = _tiplocRepo.GetByStanox(id);

            if (result != null)
            {
                return MapStanox(result);
            }

            return null;
        }

        [CachingActionFilterAttribute(86400)]
        public Stanox GetByCrs(string crsCode)
        {
            dynamic result = _tiplocRepo.GetByCRSCode(crsCode);

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