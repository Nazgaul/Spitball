using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.ReadServices;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    public class NoUniversityMailProcess : IMailProcess
    {
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IMailComponent m_MailComponent;

        public NoUniversityMailProcess(IZboxReadServiceWorkerRole zboxReadService, IMailComponent mailComponent)
        {
            m_ZboxReadService = zboxReadService;
            m_MailComponent = mailComponent;
        }


        public async Task<bool> ExcecuteAsync(int index, Action<int> progress, CancellationToken token)
        {
            var page = index;
            var needToContinueRun = true;
            while (needToContinueRun)
            {
                var list = new List<Task>();
                needToContinueRun = false;
                var result = await m_ZboxReadService.GetUsersWithoutUniversityAsync(
                        new ViewModel.Queries.Emails.UserWithoutUniversityQuery(page, 100), token);
                foreach (var user in result)
                {
                    needToContinueRun = true;
                    list.Add( m_MailComponent.GenerateAndSendEmailAsync(user.Email,
                        new NoUniversityMailParams(user.Name, new System.Globalization.CultureInfo(user.Culture))));
                }
                await Task.WhenAll(list);
                //m_MailComponent.GenerateAndSendEmailAsync(result.)

                progress(page);
                page++;
            }
            return true;

        }
    }
}
