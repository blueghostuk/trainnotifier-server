using System;
using System.Collections.Generic;
using System.Web.Http;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.Api;
using TrainNotifier.Console.WebApi.ActionFilters;
using TrainNotifier.Service;
using System.Linq;

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
        public IEnumerable<TrainMovementResult> StartingAtLocation(string stanox, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));
            return _tmRepo.StartingAtLocation(stanox, startDate, endDate);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IEnumerable<TrainMovementResult> StartingAtStation(string crsCode, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));
            return _tmRepo.StartingAtStation(crsCode, startDate, endDate);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IEnumerable<TrainMovementResult> CallingAtLocation(string stanox, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));
            return _tmRepo.CallingAtLocation(stanox, startDate, endDate);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IEnumerable<TrainMovementResult> CallingAtStation(string crsCode, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));
            return _tmRepo.CallingAtStation(crsCode, startDate, endDate);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IEnumerable<TrainMovementResult> CallingAtLocations(string fromStanox, string toStanox, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));
            return _tmRepo.CallingBetweenLocations(fromStanox, toStanox, startDate, endDate);
        }

        [HttpGet]
        [CachingActionFilterAttribute(120)]
        public IEnumerable<TrainMovementResult> CallingAtStations(string fromCrs, string toCrs, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.Date;
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));
            return _tmRepo.CallingBetweenStations(fromCrs, toCrs, startDate, endDate);
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
