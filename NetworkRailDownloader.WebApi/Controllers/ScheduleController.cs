using System;
using System.Collections.Generic;
using System.Web.Http;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.Schedule;
using TrainNotifier.Console.WebApi.ActionFilters;
using TrainNotifier.Service;

namespace TrainNotifier.Console.WebApi.Controllers
{
    public class ScheduleController : ApiController
    {
        // cache for 24 hrs
        [CachingActionFilterAttribute(60 * 60 * 24)]
        public ScheduleTrain GetByUid(string trainUid, DateTime date)
        {
            ScheduleRepository sr = new ScheduleRepository();

            return sr.GetTrainByUid(trainUid, date);
        }

        [HttpGet]
        [CachingActionFilterAttribute(604800)]
        public IEnumerable<ScheduleViewModel> GetScheduleForDate(string stanox, [FromUri]DateTime date)
        {
            ScheduleRepository sr = new ScheduleRepository();

            return sr.GetForDate(stanox, date);
        }
    }
}