using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TrainNotifier.Common.Model;
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

        private void HandleSubTrainCommand(UserContextEventArgs context, string trainId, bool subscribe)
        {
            UserContextData uc = _userManager.ActiveUsers[context.UserContext];
            if (uc != null)
            {
                if (subscribe)
                {
                    uc.StateArgs = trainId;
                    uc.State = UserContextState.SubscribeToTrain;
                }
                else
                {
                    uc.State = UserContextState.None;
                }
            }
        }

        private void HandleStanoxCommand(UserContextEventArgs context, string command, string stanoxName)
        {
            CacheServiceClient cacheService = null;
            try
            {
                cacheService = new CacheServiceClient();
                cacheService.Open();
                Stanox stanox;
                if (cacheService.TryGetStanox(stanoxName, out stanox))
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
                if (cacheService != null)
                    cacheService.Close();
            }
        }

        private void CacheActivation(dynamic body)
        {
            TrainMovement trainMovement = TrainMovementMapper.MapFromBody(body);
            CacheServiceClient cacheService = null;
            try
            {
                cacheService = new CacheServiceClient();
                cacheService.Open();
                cacheService.CacheTrainMovement(trainMovement);
            }
            finally
            {
                if (cacheService != null)
                    cacheService.Close();
            }
        }

        private void CacheTrainMovement(string trainId, dynamic body)
        {
            if (string.IsNullOrWhiteSpace(trainId))
                return;

            CacheServiceClient cacheService = null;
            try
            {
                cacheService = new CacheServiceClient();
                cacheService.Open();
                TrainMovementStep step = TrainMovementStepMapper.MapFromBody(body);
                cacheService.CacheTrainStep(trainId, step);
            }
            finally
            {
                if (cacheService != null)
                    cacheService.Close();
            }
        }

        private void CacheTrainCancellation(string trainId, dynamic body)
        {
            if (string.IsNullOrWhiteSpace(trainId))
                return;

            CacheServiceClient cacheService = null;
            try
            {
                cacheService = new CacheServiceClient();
                cacheService.Open();
                CancelledTrainMovementStep step = TrainMovementStepMapper.MapFromBody(body, true);
                cacheService.CacheTrainCancellation(trainId, step);
            }
            finally
            {
                if (cacheService != null)
                    cacheService.Close();
            }
        }

        public void Dispose()
        {
        }
    }
}
