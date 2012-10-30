using Alchemy.Classes;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace NetworkRailDownloader.Console
{
    internal sealed class UserManager
    {
        private readonly IDictionary<UserContext, string> _activeUsers = new ConcurrentDictionary<UserContext, string>();

        public IDictionary<UserContext, string> ActiveUsers
        {
            get { return _activeUsers; }
        }

        public UserManager(WebSocketServerWrapper webSocketServer)
        {
            webSocketServer.OnConnected += (s, context) =>
            {
                Trace.TraceInformation("Connection From : {0}", context.UserContext.ClientAddress);
                _activeUsers.Add(context.UserContext, string.Empty);
            };

            webSocketServer.OnDisconnect += (s, context) =>
            {
                Trace.TraceInformation("{0} disconnected", context.UserContext.ClientAddress);
                _activeUsers.Remove(context.UserContext);
            };

            webSocketServer.OnReceive += (s, context) =>
            {
                Trace.TraceInformation("Received {0} from {1}", context.UserContext.DataFrame.ToString(), context.UserContext.ClientAddress);
                _activeUsers[context.UserContext] = context.UserContext.DataFrame.ToString();
            };
        }
    }
}
