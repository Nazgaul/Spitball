using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Cloudents.Connect
{

    public class SpitballHub : Hub
    {
        private readonly Lazy<IQueueProvider> m_QueueProvider;

        public SpitballHub(Lazy<IQueueProvider> queueProvider)
        {
            m_QueueProvider = queueProvider;
        }

        public void Hello()
        {
            Clients.All.hello();
        }

        public void Send(string userId, string message)
        {
            Clients.User(userId).send(message);
        }

        public override Task OnConnected()
        {

            Trace.TraceInformation("connected");
            var user = Context.User.GetUserId();

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Task t = Zbang.Zbox.Infrastructure.Extensions.TaskExtensions.CompletedTask;
            if (Context.User.Identity.IsAuthenticated)
            {
                t = m_QueueProvider.Value.InsertMessageToTranactionAsync(
                      new StatisticsData4(null, Context.User.GetUserId(), DateTime.UtcNow));
            }
            //var user = Context.User.GetUserId();
            var t1 = base.OnDisconnected(stopCalled);
            return Task.WhenAll(t, t1);
        }
    }
}