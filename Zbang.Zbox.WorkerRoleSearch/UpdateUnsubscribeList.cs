using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Mail;

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
            m_DateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            
            while (!cancellationToken.IsCancellationRequested)
            {
                //var needToContinueRun = true;
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
                m_DateTime = DateTime.UtcNow.AddDays(-10);
                await Task.Delay(TimeSpan.FromDays(1), cancellationToken);


            }
        }
    }
}
