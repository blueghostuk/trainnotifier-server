using System;
using System.Collections.Generic;
using System.Web.Http;
using TrainNotifier.Common.Model;
using TrainNotifier.Console.WebApi.ActionFilters;
using TrainNotifier.Service;

namespace TrainNotifier.Console.WebApi.Controllers
{
    public class TrainMovementController : ApiController
    {
        private static readonly LiveTrainRepository _liveTrainRepo = new LiveTrainRepository();
        private static readonly TrainMovementRepository _tmRepo = new TrainMovementRepository();

        [HttpGet]
        public IEnumerable<TrainMovement> GetById(string id)
        {
            return _liveTrainRepo.GetTrainMovementById(id);
        }

        [HttpGet]
        public ExtendedTrainMovement GetWithUid(string id, string uid)
        {
            return _liveTrainRepo.GetTrainMovementById(id, uid);
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
            return _liveTrainRepo.SearchByHeadcode(headcode);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IEnumerable<TrainMovement> WithWttId(string wttId)
        {
            return _liveTrainRepo.SearchByWttId(wttId);
        }
    }
}
