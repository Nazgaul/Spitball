using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Zbang.Zbox.Domain.Common;

namespace Zbang.Cloudents.Connect
{

    public class SpitballHub : Hub
    {
        //private readonly Lazy<IQueueProvider> m_QueueProvider;
        private readonly IZboxWriteService m_WriteService;

        public SpitballHub( IZboxWriteService writeService)
        {
           // m_QueueProvider = queueProvider;
            m_WriteService = writeService;
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

            var user = Context.User.GetUserId();
            m_WriteService.ChangeOnlineStatus(new Zbox.Domain.Commands.ChangeUserOnlineStatusCommand(user, true));
            //ChangeOnlineStatus
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var user = Context.User.GetUserId();
            m_WriteService.ChangeOnlineStatus(new Zbox.Domain.Commands.ChangeUserOnlineStatusCommand(user, false));
            //Task t = Zbang.Zbox.Infrastructure.Extensions.TaskExtensions.CompletedTask;
            //if (Context.User.Identity.IsAuthenticated)
            //{
            //    t = m_QueueProvider.Value.InsertMessageToTranactionAsync(
            //          new StatisticsData4(null, Context.User.GetUserId(), DateTime.UtcNow));
            //}
            ////var user = Context.User.GetUserId();
            //var t1 = base.OnDisconnected(stopCalled);
            return base.OnDisconnected(stopCalled);
            
            //return Task.WhenAll(t, t1);
        }
    }
}