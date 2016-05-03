using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Zbang.Cloudents.Connect
{
    public class SpitballHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}