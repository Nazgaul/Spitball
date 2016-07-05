using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.DomainProcess
{
    public class ProcessConnections : IFileProcess
    {
        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly IMailComponent m_MailComponent;

        public ProcessConnections(IZboxWriteService zboxWriteService, IMailComponent mailComponent)
        {
            m_ZboxWriteService = zboxWriteService;
            m_MailComponent = mailComponent;
        }

        public async Task<bool> ExecuteAsync(FileProcess data, CancellationToken token)
        {
            var parameters = data as SignalrConnectionsData;
            if (parameters == null) return true;
            var command = new ManageConnectionsCommand(parameters.ConnectionIds);
            var result = m_ZboxWriteService.ManageConnections(command);

            try
            {
                var proxy = await SignalrClient.GetProxyAsync();
                await proxy.Invoke("Disconnect", result.UserIds);
            }
            catch (Exception ex)
            {
                await m_MailComponent.GenerateSystemEmailAsync("signalR error", ex.Message);
                TraceLog.WriteError("on signalr update image", ex);
            }
            //var connection = m_Heartbeat.GetConnections().FirstOrDefault(w => w.ConnectionId == connectionId);
            //if (connection != null)
            //{
            //    tasks.Add(connection.Disconnect());
            //}
            return true;

        }
    }
}
