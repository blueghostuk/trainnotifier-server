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
    public class StanoxController : ApiController
    {
        // GET <controller>/5
        public Stanox Get(string id)
        {
            dynamic result = new TiplocRepository().GetByStanox(id);

            if (result != null)
            {
                return new Stanox
                {
                    Tiploc = result.Tiploc,
                    Nalco = result.Nalco,
                    Description = result.Description,
                    CRS = result.CRS,
                    StationName = result.StationName,
                    Lat = result.lat,
                    Lon = result.lon
                };
            }

            return null;
        }
    }
}