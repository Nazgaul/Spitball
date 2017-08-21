using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.Emails;
using Zbang.Zbox.WorkerRoleSearch.Mail;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class SpamGun : ISchedulerProcess
    {
        private readonly IMailComponent m_MailComponent;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IZboxWorkerRoleService m_ZboxWriteService;
        private readonly ILogger m_Logger;

        public readonly int NumberOfEmailPerSession = int.Parse(ConfigFetcher.Fetch("NumberOfSpamGunEmailBatch"));

        public const int SpanGunNumberOfQueues = 13;
        private const int NumberOfIps = 4;
        private readonly Queue<SpamGunDto>[] m_Queues = new Queue<SpamGunDto>[SpanGunNumberOfQueues];

        private readonly int m_LimitPerIp = int.Parse(ConfigFetcher.Fetch("NumberOfEmailsPerHour"));
        //private const string ServiceName = "SpamGunService";

        public SpamGun(IMailComponent mailComponent, IZboxReadServiceWorkerRole zboxReadService, IZboxWorkerRoleService zboxWriteService, ILogger logger)
        {
            m_MailComponent = mailComponent;
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteService = zboxWriteService;
            m_Logger = logger;
        }

        //private static bool NeedToProcess()
        //{
        //    if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Saturday)
        //    {
        //        return false;
        //    }
        //    if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Friday)
        //    {
        //        if (DateTime.UtcNow.Hour > 22)
        //        {
        //            return false;
        //        }
        //    }
        //    if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Sunday)
        //    {
        //        if (DateTime.UtcNow.Hour < 13)
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        public async Task<bool> ExecuteAsync(int index, Func<int, TimeSpan, Task> progressAsync, CancellationToken token)
        {
            //if (!NeedToProcess())
            //{
            //    return true;
            //}
            for (var i = 0; i < SpanGunNumberOfQueues; i++)
            {
                m_Queues[i] = new Queue<SpamGunDto>();
            }
            var totalCount = 0;
            try
            {
                for (var j = 0; j < NumberOfIps; j++)
                {
                    var counter = 0;
                    for (var i = 0; i < SpanGunNumberOfQueues; i++)
                    {
                        if (counter >= m_LimitPerIp)
                        {
                            break;
                        }
                        if (j == 3 && i == 12) //umich detect ip number 3
                        {
                            continue;
                        }
                        await BuildQueueDataAsync(m_Queues[i], i, token).ConfigureAwait(false);
                        var emailsTask = new List<Task>();
                        for (var k = 0; k < NumberOfEmailPerSession; k++)
                        {
                            if (counter >= m_LimitPerIp)
                            {
                                break;
                            }
                            if (m_Queues[i].Count == 0)
                            {
                                break;
                            }
                            var message = m_Queues[i].Dequeue();
                            if (message == null)
                            {
                                break;
                            }
                            var greekMessage = message as GreekPartnerDto;
                            Task t1;
                            if (greekMessage == null)
                            {
                                t1 = m_MailComponent.SendSpanGunEmailAsync(message.Email, BuildIpPool(j),
                                    new SpamGunMailParams(message.MailBody,
                                        message.UniversityUrl, message.FirstName.UppercaseFirst(), message.MailSubject,
                                        message.MailCategory),
                                    k, token);
                            }
                            else
                            {
                                t1 = m_MailComponent.SendSpanGunEmailAsync(message.Email, BuildIpPool(j),
                                   new GreekPartnerMailParams(message.MailBody,
                                       message.UniversityUrl, message.FirstName.UppercaseFirst(), message.MailSubject,
                                       message.MailCategory, greekMessage.School, greekMessage.Chapter),
                                       k, token);
                            }
                            await m_ZboxWriteService.UpdateSpamGunSendAsync(message.Id, token).ConfigureAwait(false);
                            counter++;
                            emailsTask.Add(t1);
                        }
                        await Task.WhenAll(emailsTask).ConfigureAwait(false);
                    }
                    totalCount += counter;
                }
            }
            catch (Exception ex)
            {
                m_Logger.Exception(ex);
                await m_MailComponent.GenerateSystemEmailAsync("spam gun error", $"error {ex}").ConfigureAwait(false);
            }
            if (totalCount > 0)
            {
                m_Logger.Info($"spam gun send {totalCount} emails");
            }

            return true;
        }

        private async Task BuildQueueDataAsync(Queue<SpamGunDto> queue, int queueUniversityId, CancellationToken token)
        {
            queue.Clear();
            var data = await m_ZboxReadService.GetSpamGunDataAsync(++queueUniversityId, token).ConfigureAwait(false);
            foreach (var val in data)
            {
                queue.Enqueue(val);
            }
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
