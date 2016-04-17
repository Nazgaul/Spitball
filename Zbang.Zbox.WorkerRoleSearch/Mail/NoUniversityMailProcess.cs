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
            var list = new List<Task>();
            while (needToContinueRun)
            {
                
                needToContinueRun = false;
                var result = await m_ZboxReadService.GetUsersWithoutUniversityAsync(
                        new ViewModel.Queries.Emails.UserWithoutUniversityQuery(page, 100), token);
                foreach (var user in result)
                {
                    needToContinueRun = true;
                    var email = user.Email;
#if DEBUG
                    email = "eidan@spitball.co";
#endif
                    var culture = string.IsNullOrEmpty(user.Culture)
                        ? Thread.CurrentThread.CurrentCulture
                        : new System.Globalization.CultureInfo(user.Culture);

                    list.Add( m_MailComponent.GenerateAndSendEmailAsync(email,
                        new NoUniversityMailParams(user.Name, culture)));
                }
                await Task.WhenAll(list);
                list.Clear();
                page++;
                progress(page);
                
            }
            return true;

        }
    }
}
