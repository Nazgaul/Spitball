using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateUnsubscribeList : IJob
    {
        private readonly IMailComponent m_MailComponent;
        private readonly IZboxWorkerRoleService m_ZboxWorkerRoleService;
        private DateTime m_DateTime;

        public UpdateUnsubscribeList(IMailComponent mailComponent, IZboxWorkerRoleService zboxWorkerRoleService)
        {
            m_MailComponent = mailComponent;
            m_ZboxWorkerRoleService = zboxWorkerRoleService;
            m_DateTime = new DateTime(2016, 4, 17, 0, 0, 0, DateTimeKind.Utc);
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            if (RoleIndexProcessor.GetIndex() != 0)
            {
                return;
            }
            
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    //var needToContinueRun = true;
                    TraceLog.WriteInfo("update unsubscribe list");
                    var page = 0;
                    while (true)
                    {
                       
                        var result =
                            (await m_MailComponent.GetUnsubscribesAsync(m_DateTime, page++, cancellationToken)).ToList();
                        if (result.Count == 0)
                        {
                            break;
                        }
                        m_ZboxWorkerRoleService.UpdateUserFromUnsubscribe(
                            new Domain.Commands.UnsubscribeUsersFromEmailCommand(result));
                    }
                    TraceLog.WriteInfo("update unsubscribe list complete");
                    m_DateTime = DateTime.UtcNow.AddDays(-10);
                    await Task.Delay(TimeSpan.FromDays(1), cancellationToken);
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("unsubscribe list ", ex);
                }


            }
        }

        public void Stop()
        {
        }
    }
}
