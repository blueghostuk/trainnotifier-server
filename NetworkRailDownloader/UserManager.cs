using Alchemy.Classes;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace NetworkRailDownloader.Common
{
    internal sealed class UserManager
    {
        private readonly IDictionary<UserContext, UserContextData> _activeUsers = new ConcurrentDictionary<UserContext, UserContextData>();

        public IDictionary<UserContext, UserContextData> ActiveUsers
        {
            get { return _activeUsers; }
        }

        public UserManager(WebSocketServerWrapper webSocketServer)
        {
            webSocketServer.OnConnected += (s, context) =>
            {
                Trace.TraceInformation("Connection From : {0}", context.UserContext.ClientAddress);
                AddNewUser(context.UserContext);
            };

            webSocketServer.OnDisconnect += (s, context) =>
            {
                Trace.TraceInformation("{0} disconnected", context.UserContext.ClientAddress);
                RemoveUser(context.UserContext);
            };

            webSocketServer.OnReceive += (s, context) =>
            {
                Trace.TraceInformation("Received {0} from {1}", context.UserContext.DataFrame.ToString(), context.UserContext.ClientAddress);
                string command = context.UserContext.DataFrame.ToString();
                UserContextData data = null;
                if (!_activeUsers.TryGetValue(context.UserContext, out data))
                {
                    data = AddNewUser(context.UserContext);
                }
                switch (command)
                {
                    case "subscribe":
                        data.State = UserContextState.SubscribeToFeed;
                        break;
                    case "unsubscribe":
                        data.State = UserContextState.None;
                        break;
                    default:
                        data.StateArgs = command;
                        break;
                }
                data.LastRequest = command;
            };
        }

        private void RemoveUser(UserContext context)
        {
            _activeUsers.Remove(context);
        }

        private UserContextData AddNewUser(UserContext context)
        {
            UserContextData data = new UserContextData();
            _activeUsers.Add(context, data);
            return data;
        }
    }

    public sealed class UserContextData
    {
        public UserContextState State { get; set; }
        public string LastRequest { get; set; }
        public string StateArgs { get; set; }

        public UserContextData()
        {
            StateArgs = string.Empty;
        }
    }

    public enum UserContextState
    {
        None,
        SubscribeToFeed
    }

}
