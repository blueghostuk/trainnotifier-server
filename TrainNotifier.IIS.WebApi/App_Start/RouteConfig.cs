using System.Web.Http;
using System.Web.Routing;

namespace TrainNotifier.IIS.WebApi
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapHttpRoute(
                name: "Stanox-FindByCrs",
                routeTemplate: "Stanox/Single/{crsCode}",
                defaults: new { controller = "Stanox", action = "GetByCrs" });
            routes.MapHttpRoute(
                name: "Stanox-AllByCRS",
                routeTemplate: "Stanox/Find/{crsCode}",
                defaults: new { controller = "Stanox", action = "AllByCrs" });

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
                name: "TM-CallingAtLocations",
                routeTemplate: "TrainMovement/Between/Location/{fromStanox}/{toStanox}",
                defaults: new { controller = "TrainMovement", action = "CallingAtLocations" });

            routes.MapHttpRoute(
                name: "TM-CallingAtStations",
                routeTemplate: "TrainMovement/Between/Station/{fromCrs}/{toCrs}",
                defaults: new { controller = "TrainMovement", action = "CallingAtStations" });

            routes.MapHttpRoute(
                name: "TM-TerminatingAtLocation",
                routeTemplate: "TrainMovement/TerminatingAt/Location/{stanox}",
                defaults: new { controller = "TrainMovement", action = "TerminatingAtLocation" });

            routes.MapHttpRoute(
                name: "TM-TerminatingAtStation",
                routeTemplate: "TrainMovement/TerminatingAt/Station/{crsCode}",
                defaults: new { controller = "TrainMovement", action = "TerminatingAtStation" });

            routes.MapHttpRoute(
                name: "TM-Nearest",
                routeTemplate: "TrainMovement/Nearest/",
                defaults: new { controller = "TrainMovement", action = "GetNearestTrain" });

            routes.MapHttpRoute(
                name: "TM-ById",
                routeTemplate: "TrainMovement/{id}",
                defaults: new { controller = "TrainMovement", action = "GetById" });

            routes.MapHttpRoute(
                name: "TM-ForUid",
                routeTemplate: "TrainMovement/Uid/{trainUid}/{date}",
                defaults: new { controller = "TrainMovement", action = "GetForUid" });

            routes.MapHttpRoute(
                name: "TM-ForHeadcode",
                routeTemplate: "TrainMovement/Headcode/{headcode}/{date}",
                defaults: new { controller = "TrainMovement", action = "GetForHeadcode" });

            routes.MapHttpRoute(
                name: "TM-ForHeadcodeAndLocation",
                routeTemplate: "TrainMovement/Headcode/{headcode}/{crsCode}/{platform}",
                defaults: new { controller = "TrainMovement", action = "GetForHeadcodeByLocation" });

            routes.MapHttpRoute(
                name: "Assoc-ForTrain",
                routeTemplate: "Association/{trainUid}/{date}",
                defaults: new { controller = "Association", action = "GetForTrain" });

            routes.MapHttpRoute(
                name: "TD-ForTrain",
                routeTemplate: "Td/Describer/{describer}",
                defaults: new { controller = "TD", action = "GetTrain" });

            routes.MapHttpRoute(
                name: "TD-ForBerth",
                routeTemplate: "Td/Berth/{berth}",
                defaults: new { controller = "TD", action = "GetBerthDescription" });

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}