﻿using System;
using System.Collections.Generic;
using System.Web.Http;
using TrainNotifier.Common.Model;
using TrainNotifier.Console.WebApi.ActionFilters;
using TrainNotifier.Service;
using System.Linq;

namespace TrainNotifier.Console.WebApi.Controllers
{
    public class AssociationController : ApiController
    {
        [CachingActionFilterAttribute(60 * 60 * 24)]
        public IEnumerable<Association> GetForTrain(string trainUid, [FromUri]DateTime date)
        {
            AssociationRepository ar = new AssociationRepository();

            return ar.GetForTrain(trainUid, date)
                .OrderBy(a => a.AssociationType)
                .OrderBy(a => a.MainTrainUid);
        }
    }
}