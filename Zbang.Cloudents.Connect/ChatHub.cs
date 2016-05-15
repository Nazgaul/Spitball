using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Zbang.Cloudents.Connect
{
    [Authorize]
    public class ChatHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        public void Send(string name, string message)
        {
            Clients.All.broadcastMessage(name, message);
        }

        public override Task OnConnected()
        {
            var user = Context.User.GetUserId();
            return base.OnConnected();
        }
    }
}