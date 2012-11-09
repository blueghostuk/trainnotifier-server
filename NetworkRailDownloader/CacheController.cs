using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
                            CacheTrainCancellation((string)message.body.train_id, message.body);
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
                int idx = command.IndexOf(':');
                if (idx != -1)
                {
                    string cmdText = new string(command.Take(idx).ToArray());
                    string args = new string(command.Skip(idx + 1).ToArray());
                    switch (cmdText)
                    {
                        case "getservice":
                            HandleGetServiceCommand(context, args);
                            break;
                        case "gettrain":
                            HandleGetTrainCommand(context, args);
                            break;
                        case "subtrain":
                            HandleSubTrainCommand(context, args, true);
                            break;
                        case "unsubtrain":
                            HandleSubTrainCommand(context, args, false);
                            break;
                        case "stanox":
                            HandleStanoxCommand(context, "stanox", args);
                            break;
                        case "crs":
                            HandleStanoxCommand(context, "crs", _stanoxRepository.GetStanoxByCrs(args));
                            break;
                    }
                }
            };
        }

        private void HandleGetServiceCommand(UserContextEventArgs context, string headCode)
        {
            CacheServiceClient _cacheService = null;
            try
            {
                IEnumerable<string> data;
                _cacheService = new CacheServiceClient();
                _cacheService.Open();
                if (_cacheService.TryGetService(headCode, out data))
                {
                    string response = JsonConvert.SerializeObject(new CommandResponse<IEnumerable<string>>
                    {
                        Command = "getservice",
                        Args = headCode,
                        Response = data
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

        private void HandleSubTrainCommand(UserContextEventArgs context, string trainId, bool subscribe)
        {
            CacheServiceClient _cacheService = null;
            try
            {
                TrainMovement trainMovement;
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

        private void HandleGetTrainCommand(UserContextEventArgs context, string trainId)
        {
            CacheServiceClient _cacheService = null;
            try
            {
                TrainMovement trainMovement;
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
            TrainMovement trainMovement = TrainMovementMapper.MapFromBody(body);
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
                    TrainMovementStep step = TrainMovementStepMapper.MapFromBody(body);
                    _cacheService.CacheTrainStep(trainId, (string)body.train_service_code, step);
                }
            }
            finally
            {
                if (_cacheService != null)
                    _cacheService.Close();
            }
        }

        private void CacheTrainCancellation(string trainId, dynamic body)
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
                    CancelledTrainMovementStep step = TrainMovementStepMapper.MapFromBody(body, true);
                    _cacheService.CacheTrainCancellation(trainId, (string)body.train_service_code, step);
                }
            }
            finally
            {
                if (_cacheService != null)
                    _cacheService.Close();
            }
        }

        public void Dispose()
        {
        }
    }
}
