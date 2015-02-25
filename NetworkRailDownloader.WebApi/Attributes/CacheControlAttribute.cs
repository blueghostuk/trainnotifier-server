using System;
using System.Net.Http.Headers;
using System.Web.Http.Filters;

namespace TrainNotifier.Console.WebApi.Attributes
{
    public class CacheControlAttribute : ActionFilterAttribute
    {
        public int MaxAge { get; set; }

        public CacheControlAttribute()
        {
            MaxAge = 3600;
        }

        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            if (context != null && context.Response != null)
            {
                context.Response.Headers.CacheControl = new CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = TimeSpan.FromSeconds(MaxAge)
                };
            }

            base.OnActionExecuted(context);
        }
    }
}
