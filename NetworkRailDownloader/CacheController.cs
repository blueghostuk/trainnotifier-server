using Newtonsoft.Json;
using System;
using System.Linq;
using System.ServiceModel;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Services;
using TrainNotifier.ServiceLayer;

namespace TrainNotifier.Console.WebSocketServer
{
    internal sealed class CacheController : IDisposable
    {
        private readonly UserManager _userManager;
        private readonly StanoxRepository _stanoxRepository = new StanoxRepository();

        public CacheController(NMSWrapper nmsWrapper, WebSocketServerWrapper wssWrapper, UserManager userManager)
        {
            _userManager = userManager;
            nmsWrapper.FeedDataRecieved += (s, f) =>
            {
                foreach (var message in f.Data)
                {
                    switch (byte.Parse((string)message.header.msg_type))
                    {
                        // activation
                        case 1:
                            CacheActivation(message.body);
                            break;

                        // cancellation
                        case 2:
                            break;

                        // train movement
                        case 3:
                            CacheTrainMovement((string)message.body.train_id, message.body);
                            break;

                        // unidentified train
                        case 4:
                            break;

                        // train reinstatement
                        case 5:
                            break;

                        // train change of origin
                        case 6:
                            break;

                        // train change of identity
                        case 7:
                            break;

                        // train change of location
                        case 8:
                            break;
                    }
                }
            };

            wssWrapper.OnReceive += (s, context) =>
            {
                string command = context.UserContext.DataFrame.ToString();
                switch (new string(command.Take(10).ToArray()))
                {
                    case "gettrain--":
                        HandleGetTrainCommand(context, new string(command.Skip(11).ToArray()));
                        break;
                    case "stanox----":
                        HandleStanoxCommand(context, "stanox", new string(command.Skip(11).ToArray()));
                        break;
                    case "crs-------":
                        HandleStanoxCommand(context, "crs", _stanoxRepository.GetStanoxByCrs(new string(command.Skip(11).ToArray())));
                        break;
                }
            };
        }

        private void HandleGetTrainCommand(UserContextEventArgs context, string trainId)
        {
            TrainMovement trainMovement;
            CacheServiceClient _cacheService = null;
            try
            {
                _cacheService = new CacheServiceClient();
                _cacheService.Open();
                if (_cacheService.TryGetTrainMovement(trainId, out trainMovement))
                {
                    string response = JsonConvert.SerializeObject(new CommandResponse<TrainMovement>
                    {
                        Command = "gettrain",
                        Args = trainId,
                        Response = trainMovement
                    });
                    context.UserContext.Send(response);
                }
            }
            finally
            {
                if (_cacheService != null)
                    _cacheService.Close();
            }
        }

        private void HandleStanoxCommand(UserContextEventArgs context, string command, string stanoxName)
        {
            CacheServiceClient _cacheService = null;
            try
            {
                _cacheService = new CacheServiceClient();
                _cacheService.Open();
                Stanox stanox;
                if (_cacheService.TryGetStanox(stanoxName, out stanox))
                {
                    string response = JsonConvert.SerializeObject(new CommandResponse<Stanox>
                    {
                        Command = command,
                        Args = stanoxName,
                        Response = stanox
                    });
                    context.UserContext.Send(response);
                }
            }
            finally
            {
                if (_cacheService != null)
                    _cacheService.Close();
            }
        }

        private void CacheActivation(dynamic body)
        {
            TrainMovement trainMovement = new TrainMovement
            {
                Activated = UnixTsToDateTime(double.Parse((string)body.creation_timestamp)),
                Id = (string)body.train_id,
                SchedOriginDeparture = UnixTsToDateTime(double.Parse((string)body.origin_dep_timestamp)),
                SchedOriginStanox = (string)body.sched_origin_stanox,
                ServiceCode = (string)body.train_service_code
            };
            CacheServiceClient _cacheService = null;
            try
            {
                _cacheService = new CacheServiceClient();
                _cacheService.Open();
                _cacheService.CacheTrainMovement(trainMovement);
            }
            finally
            {
                if (_cacheService != null)
                    _cacheService.Close();
            }
        }

        private void CacheTrainMovement(string trainId, dynamic body)
        {
            if (string.IsNullOrWhiteSpace(trainId))
                return;

            CacheServiceClient _cacheService = null;
            try
            {
                _cacheService = new CacheServiceClient();
                _cacheService.Open();
                TrainMovement trainMovement;
                if (!_cacheService.TryGetTrainMovement(trainId, out trainMovement))
                {
                    trainMovement = new TrainMovement
                    {
                        Id = (string)body.train_id,
                        ServiceCode = (string)body.train_service_code
                    };
                    _cacheService.CacheTrainMovement(trainMovement);
                }
                if (trainMovement != null)
                {
                    DateTime? plannedTime = null;
                    if (!string.IsNullOrEmpty((string)body.planned_timestamp))
                    {
                        plannedTime = UnixTsToDateTime(double.Parse((string)body.planned_timestamp));
                    }
                    TrainMovementStep step = new TrainMovementStep
                    {
                        ActualTimeStamp = UnixTsToDateTime(double.Parse((string)body.actual_timestamp)),
                        EventType = (string)body.event_type,
                        Line = (string)body.line_ind,
                        PlannedTime = plannedTime,
                        Platform = (string)body.platform,
                        Stanox = (string)body.loc_stanox,
                        Terminated = ((string)body.train_teminated) == "true"
                    };
                    _cacheService.CacheTrainStep(trainId, (string)body.train_service_code, step);
                }
            }
            finally
            {
                if (_cacheService != null)
                    _cacheService.Close();
            }
        }

        private static readonly DateTime _epoch = new DateTime(1970, 1, 1);

        private static DateTime UnixTsToDateTime(double timeStamp)
        {
            return _epoch.AddMilliseconds(timeStamp);
        }

        public void Dispose()
        {
        }
    }
}
