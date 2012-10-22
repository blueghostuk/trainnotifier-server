using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkRailDownloader.Common;
using NetworkRailDownloader.Downloader;
using Alchemy.Classes;
using Alchemy;
using System.Net;
using System.Diagnostics;
using Essential.Diagnostics;

namespace NetworkRailDownloader.Console
{
    class Program
    {
        private static readonly ICollection<UserContext> _activeUsers = new HashSet<UserContext>();

        static void Main(string[] args)
        {
            SetupTrace();

            System.Console.WriteLine("Press ENTER key to disconnect");

            WebSocketServer webSocketServer = new WebSocketServer(81, IPAddress.Any)
            {
                OnConnected = new OnEventDelegate(Program.OnWsConnected),
                OnDisconnect = new OnEventDelegate(Program.OnWsDisconnect),
                TimeOut = TimeSpan.FromMinutes(5)
            };
            webSocketServer.Start();

            IDownloader downloader = (IDownloader)new NMSConnector();

            downloader.FeedDataRecieved += (src, feedData) => 
                Parallel.ForEach(_activeUsers, uc => uc.Send(feedData.Data, false, false));

            Task.Run(() => downloader.SubscribeToFeed(Feed.TrainMovement));

            System.Console.ReadLine();

            downloader.Quit();
            webSocketServer.Stop();

            System.Console.WriteLine("Press ENTER key to quit");
            System.Console.ReadLine();
        }

        private static void SetupTrace()
        {
            Trace.Listeners.Add(new ColoredConsoleTraceListener
            {
                Template = "{DateTime:HH':'mm':'ssZ} [{Thread}] {EventType} {Source} {Id}: {Message}{Data}"
            });
        }

        private static void OnWsConnected(UserContext uc)
        {
            System.Console.WriteLine("Connection From : {0}", uc.ClientAddress);
            _activeUsers.Add(uc);
        }

        private static void OnWsDisconnect(UserContext uc)
        {
            System.Console.WriteLine("{0} disconnected", uc.ClientAddress);
            _activeUsers.Remove(uc);
        }
    }
}
