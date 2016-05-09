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