using NetworkRailDownloader.ServiceLayer;
using NetworkRailDownloader.WebApi.ActionFilters;
using NetworkRailDownloader.WebApi.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NetworkRailDownloader.WebApi.Controllers
{
    public class StanoxController : ApiController
    {
        //[WebApiOutputCache(86400, 604800, false)]
        public IEnumerable<Stanox> Get()
        {
            IEnumerable<dynamic> results = new TiplocRepository().Get();

            return results.Select(r => MapStanox(r))
                .Cast<Stanox>();
        }

        //[WebApiOutputCache(3600, 604800, false)]
        public Stanox Get(string id)
        {
            dynamic result = new TiplocRepository().GetByStanox(id);

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