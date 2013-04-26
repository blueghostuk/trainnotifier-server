using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using TrainNotifier.Common.Exceptions;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.PPM;
using TrainNotifier.Common.Model.Schedule;
using TrainNotifier.Common.Model.VSTP;
using TrainNotifier.Service;

namespace TrainNotifier.Console.WebSocketServer
{
    internal sealed class CacheController : IDisposable
    {
        private readonly UserManager _userManager;
        private readonly TiplocRepository _tiplocRepository = new TiplocRepository();
        private readonly ScheduleRepository _scheduleRepository = new ScheduleRepository();
        private readonly LiveTrainRepository _liveTrainRepository = new LiveTrainRepository();
        private readonly ICollection<TiplocCode> _tiplocs;

        private static readonly bool _saveTdData = bool.Parse(ConfigurationManager.AppSettings["SaveTrainDescriberData"]);

        private static readonly object _cacheLock = new object();

        public CacheController(NMSWrapper nmsWrapper, WebSocketServerWrapper wssWrapper, UserManager userManager)
        {
            _tiplocs = _tiplocRepository.GetTiplocs().ToList();

            _userManager = userManager;
            nmsWrapper.FeedDataRecieved += (s, f) =>
            {
               Task.Run(() =>
                    {
                        switch (f.Source)
                        {
                            case Common.Feed.TrainMovement:
                                ICollection<ITrainData> data = new List<ITrainData>(32);
                                foreach (var message in f.Data)
                                {
                                    switch (byte.Parse((string)message.header.msg_type))
                                    {
                                        // activation
                                        case 1:
                                            data.Add(CacheActivation(message.body));
                                            break;

                                        // cancellation
                                        case 2:
                                            data.Add(CacheTrainCancellation((string)message.body.train_id, message.body));
                                            break;

                                        // train movement
                                        case 3:
                                            data.Add(CacheTrainMovement((string)message.body.train_id, message.body));
                                            break;

                                        // unidentified train
                                        case 4:
                                            break;

                                        // train reinstatement
                                        case 5:
                                            data.Add(CacheTrainReinstatement((string)message.body.train_id, message.body));
                                            break;

                                        // train change of origin
                                        case 6:
                                            data.Add(CacheChangeOfOrigin((string)message.body.train_id, message.body));
                                            break;

                                        // train change of identity
                                        case 7:
                                            break;

                                        // train change of location
                                        case 8:
                                            break;
                                    }
                                }
                                data = data.Where(d => d != null)
                                    .ToList();
                                if (data.Any())
                                {
                                    lock (_cacheLock)
                                    {
                                        CacheServiceClient cacheService = null;
                                        try
                                        {
                                            cacheService = new CacheServiceClient();
                                            cacheService.Open();
                                            cacheService.CacheTrainData(data);
                                        }
                                        finally
                                        {
                                            try
                                            {
                                                if (cacheService != null)
                                                    cacheService.Close();
                                            }
                                            catch (CommunicationObjectFaultedException e)
                                            {
                                                Trace.TraceError("Error Closing Cache Connection: {0}", e);
                                            }
                                        }
                                    }
                                }
                                break;
                            case Common.Feed.TrainDescriber:
                                if (_saveTdData)
                                {
                                    ICollection<TrainDescriber> tdData = new List<TrainDescriber>(32);
                                    foreach (var message in f.Data)
                                    {
                                        tdData.Add(TrainDescriberMapper.MapFromBody(message));
                                    }
                                    tdData = tdData.Where(d => d != null)
                                        .ToList();
                                    if (tdData.Any())
                                    {
                                        CacheServiceClient cacheService = null;
                                        try
                                        {
                                            cacheService = new CacheServiceClient();
                                            cacheService.Open();
                                            cacheService.CacheTrainDescriberData(tdData);
                                        }
                                        catch (Exception e)
                                        {
                                            Trace.TraceError("Error In Cache Connection: {0}", e);
                                        }
                                        finally
                                        {
                                            try
                                            {
                                                if (cacheService != null)
                                                    cacheService.Close();
                                            }
                                            catch (CommunicationObjectFaultedException e)
                                            {
                                                Trace.TraceError("Error Closing Cache Connection: {0}", e);
                                            }
                                        }
                                    }
                                }
                                break;

                            case Common.Feed.VSTP:

                                ScheduleTrain train = null;
                                try
                                {
                                    train = VSTPMapper.ParseJsonVSTP(f.Data, _tiplocs);
                                }
                                catch (TiplocNotFoundException tnfe)
                                {
                                    Trace.TraceError("Could not add VSTP: {0}", tnfe);
                                }
                                catch (Exception e)
                                {
                                    Trace.TraceError("Could not add VSTP: {0}", e);
                                }

                                if (train != null)
                                {
                                    CacheServiceClient cacheService = null;
                                    try
                                    {
                                        cacheService = new CacheServiceClient();
                                        cacheService.Open();
                                        cacheService.CacheVSTPSchedule(train);
                                    }
                                    catch (Exception e)
                                    {
                                        Trace.TraceError("Error In Cache Connection: {0}", e);
                                    }
                                    finally
                                    {
                                        try
                                        {
                                            if (cacheService != null)
                                                cacheService.Close();
                                        }
                                        catch (CommunicationObjectFaultedException e)
                                        {
                                            Trace.TraceError("Error Closing Cache Connection: {0}", e);
                                        }
                                    }
                                }

                                break;

                            case Common.Feed.RtPPM:
                                RtppmData ppmData = null;
                                try
                                {
                                    ppmData = PPMJsonMapper.ParsePPMData(f.Data.RTPPMDataMsgV1.RTPPMData);
                                }
                                catch (Exception e)
                                {
                                    Trace.TraceError("Could not add PPM Data: {0}", e);
                                }
                                if (ppmData != null)
                                {
                                    CacheServiceClient cacheService = null;
                                    try
                                    {
                                        cacheService = new CacheServiceClient();
                                        cacheService.Open();
                                        cacheService.CachePPMData(ppmData);
                                    }
                                    catch (Exception e)
                                    {
                                        Trace.TraceError("Error In Cache Connection: {0}", e);
                                    }
                                    finally
                                    {
                                        try
                                        {
                                            if (cacheService != null)
                                                cacheService.Close();
                                        }
                                        catch (CommunicationObjectFaultedException e)
                                        {
                                            Trace.TraceError("Error Closing Cache Connection: {0}", e);
                                        }
                                    }
                                }
                                break;
                        }
                    });
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
                        case "substanox":
                            HandleSubStanoxCommand(context, args, true);
                            break;
                        case "unsubstanox":
                            HandleSubStanoxCommand(context, args, false);
                            break;
                    }
                }
            };
        }

        private void HandleSubStanoxCommand(UserContextEventArgs context, string stanox, bool subscribe)
        {
            UserContextData uc = _userManager.ActiveUsers[context.UserContext];
            if (uc != null)
            {
                if (subscribe)
                {
                    uc.StateArgs = stanox;
                    uc.State = UserContextState.SubscribeToStanox;
                }
                else
                {
                    uc.State = UserContextState.None;
                }
            }
        }

        private void HandleSubTrainCommand(UserContextEventArgs context, string trainId, bool subscribe)
        {
            UserContextData uc = _userManager.ActiveUsers[context.UserContext];
            if (uc != null)
            {
                if (subscribe)
                {
                    uc.StateArgs = trainId;
                    uc.HeadCode = _liveTrainRepository.GetHeadCode(trainId);
                    uc.State = UserContextState.SubscribeToTrain;
                    Trace.TraceInformation("User {0} subscribed to train {1} - {2}", context.UserContext.ClientAddress, uc.HeadCode, trainId);
                }
                else
                {
                    uc.State = UserContextState.None;
                }
            }
        }

        private TrainMovement CacheActivation(dynamic body)
        {
            return TrainMovementMapper.MapFromBody(body);
        }

        private TrainMovementStep CacheTrainMovement(string trainId, dynamic body)
        {
            if (string.IsNullOrWhiteSpace(trainId))
                return null;

            TrainMovementStep step = TrainMovementStepMapper.MapFromBody(body);
            step.TrainId = trainId;
            return step;
        }

        private TrainReinstatement CacheTrainReinstatement(string trainId, dynamic body)
        {
            if (string.IsNullOrWhiteSpace(trainId))
                return null;

            TrainReinstatement step = TrainReinstatementMapper.MapFromBody(body);
            step.TrainId = trainId;
            return step;
        }

        private TrainChangeOfOrigin CacheChangeOfOrigin(string trainId, dynamic body)
        {
            if (string.IsNullOrWhiteSpace(trainId))
                return null;

            TrainChangeOfOrigin step = TrainChangeOfOriginMapper.MapFromBody(body);
            step.TrainId = trainId;
            return step;
        }

        private CancelledTrainMovementStep CacheTrainCancellation(string trainId, dynamic body)
        {
            if (string.IsNullOrWhiteSpace(trainId))
                return null;

            CancelledTrainMovementStep step = TrainMovementStepMapper.MapFromBody(body, true);
            step.TrainId = trainId;
            return step;
        }

        public void Dispose()
        {
        }
    }
}
