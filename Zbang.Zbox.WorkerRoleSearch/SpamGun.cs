using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Storage;
using Cloudents.Core.Interfaces;
using Zbang.Zbox.ReadServices;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class SpamGun : ISchedulerProcess
    {
        private readonly IMailComponent _mailComponent;
        private readonly IZboxReadServiceWorkerRole _zboxReadService;
        private readonly IZboxWorkerRoleService _zboxWriteService;
        private readonly ILogger _logger;

        private readonly IBlobProvider2<SpamGunContainerName> _blobProvider;

        private readonly int _numberOfEmailPerSession = int.Parse(ConfigFetcher.Fetch("NumberOfSpamGunEmailBatch"));

        private const int NumberOfIps = 4;

        private readonly int _limitPerIp = int.Parse(ConfigFetcher.Fetch("NumberOfEmailsPerHour"));

        public SpamGun(IMailComponent mailComponent, IZboxReadServiceWorkerRole zboxReadService, IZboxWorkerRoleService zboxWriteService, ILogger logger, IBlobProvider2<SpamGunContainerName> blobProvider)
        {
            _mailComponent = mailComponent;
            _zboxReadService = zboxReadService;
            _zboxWriteService = zboxWriteService;
            _logger = logger;
            _blobProvider = blobProvider;
        }

        public async Task<bool> ExecuteAsync(int index, Func<int, TimeSpan, Task> progressAsync, CancellationToken token)
        {
            var numberOfUniversities = (await _zboxReadService.SpamGunUniversityNumberAsync(token).ConfigureAwait(false)).ToList();
            var htmlBody = await _blobProvider.DownloadTextAsync("mail_template.html").ConfigureAwait(false);
            var totalCount = 0;
            try
            {
                for (var j = 0; j < NumberOfIps; j++)
                {
                    var counter = 0;
                    foreach (var universityId in numberOfUniversities)
                    {
                        //for (var i = 0; i < numberOfUniversities; i++)
                        //{
                        if (counter >= _limitPerIp)
                        {
                            break;
                        }
                        if (j == 3 && universityId == 12) //umich detect ip number 3
                        {
                            continue;
                        }
                        var emails = await _zboxReadService.GetSpamGunDataAsync(universityId, _numberOfEmailPerSession, token).ConfigureAwait(false);

                        var emailsTask = new List<Task>();
                        var k = 0;
                        foreach (var email in emails) //{
                        {
                            if (counter >= _limitPerIp)
                            {
                                break;
                            }
                            var t1 = _mailComponent.SendSpanGunEmailAsync(email.Email, BuildIpPool(j),
                                   new SpamGunMailParams(email.MailBody,
                                       email.FirstName.UppercaseFirst(), email.MailSubject,
                                       email.MailCategory, htmlBody),
                                   k, token);
                            await _zboxWriteService.UpdateSpamGunSendAsync(email.Id, token).ConfigureAwait(false);
                            counter++;
                            k++;
                            emailsTask.Add(t1);
                        }
                        await Task.WhenAll(emailsTask).ConfigureAwait(false);
                    }
                    totalCount += counter;
                }
            }
            catch (Exception ex)
            {
                _logger.Exception(ex);
            }
            if (totalCount > 0)
            {
                _logger.Info($"spam gun send {totalCount} emails");
            }

            return true;
        }



        private static string BuildIpPool(int i)
        {
            if (i == 0)
            {
                return string.Empty;
            }
            return i.ToString();
        }
    }
}
