using System.Net.Http.Headers;
using System.Web.Http.Filters;
using TrainNotifier.Common.Model;

namespace TrainNotifier.Console.WebApi.ActionFilters
{
    internal sealed class CacheableItemFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var returnedItem = ((System.Net.Http.ObjectContent)(actionExecutedContext.Response.Content)).Value as ICacheableItem;
            if (returnedItem != null && returnedItem.CacheAge.HasValue)
            {
                actionExecutedContext.Response.Headers.CacheControl = new CacheControlHeaderValue
                {
                    MaxAge = returnedItem.CacheAge
                };
            }
        }
    }
}
