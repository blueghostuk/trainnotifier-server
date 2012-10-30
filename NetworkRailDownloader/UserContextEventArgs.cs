using Alchemy.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkRailDownloader.Console
{
    internal sealed class UserContextEventArgs : EventArgs
    {
        public readonly UserContext UserContext;

        public UserContextEventArgs(UserContext userContext)
        {
            UserContext = userContext;
        }
    }
}
