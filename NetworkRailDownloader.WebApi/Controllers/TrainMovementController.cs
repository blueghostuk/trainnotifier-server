using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TrainNotifier.Common.Model;
using TrainNotifier.Console.WebApi.ActionFilters;
using TrainNotifier.Console.WebApi.ViewModels;
using TrainNotifier.Service;

namespace TrainNotifier.Console.WebApi.Controllers
{
    public class TrainMovementController : ApiController
    {
        private static readonly ArchiveRepository _archiveRepo = new ArchiveRepository();

        [HttpGet]
        public TrainMovement Get(string id)
        {
            return _archiveRepo.GetTrainMovementById(id);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IEnumerable<TrainMovement> StartingAtStation(string stanox)
        {
            return _archiveRepo.SearchByOrigin(stanox);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IEnumerable<TrainMovement> CallingAtStation(string stanox)
        {
            return _archiveRepo.TrainsCallingAtStation(stanox);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IEnumerable<TrainMovement> WithHeadcode(string headcode)
        {
            return _archiveRepo.SearchByHeadcode(headcode);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IEnumerable<WttIdSearchResult> WithWttId(string wttId)
        {
            return _archiveRepo.SearchByWttId(wttId)
                .Select(r => new WttIdSearchResult
                {
                    TrainId = r.TrainId,
                    HeadCode = r.Headcode,
                    WttId = r.SchedWttId,
                    From = r.OriginStanox,
                    Depart = r.OriginDepartTimestamp
                });
        }
    }
}
