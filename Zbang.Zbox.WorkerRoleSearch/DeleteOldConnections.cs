using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class DeleteOldConnections : IJob
    {
        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly IMailComponent m_MailComponent;

        public DeleteOldConnections(IZboxWriteService zboxWriteService, IMailComponent mailComponent)
        {
            m_ZboxWriteService = zboxWriteService;
            m_MailComponent = mailComponent;
        }
        public string Name => nameof(DeleteOldConnections);

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var command = new RemoveOldConnectionCommand();
                try
                {

                    m_ZboxWriteService.RemoveOldConnections(command);
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError(ex);
                }
                if (command.UserIds != null && command.UserIds.Any())
                {
                    
                    try
                    {
                        var proxy = await SignalrClient.GetProxyAsync().ConfigureAwait(false);
                        foreach (var userId in command.UserIds)
                        {
                            await proxy.Invoke("Offline", userId).ConfigureAwait(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        await m_MailComponent.GenerateSystemEmailAsync("signalR error", ex.Message).ConfigureAwait(false);
                        TraceLog.WriteError("on signalr disconnect process", ex);
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken).ConfigureAwait(false);
            }
        }

    }
}
