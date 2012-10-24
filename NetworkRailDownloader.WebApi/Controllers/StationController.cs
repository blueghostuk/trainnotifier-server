using NetworkRailDownloader.ServiceLayer;
using NetworkRailDownloader.WebApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NetworkRailDownloader.WebApi.Controllers
{
    public class StationController : ApiController
    {
        public IEnumerable<string> Get()
        {
            IEnumerable<dynamic> results = new TiplocRepository().Get();

            return results
                .Where(r => !string.IsNullOrEmpty(r.StationName))
                .Select(r => r.StationName + " (" + r.CRS + ")")
                .Cast<string>();
        }

        // GET <controller>/5
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