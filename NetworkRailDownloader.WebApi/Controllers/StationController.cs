using NetworkRailDownloader.ServiceLayer;
using NetworkRailDownloader.WebApi.ActionFilters;
using NetworkRailDownloader.WebApi.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NetworkRailDownloader.WebApi.Controllers
{
    public class StationController : ApiController
    {
        [WebApiOutputCache(86400, 604800, false)]
        public IEnumerable<string> Get()
        {
            IEnumerable<dynamic> results = new TiplocRepository().Get();

            return results
                .Where(r => !string.IsNullOrEmpty(r.StationName))
                .Select(r => r.StationName + " (" + r.CRS + ")")
                .Cast<string>();
        }

        [WebApiOutputCache(3600, 604800, false)]
        public Stanox Get(string id)
        {
            dynamic result = new TiplocRepository().GetByStationName(id);

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