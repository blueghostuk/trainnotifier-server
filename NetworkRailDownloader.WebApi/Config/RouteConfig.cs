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
                name: "TM-StartingAtLocation",
                routeTemplate: "TrainMovement/StartingAt/Location/{stanox}",
                defaults: new { controller = "TrainMovement", action = "StartingAtLocation" });
            routes.MapHttpRoute(
                name: "TM-StartingAtStation",
                routeTemplate: "TrainMovement/StartingAt/Station/{crsCode}",
                defaults: new { controller = "TrainMovement", action = "StartingAtStation" });

            routes.MapHttpRoute(
                name: "TM-CallingAtLocation",
                routeTemplate: "TrainMovement/CallingAt/Location/{stanox}",
                defaults: new { controller = "TrainMovement", action = "CallingAtLocation" });

            routes.MapHttpRoute(
                name: "TM-CallingAtStation",
                routeTemplate: "TrainMovement/CallingAt/Station/{crsCode}",
                defaults: new { controller = "TrainMovement", action = "CallingAtStation" });

            routes.MapHttpRoute(
                name: "TM-TerminatingAtStation",
                routeTemplate: "TrainMovement/TerminatingAtStation/{stanox}",
                defaults: new { controller = "TrainMovement", action = "TerminatingAtStation" });

            routes.MapHttpRoute(
                name: "TM-CallingAtLocations",
                routeTemplate: "TrainMovement/Between/Location/{fromStanox}/{toStanox}",
                defaults: new { controller = "TrainMovement", action = "CallingAtLocations" });

            routes.MapHttpRoute(
                name: "TM-CallingAtStations",
                routeTemplate: "TrainMovement/Between/Station/{fromCrs}/{toCrs}",
                defaults: new { controller = "TrainMovement", action = "CallingAtStations" });

            routes.MapHttpRoute(
                name: "TM-ById",
                routeTemplate: "TrainMovement/{id}",
                defaults: new { controller = "TrainMovement", action = "GetById" });

            routes.MapHttpRoute(
                name: "TM-ForUid",
                routeTemplate: "TrainMovement/Uid/{trainUid}/{date}",
                defaults: new { controller = "TrainMovement", action = "GetForUid" });

            routes.MapHttpRoute(
                name: "Assoc-ForTrain",
                routeTemplate: "Association/{trainUid}/{date}",
                defaults: new { controller = "Association", action = "GetForTrain" });

            routes.MapHttpRoute(
                name: "Schedule-ForStanox",
                routeTemplate: "Schedule/stanox/{stanox}/{date}",
                defaults: new { controller = "Schedule", action = "GetScheduleForDate" });

            routes.MapHttpRoute(
                name: "Schedule-ForUid",
                routeTemplate: "Schedule/uid/{trainUid}/{date}",
                defaults: new { controller = "Schedule", action = "GetByUid" });

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
