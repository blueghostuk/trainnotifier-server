using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace TrainNotifier.Console.WebApi.Config
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(HttpRouteCollection routes)
        {
            // Train Movements
            routes.MapHttpRoute(
                name: "TM-StartingAtStation",
                routeTemplate: "TrainMovement/StartingAtStation/{stanox}",
                defaults: new { controller = "TrainMovement", action = "StartingAtStation" });

            routes.MapHttpRoute(
                name: "TM-CallingAtStation",
                routeTemplate: "TrainMovement/CallingAtStation/{stanox}",
                defaults: new { controller = "TrainMovement", action = "CallingAtStation" });

            routes.MapHttpRoute(
                name: "TM-WithHeadcode",
                routeTemplate: "TrainMovement/WithHeadcode/{headcode}",
                defaults: new { controller = "TrainMovement", action = "WithHeadcode" });

            routes.MapHttpRoute(
                name: "TM-WithWttId",
                routeTemplate: "TrainMovement/WithWttId/{wttId}",
                defaults: new { controller = "TrainMovement", action = "WithWttId" });

            routes.MapHttpRoute(
                name: "TM-ById",
                routeTemplate: "TrainMovement/{id}",
                defaults: new { controller = "TrainMovement", action = "GetById" });

            routes.MapHttpRoute(
                name: "TM-WithUid",
                routeTemplate: "TrainMovement/{id}/{uid}",
                defaults: new { controller = "TrainMovement", action = "GetWithUid" });

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
