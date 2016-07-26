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

        public async Task<bool> ExecuteAsync(int index, Func<int, Task> progressAsync, CancellationToken token)
        {
            var page = index;
            var needToContinueRun = true;
            var list = new List<Task>();
            while (needToContinueRun)
            {
                TraceLog.WriteInfo($"{ServiceName} running  mail page {page}");
                needToContinueRun = false;
                int pageSize = 100;
                if (RoleIndexProcessor.IsEmulated)
                {
                    pageSize = 10;
                }

                var query = new MarketingQuery(page, pageSize);
                var result = await GetDataAsync(query, token);

                foreach (var user in result)
                {
                    needToContinueRun = true;
                    var email = user.Email;
                    if (RoleIndexProcessor.IsEmulated)
                    {
                        email = "ram@cloudents.com";
                    }

                    var culture = string.IsNullOrEmpty(user.Culture)
                        ? new CultureInfo("en-US")
                        : new CultureInfo(user.Culture);
                    var markertingMail = BuildMarkertingMail(user.Name, culture);
                    list.Add(m_MailComponent.GenerateAndSendEmailAsync(email,
                        markertingMail, token));

                }
                await Task.WhenAll(list);
                list.Clear();
                page++;
                if (RoleIndexProcessor.IsEmulated)
                {
                    needToContinueRun = false;
                }
                await progressAsync(page);

            }
            await m_MailComponent.GenerateSystemEmailAsync(ServiceName, $"finish to run  with page {page}");
            TraceLog.WriteInfo($"{ServiceName} finish running  mail page {page}");
            return true;

        }
    }
}
