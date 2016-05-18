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
    public class LikesMailProcess : IMailProcess
    {
        private readonly IMailComponent m_MailComponent;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private const string ServiceName = "LikesMailProcess";

        public LikesMailProcess(IZboxReadServiceWorkerRole zboxReadService, IMailComponent mailComponent)
        {
            m_ZboxReadService = zboxReadService;
            m_MailComponent = mailComponent;
        }


        public async Task<bool> ExecuteAsync(int index, Func<int, Task> progressAsync, CancellationToken token)
        {
            //await m_MailComponent.GenerateSystemEmailAsync(ServiceName, " starting to run ");
            var list = new List<Task>();
            TraceLog.WriteInfo($"{ServiceName} running  mail ");
            var result = await m_ZboxReadService.GetLikesDataAsync(token);
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
                var markertingMail = new LikesMailParams(culture, userData.Name, user.Select(s => new LikeData
                {
                    OnLikeText = s.OnElement,
                    UserName = s.LikePersonName,
                    Type = s.LikeType
                }));
                list.Add(m_MailComponent.GenerateAndSendEmailAsync(email,
                    markertingMail, token));
            }
            await Task.WhenAll(list);
            await m_MailComponent.GenerateSystemEmailAsync(ServiceName, $"finish to run to people {list.Count}");
            TraceLog.WriteInfo($"{ServiceName} finish running  mail");
            return true;
        }
    }
}
