using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TrainNotifier.Common;
using TrainNotifier.Common.Model;

namespace TrainNotifier.Console.WebApi.Controllers
{
    public class CancellationsController : ApiController
    {
        public IEnumerable<ExtendedCancellation> GetCancellations(DateTime? startDate = null, DateTime? endDate = null)
        {
            throw new NotImplementedException();
        }
    }
}