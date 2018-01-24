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
using Zbang.Zbox.WorkerRoleSearch.Mail;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class SpamGun : ISchedulerProcess
    {
        private readonly IMailComponent _mailComponent;
        private readonly IZboxReadServiceWorkerRole _zboxReadService;
        private readonly IZboxWorkerRoleService _zboxWriteService;
        private readonly ILogger _logger;

        private readonly IBlobProvider2<SpamGunContainerName> _blobProvider;

        public readonly int NumberOfEmailPerSession = int.Parse(ConfigFetcher.Fetch("NumberOfSpamGunEmailBatch"));

        //public const int SpanGunNumberOfQueues = 13;
        private const int NumberOfIps = 4;
        //private readonly Queue<SpamGunDto>[] _queues = new Queue<SpamGunDto>[SpanGunNumberOfQueues];

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
            //if (!NeedToProcess())
            //{
            //    return true;
            //}
            var numberOfUniversities = (await _zboxReadService.SpamGunUniversityNumberAsync(token).ConfigureAwait(false)).ToList();
            var htmlBody = await _blobProvider.DownloadTextAsync("mail.html").ConfigureAwait(false);
            //for (var i = 0; i < SpanGunNumberOfQueues; i++)
            //{
            //    _queues[i] = new Queue<SpamGunDto>();
            //}
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
                        var emails = await _zboxReadService.GetSpamGunDataAsync(universityId, NumberOfEmailPerSession, token).ConfigureAwait(false);

                        // await BuildQueueDataAsync(_queues[i], i, token).ConfigureAwait(false);
                        var emailsTask = new List<Task>();
                        var k = 0;
                        foreach (var email in emails) //{ 
                        //for (var k = 0; k < NumberOfEmailPerSession; k++)
                        {
                            if (counter >= _limitPerIp)
                            {
                                break;
                            }
                            //if (_queues[i].Count == 0)
                            //{
                            //    break;
                            //}
                            //var message = _queues[i].Dequeue();
                            //if (message == null)
                            //{
                            //    break;
                            //}
                            //var greekMessage = email as GreekPartnerDto;
                            //Task t1;
                            //if (greekMessage == null)
                            //{
                            var t1 = _mailComponent.SendSpanGunEmailAsync(email.Email, BuildIpPool(j),
                                   new SpamGunMailParams(email.MailBody,
                                       email.FirstName.UppercaseFirst(), email.MailSubject,
                                       email.MailCategory, htmlBody),
                                   k, token);
                            //}
                            //else
                            //{
                            //    t1 = _mailComponent.SendSpanGunEmailAsync(greekMessage.Email, BuildIpPool(j),
                            //       new GreekPartnerMailParams(greekMessage.MailBody,
                            //           greekMessage.UniversityUrl, greekMessage.FirstName.UppercaseFirst(), greekMessage.MailSubject,
                            //           greekMessage.MailCategory, greekMessage.School, greekMessage.Chapter),
                            //           k, token);
                            //}
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
                await _mailComponent.GenerateSystemEmailAsync("spam gun error", $"error {ex}").ConfigureAwait(false);
            }
            if (totalCount > 0)
            {
                _logger.Info($"spam gun send {totalCount} emails");
            }

            return true;
        }

        //private async Task BuildQueueDataAsync(Queue<SpamGunDto> queue, int queueUniversityId, CancellationToken token)
        //{
        //    queue.Clear();
        //    foreach (var val in await _zboxReadService.GetSpamGunDataAsync(++queueUniversityId, token).ConfigureAwait(false))
        //    {
        //        queue.Enqueue(val);
        //    }
        //}

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
