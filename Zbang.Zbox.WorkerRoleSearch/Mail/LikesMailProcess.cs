using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Cloudents.Core.Interfaces;
using Zbang.Zbox.ReadServices;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    public class LikesMailProcess : ISchedulerProcess
    {
        private readonly IMailComponent _mailComponent;
        private readonly IZboxReadServiceWorkerRole _zboxReadService;
        private const string ServiceName = nameof(LikesMailProcess);
        private readonly ILogger _logger;

        public LikesMailProcess(IZboxReadServiceWorkerRole zboxReadService, IMailComponent mailComponent, ILogger logger)
        {
            _zboxReadService = zboxReadService;
            _mailComponent = mailComponent;
            _logger = logger;
        }

        public async Task<bool> ExecuteAsync(int index, Func<int,TimeSpan, Task> progressAsync, CancellationToken token)
        {
            var list = new List<Task>();
            var result = await _zboxReadService.GetLikesDataAsync(token).ConfigureAwait(false);
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
                list.Add(_mailComponent.GenerateAndSendEmailAsync(email,
                    marketingMail, token));
            }
            await Task.WhenAll(list).ConfigureAwait(false);
            _logger.Info($"{ServiceName} finish running  mail");
            return true;
        }
    }
}
