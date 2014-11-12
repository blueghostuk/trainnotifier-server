﻿using Alchemy.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrainNotifier.Common;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.NMS;

namespace TrainNotifier.Console.WebSocketServer
{
    internal sealed class NMSWrapper
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly UserManager _userManager;
        private readonly INMSConnector _nmsDownloader;

        public event EventHandler<FeedEventArgs> FeedDataRecieved;

        public NMSWrapper(UserManager userManager, CancellationTokenSource cancellationTokenSource)
        {
            _userManager = userManager;
            _cancellationTokenSource = cancellationTokenSource;
            _nmsDownloader = new NMSConnector(_cancellationTokenSource);
        }

        public Task Start()
        {
            _nmsDownloader.TrainDataRecieved += (src, feedData) =>
            {
                // run this as a task to return to callee quicker
                Task.Run(() =>
                {
                    dynamic evtData = JsonConvert.DeserializeObject<dynamic>(feedData.Data);

                    switch (feedData.FeedSource)
                    {
                        case Feed.TrainMovement:

                            Parallel.ForEach(_userManager.ActiveUsers
                                .Where(u => u.Value.State == UserContextState.SubscribeToFeed), uc => SendData(uc, evtData as IEnumerable<dynamic>));

                            Parallel.ForEach(_userManager.ActiveUsers
                                .Where(u => u.Value.State == UserContextState.SubscribeToTrain)
                                .Where(u => DataContainsTrain(evtData, u.Value.StateArgs)), uc => SendTrainData(uc, evtData as IEnumerable<dynamic>));

                            Parallel.ForEach(_userManager.ActiveUsers
                                .Where(u => u.Value.State == UserContextState.SubscribeToStanox)
                                .Where(u => DataContainsStanox(evtData, u.Value.StateArgs)), uc => SendStanoxData(uc, evtData as IEnumerable<dynamic>));

                            break;
                        case Feed.TrainDescriber:

                            if (_userManager.ActiveUsers.Any())
                            {
                                ICollection<TrainDescriber> tdData = new List<TrainDescriber>(32);
                                foreach (var message in evtData)
                                {
                                    tdData.Add(TrainDescriberMapper.MapFromBody(message));
                                }
                                tdData = tdData.Where(d => d != null)
                                    .Where(d => !string.IsNullOrEmpty(d.Description))
                                    .ToList();
                                Parallel.ForEach(_userManager.ActiveUsers
                                    .Where(u => u.Value.State == UserContextState.SubscribeToTrain)
                                    .Where(u => !string.IsNullOrEmpty(u.Value.HeadCode))
                                    .Where(u => DataContainsTrainDescriber(tdData, u.Value.HeadCode)), uc => SendTrainDescriberData(uc, tdData));
                            }

                            break;
                    }

                    var eh = FeedDataRecieved;
                    if (null != eh)
                        eh(this, new FeedEventArgs(feedData.FeedSource, evtData));
                }, _cancellationTokenSource.Token);
            };

            return Task.Run(() => _nmsDownloader.SubscribeToFeeds(), _cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _nmsDownloader.Quit();
            _cancellationTokenSource.Cancel();
        }

        private bool DataContainsStanox(dynamic evtData, string stanox)
        {
            if (string.IsNullOrWhiteSpace(stanox))
                return false;
            foreach (dynamic evt in evtData)
            {
                if (evt.body.loc_stanox == stanox)
                    return true;
            }
            return false;
        }

        private bool DataContainsTrain(dynamic evtData, string trainId)
        {
            if (string.IsNullOrWhiteSpace(trainId))
                return false;

            foreach (dynamic evt in evtData)
            {
                if (evt.body.train_id == trainId)
                    return true;
            }
            return false;
        }

        private bool DataContainsTrainDescriber(IEnumerable<TrainDescriber> tdData, string headCode)
        {
            return tdData
                .Where(td => !string.IsNullOrEmpty(td.Description))
                .Any(td => td.Description.Equals(headCode, StringComparison.InvariantCultureIgnoreCase));
        }

        private static bool DoSendData(dynamic evtData, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
                return true;

            foreach (dynamic evt in evtData)
            {
                if (evt.body.loc_stanox == filter)
                    return true;
            }
            return false;
        }

        private static void SendStanoxData(KeyValuePair<UserContext, UserContextData> uc, IEnumerable<dynamic> evtData)
        {
            var data = evtData
                .Where(e => e.body.loc_stanox == uc.Value.StateArgs)
                .Select(e => e);

            uc.Key.Send(JsonConvert.SerializeObject(new CommandResponse<IEnumerable<dynamic>>
            {
                Command = "substanoxupdate",
                Args = uc.Value.StateArgs,
                Response = data
            }));
        }

        private static void SendData(KeyValuePair<UserContext, UserContextData> uc, IEnumerable<dynamic> evtData)
        {
            uc.Key.Send(JsonConvert.SerializeObject(evtData.Select(e => e.body)));
        }

        private void SendTrainData(KeyValuePair<UserContext, UserContextData> uc, IEnumerable<dynamic> evtData)
        {
            var data = evtData
                .Where(e => e.body.train_id == uc.Value.StateArgs)
                .Select(e => TrainMovementStepMapper.MapFromBody(e.body))
                .Cast<TrainMovementStep>()
                .ToList();

            uc.Key.Send(JsonConvert.SerializeObject(new CommandResponse<IEnumerable<TrainMovementStep>>
            {
                Command = "subtrainupdate",
                Args = uc.Value.StateArgs,
                Response = data
            }));
        }

        private void SendTrainDescriberData(KeyValuePair<UserContext, UserContextData> uc, IEnumerable<TrainDescriber> tdData)
        {
            var data = tdData
                .Where(td => !string.IsNullOrEmpty(td.Description))
                .Where(td => td.Description.Equals(uc.Value.HeadCode, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            uc.Key.Send(JsonConvert.SerializeObject(new CommandResponse<IEnumerable<TrainDescriber>>
            {
                Command = "subtrainupdate-berth",
                Args = uc.Value.StateArgs,
                Response = data
            }));
        }
    }

    public sealed class FeedEventArgs : EventArgs
    {
        public readonly Feed Source;
        public readonly dynamic Data;

        public FeedEventArgs(Feed source, dynamic data)
        {
            Source = source;
            Data = data;
        }
    }
}
