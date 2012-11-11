using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return _archiveRepo.GetBtWttId(wttId)
                .Select(r => new WttIdSearchResult
                {
                    WttId = r.sched_wtt_id,
                    TrainId = r.Id,
                    From = r.sched_origin_stanox,
                    Depart = r.origin_dep_timestamp
                });
        }

        public TrainMovement GetTrainMovementById(string trainId)
        {
            return _archiveRepo.GetTrainMovementById(trainId);
        }

        public IEnumerable<TrainMovement> GetTrainMovementsOrigin(string stanox)
        {
            return _archiveRepo.GetTrainMovementsOrigin(stanox);
        }
    }
}
