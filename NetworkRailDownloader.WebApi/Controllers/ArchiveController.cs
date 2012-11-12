using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TrainNotifier.Common.Model;
using TrainNotifier.Console.WebApi.ViewModels;
using TrainNotifier.Service;

namespace TrainNotifier.Console.WebApi.Controllers
{
    public class ArchiveController : ApiController
    {
        private static readonly ArchiveRepository _archiveRepo = new ArchiveRepository();

        public IEnumerable<WttIdSearchResult> GetByWttId(string wttId)
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

        public TrainMovement GetTrainMovementById(string trainId)
        {
            return _archiveRepo.GetTrainMovementById(trainId);
        }

        public IEnumerable<TrainMovement> GetTrainMovementsByOrigin(string stanox)
        {
            return _archiveRepo.SearchByOrigin(stanox);
        }

        public IEnumerable<TrainMovement> GetTrainMovementsByHeadcode(string headcode)
        {
            return _archiveRepo.SearchByHeadcode(headcode);
        }
    }
}
