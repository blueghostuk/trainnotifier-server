using System;
using System.Net.Http.Headers;
using System.Web.Http.Filters;

namespace TrainNotifier.Console.WebApi.ActionFilters
{
    internal sealed class CachingActionFilterAttribute : ActionFilterAttribute
    {
        // cache length in seconds
        private readonly TimeSpan _timespan;

        public CachingActionFilterAttribute(int timespanSeconds)
        {
            _timespan = TimeSpan.FromSeconds(timespanSeconds);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext != null &&
                actionExecutedContext.Response != null &&
                actionExecutedContext.Response.IsSuccessStatusCode &&
                actionExecutedContext.Response.Headers != null)
            {
                actionExecutedContext.Response.Headers.CacheControl = new CacheControlHeaderValue
                {
                    MaxAge = _timespan
                };
            }
            //base.OnActionExecuted(actionExecutedContext);
        }
    }
}
