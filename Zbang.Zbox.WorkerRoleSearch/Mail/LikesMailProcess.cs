using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    public class LikesMailProcess : ISchedulerProcess
    {
        private readonly IMailComponent m_MailComponent;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private const string ServiceName = nameof(LikesMailProcess);
        private readonly ILogger m_Logger;

        public LikesMailProcess(IZboxReadServiceWorkerRole zboxReadService, IMailComponent mailComponent, ILogger logger)
        {
            m_ZboxReadService = zboxReadService;
            m_MailComponent = mailComponent;
            m_Logger = logger;
        }

        public async Task<bool> ExecuteAsync(int index, Func<int,TimeSpan, Task> progressAsync, CancellationToken token)
        {
            var list = new List<Task>();
            var result = await m_ZboxReadService.GetLikesDataAsync(token).ConfigureAwait(false);
            foreach (var user in result.GroupBy(g => g.Email))
            {
                var email = user.Key;
                if (RoleIndexProcessor.IsEmulated)
                {
                    email = "ram@cloudents.com";
                }
                var userData = user.First();
                var culture = string.IsNullOrEmpty(userData.Culture)
                    ? new CultureInfo("en-US")
                    : new CultureInfo(userData.Culture);
                var marketingMail = new LikesMailParams(culture, userData.Name, user.Select(s => new LikeData
                {
                    OnLikeText = s.OnElement,
                    UserName = s.LikePersonName,
                    Type = s.LikeType
                }));
                list.Add(m_MailComponent.GenerateAndSendEmailAsync(email,
                    marketingMail, token));
            }
            await Task.WhenAll(list).ConfigureAwait(false);
            m_Logger.Info($"{ServiceName} finish running  mail");
            return true;
        }
    }
}
