﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TrainNotifier.Console.WebApi.ActionFilters;
using TrainNotifier.Console.WebApi.ViewModels;
using TrainNotifier.ServiceLayer;

namespace TrainNotifier.Console.WebApi.Controllers
{
    public class StanoxController : ApiController
    {
        [CachingActionFilterAttribute(86400)]
        public IEnumerable<Stanox> Get()
        {
            IEnumerable<dynamic> results = new TiplocRepository().Get();

            return results.Select(r => MapStanox(r))
                .Cast<Stanox>();
        }

        [CachingActionFilterAttribute(3600)]
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