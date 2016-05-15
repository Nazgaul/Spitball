using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Zbang.Cloudents.Connect
{
   
    public class SpitballHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        public override Task OnConnected()
        {
            
            Trace.TraceInformation("connected");
            var user = Context.User.GetUserId();

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var user = Context.User.GetUserId();
            return base.OnDisconnected(stopCalled);
        }
    }
}