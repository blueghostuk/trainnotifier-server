using System.Collections.Generic;
using System.Web.Http;
using TrainNotifier.Common.Model.Schedule;
using TrainNotifier.Console.WebApi.ActionFilters;
using TrainNotifier.Service;

namespace TrainNotifier.Console.WebApi.Controllers
{
    public class StanoxController : ApiController
    {
        private static readonly TiplocRepository _tiplocRepo = new TiplocRepository();

        [CachingActionFilterAttribute(604800)]
        public IEnumerable<StationTiploc> Get()
        {
            return _tiplocRepo.Get();
        }

        [CachingActionFilterAttribute(604800)]
        public StationTiploc Get(string id)
        {
            return _tiplocRepo.GetByStanox(id);
        }

        [CachingActionFilterAttribute(604800)]
        public StationTiploc GetByCrs(string crsCode)
        {
            return _tiplocRepo.GetByCRSCode(crsCode);
        }
    }
}