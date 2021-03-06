﻿using System.Web.Http;

namespace TrainNotifier.Console.WebApi.Config
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "Stanox-FindByCrs",
                routeTemplate: "Stanox/Single/{crsCode}",
                defaults: new { controller = "Stanox", action = "GetByCrs" });

            config.Routes.MapHttpRoute(
                name: "Stanox-AllByCRS",
                routeTemplate: "Stanox/Find/{crsCode}",
                defaults: new { controller = "Stanox", action = "AllByCrs" });

            config.Routes.MapHttpRoute(
                name: "TM-StartingAtLocation",
                routeTemplate: "TrainMovement/StartingAt/Location/{stanox}",
                defaults: new { controller = "TrainMovement", action = "StartingAtLocation" });
            config.Routes.MapHttpRoute(
                name: "TM-StartingAtStation",
                routeTemplate: "TrainMovement/StartingAt/Station/{crsCode}",
                defaults: new { controller = "TrainMovement", action = "StartingAtStation" });

            config.Routes.MapHttpRoute(
                name: "TM-CallingAtLocation",
                routeTemplate: "TrainMovement/CallingAt/Location/{stanox}",
                defaults: new { controller = "TrainMovement", action = "CallingAtLocation" });

            config.Routes.MapHttpRoute(
                name: "TM-CallingAtStation",
                routeTemplate: "TrainMovement/CallingAt/Station/{crsCode}",
                defaults: new { controller = "TrainMovement", action = "CallingAtStation" });

            config.Routes.MapHttpRoute(
                name: "TM-CallingAtLocations",
                routeTemplate: "TrainMovement/Between/Location/{fromStanox}/{toStanox}",
                defaults: new { controller = "TrainMovement", action = "CallingAtLocations" });

            config.Routes.MapHttpRoute(
                name: "TM-CallingAtStations",
                routeTemplate: "TrainMovement/Between/Station/{fromCrs}/{toCrs}",
                defaults: new { controller = "TrainMovement", action = "CallingAtStations" });

            config.Routes.MapHttpRoute(
                name: "TM-TerminatingAtLocation",
                routeTemplate: "TrainMovement/TerminatingAt/Location/{stanox}",
                defaults: new { controller = "TrainMovement", action = "TerminatingAtLocation" });

            config.Routes.MapHttpRoute(
                name: "TM-TerminatingAtStation",
                routeTemplate: "TrainMovement/TerminatingAt/Station/{crsCode}",
                defaults: new { controller = "TrainMovement", action = "TerminatingAtStation" });

            config.Routes.MapHttpRoute(
                name: "TM-Nearest",
                routeTemplate: "TrainMovement/Nearest/",
                defaults: new { controller = "TrainMovement", action = "GetNearestTrain" });

            config.Routes.MapHttpRoute(
                name: "TM-ById",
                routeTemplate: "TrainMovement/{id}",
                defaults: new { controller = "TrainMovement", action = "GetById" });

            config.Routes.MapHttpRoute(
                name: "TM-ForUid",
                routeTemplate: "TrainMovement/Uid/{trainUid}/{date}",
                defaults: new { controller = "TrainMovement", action = "GetForUid" });

            config.Routes.MapHttpRoute(
                name: "TM-ForHeadcode",
                routeTemplate: "TrainMovement/Headcode/{headcode}/{date}",
                defaults: new { controller = "TrainMovement", action = "GetForHeadcode" });

            config.Routes.MapHttpRoute(
                name: "TM-ForHeadcodeAndLocation",
                routeTemplate: "TrainMovement/Headcode/{headcode}/{crsCode}/{platform}",
                defaults: new { controller = "TrainMovement", action = "GetForHeadcodeByLocation" });

            config.Routes.MapHttpRoute(
                name: "Assoc-ForTrain",
                routeTemplate: "Association/{trainUid}/{date}",
                defaults: new { controller = "Association", action = "GetForTrain" });

            config.Routes.MapHttpRoute(
                name: "TD-ForTrain",
                routeTemplate: "Td/Describer/{describer}",
                defaults: new { controller = "TD", action = "GetTrain" });

            config.Routes.MapHttpRoute(
                name: "TD-ForBerth",
                routeTemplate: "Td/Berth/{berth}",
                defaults: new { controller = "TD", action = "GetBerthDescription" });

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
