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
        private static readonly TrainMovementRepository _tmRepo = new TrainMovementRepository();

        [HttpGet]
        [Obsolete("Will be removed in future version")]
        public ViewModelTrainMovement GetById(string id)
        {
            return _tmRepo.GetTrainMovementById(id);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public ViewModelTrainMovement GetForUid(string trainUid, DateTime date)
        {
            return _tmRepo.GetTrainMovementById(trainUid, date);
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
        public IEnumerable<CallingAtStationsTrainMovement> CallingAtStations(string fromStanox, string toStanox, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.AddDays(1);
            return _tmRepo.CallingAt(fromStanox, toStanox, startDate, endDate);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IEnumerable<OriginTrainMovement> TerminatingAtStation(string stanox, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.AddDays(1);
            return _tmRepo.TerminatingAtStation(stanox, startDate, endDate);
        }
    }
}
