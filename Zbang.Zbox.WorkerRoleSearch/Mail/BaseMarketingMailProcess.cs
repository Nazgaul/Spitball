using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.Emails;
using Zbang.Zbox.ViewModel.Queries.Emails;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    public abstract class BaseMarketingMailProcess : IMailProcess
    {
        private readonly IMailComponent m_MailComponent;

        protected BaseMarketingMailProcess(IMailComponent mailComponent)
        {
            m_MailComponent = mailComponent;
        }

        protected abstract Task<IEnumerable<MarketingDto>> GetDataAsync(MarketingQuery query, CancellationToken token);

        protected abstract MarketingMailParams BuildMarkertingMail(string name, CultureInfo info);

        protected abstract string ServiceName
        {
            get;
        }

        public async Task<bool> ExcecuteAsync(int index, Action<int> progress, CancellationToken token)
        {
          await  m_MailComponent.GenerateSystemEmailAsync($"starting to run {ServiceName}");
            var page = index;
            var needToContinueRun = true;
            var list = new List<Task>();
            while (needToContinueRun)
            {
                TraceLog.WriteInfo($"running {ServiceName} mail page {page}");
                needToContinueRun = false;
                int pageSize = 100;
                if (RoleEnvironment.IsEmulated)
                {
                    pageSize = 10;
                }

                var query = new MarketingQuery(page, pageSize);
                var result = await GetDataAsync(query, token);
                
                foreach (var user in result)
                {
                    needToContinueRun = true;
                    var email = user.Email;
                    if (RoleEnvironment.IsEmulated)
                    {
                        email = "ram@cloudents.com";
                    }

                    var culture = string.IsNullOrEmpty(user.Culture)
                        ? Thread.CurrentThread.CurrentCulture
                        : new CultureInfo(user.Culture);
                    var markertingMail = BuildMarkertingMail(user.Name, culture);
                    list.Add(m_MailComponent.GenerateAndSendEmailAsync(email,
                        markertingMail, token));
                }
                await Task.WhenAll(list);
                list.Clear();
                page++;
                progress(page);

            }
            await m_MailComponent.GenerateSystemEmailAsync($"finish to run {ServiceName} with page {page}");
            TraceLog.WriteInfo($"finish running {ServiceName} mail page {index}");
            return true;

        }
    }
}
