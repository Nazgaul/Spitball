using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.Emails;
using Zbang.Zbox.ViewModel.Queries.Emails;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    public abstract class BaseMarketingMailProcess : ISchedulerProcess
    {
        private readonly IMailComponent m_MailComponent;
        private readonly ILogger m_Logger;


        protected BaseMarketingMailProcess(IMailComponent mailComponent, ILogger logger)
        {
            m_MailComponent = mailComponent;
            m_Logger = logger;
        }

        protected abstract Task<IEnumerable<MarketingDto>> GetDataAsync(MarketingQuery query, CancellationToken token);

        protected abstract MarketingMailParams BuildMarketingMail(string name, CultureInfo info);

        protected abstract string ServiceName
        {
            get;
        }

        public async Task<bool> ExecuteAsync(int index, Func<int, TimeSpan, Task> progressAsync, CancellationToken token)
        {
            if (progressAsync == null) throw new ArgumentNullException(nameof(progressAsync));
            var page = index;
            var needToContinueRun = true;
            var list = new List<Task>();
            while (needToContinueRun)
            {
                try
                {
                    m_Logger.Info($"{ServiceName} running  mail page {page}");
                    needToContinueRun = false;
                    var pageSize = 100;
                    if (RoleIndexProcessor.IsEmulated)
                    {
                        pageSize = 10;
                    }

                    var query = new MarketingQuery(page, pageSize);
                    var result = await GetDataAsync(query, token).ConfigureAwait(false);

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
                        var marketingMail = BuildMarketingMail(user.Name, culture);
                        list.Add(m_MailComponent.GenerateAndSendEmailAsync(email,
                            marketingMail, token));

                    }
                    await Task.WhenAll(list).ConfigureAwait(false);
                    list.Clear();
                    if (RoleIndexProcessor.IsEmulated)
                    {
                        needToContinueRun = false;
                    }
                }
                catch (Exception ex)
                {
                    m_Logger.Exception(ex);
                }
                page++;
                await progressAsync(page, TimeSpan.FromMinutes(15)).ConfigureAwait(false);

            }
            //await m_MailComponent.GenerateSystemEmailAsync(ServiceName, $"finish to run  with page {page}").ConfigureAwait(false);
            m_Logger.Info($"{ServiceName} finish running  mail page {page}");
            return true;

        }
    }
}
