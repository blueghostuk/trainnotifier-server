using Alchemy.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainNotifier.Common;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.NMS;

namespace TrainNotifier.Console.WebSocketServer
{
    internal sealed class NMSWrapper
    {
        private readonly UserManager _userManager;
        private readonly IDownloader _nmsDownloader;

        public event EventHandler<FeedEventArgs> FeedDataRecieved;

        public NMSWrapper(UserManager userManager)
        {
            _userManager = userManager;
            _nmsDownloader = new NMSConnector();
        }

        public Task Start()
        {
            _nmsDownloader.FeedDataRecieved += (src, feedData) =>
            {
                dynamic evtData = JsonConvert.DeserializeObject<dynamic>(feedData.Data);
                Parallel.ForEach(_userManager.ActiveUsers
                    .Where(u => u.Value.State == UserContextState.SubscribeToRawFeed), uc => SendData(uc, evtData as IEnumerable<dynamic>));

                Parallel.ForEach(_userManager.ActiveUsers
                    .Where(u => u.Value.State == UserContextState.SubscribeToTrain)
                    .Where(u => DataContainsTrain(evtData, u.Value.StateArgs)), uc => SendTrainData(uc, evtData as IEnumerable<dynamic>));

                var eh = FeedDataRecieved;
                if (null != eh)
                    eh(this, new FeedEventArgs(evtData));
            };

            return Task.Run(() => _nmsDownloader.SubscribeToFeed(Feed.TrainMovement));
        }

        public void Stop()
        {
            _nmsDownloader.Quit();
        }

        private bool DataContainsTrain(dynamic evtData, string trainId)
        {
            if (string.IsNullOrWhiteSpace(trainId))
                return true;

            foreach (dynamic evt in evtData)
            {
                if (evt.body.train_id == trainId)
                    return true;
            }
            return false;
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
    }

    public sealed class FeedEventArgs : EventArgs
    {
        public readonly dynamic Data;

        public FeedEventArgs(dynamic data)
        {
            Data = data;
        }
    }
}
