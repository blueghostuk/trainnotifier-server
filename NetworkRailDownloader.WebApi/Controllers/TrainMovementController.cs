using System;
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
        private static readonly TrainMovementRepository _tmRepo = new TrainMovementRepository();

        [HttpGet]
        public IEnumerable<TrainMovement> GetById(string id)
        {
            return _archiveRepo.GetTrainMovementById(id);
        }

        [HttpGet]
        public TrainMovement GetWithUid(string id, string uid)
        {
            return _archiveRepo.GetTrainMovementById(id, uid);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IEnumerable<OriginTrainMovement> StartingAtStation(string stanox, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.AddDays(1);
            return _tmRepo.StartingAt(stanox, startDate, endDate);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IEnumerable<CallingAtTrainMovement> CallingAtStation(string stanox, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.AddDays(1);
            return _tmRepo.CallingAt(stanox, startDate, endDate);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IEnumerable<TrainMovement> WithHeadcode(string headcode)
        {
            return _archiveRepo.SearchByHeadcode(headcode);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IEnumerable<TrainMovement> WithWttId(string wttId)
        {
            return _archiveRepo.SearchByWttId(wttId);
        }
    }
}
