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
        //private readonly IQueueProviderExtract m_QueueProvider;
        private readonly IMailComponent m_MailComponent;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IZboxWorkerRoleService m_ZboxWriteService;


        public readonly int NumberOfEmailPerSession = int.Parse(ConfigFetcher.Fetch("NumberOfSpamGunEmailBatch"));
        //public readonly TimeSpan NumberOfTimeToSleep = TimeSpan.FromSeconds(2500);
        //public readonly TimeSpan NumberOfTimeToSleep = TimeSpan.FromHours(1);

        public const int SpanGunNumberOfQueues = 13;
        private const int NumberOfIps = 4;
        private readonly Queue<SpamGunDto>[] m_Queues = new Queue<SpamGunDto>[SpanGunNumberOfQueues];

        //20,28,39,55,77,108,151,211,295,413,579,810,1000,1587,2222,3111,4356,6098,8583,11953,16734,23427,32798,45917,64284,89998,125997,176395,246953,345735,484029,677640,948696,1328175,1859444,2603222,3644511,5102316,7143242,10000539,14000754,19601056
        private readonly int m_LimitPerIp = int.Parse(ConfigFetcher.Fetch("NumberOfEmailsPerHour"));
        private const string ServiceName = "SpamGunService";


        public SpamGun(IMailComponent mailComponent, IZboxReadServiceWorkerRole zboxReadService, IZboxWorkerRoleService zboxWriteService)
        {
            m_MailComponent = mailComponent;
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteService = zboxWriteService;
        }


        public async Task<bool> ExecuteAsync(int index, Func<int, Task> progressAsync, CancellationToken token)
        {
            for (var i = 0; i < SpanGunNumberOfQueues; i++)
            {
                m_Queues[i] = new Queue<SpamGunDto>();
            }
            //var reachHourLimit = false;
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
                            TraceLog.WriteInfo($"{ServiceName} ip {j} reach hour peak");
                            // reachHourLimit = true;
                            break;
                        }
                        
                        await BuildQueueDataAsync(m_Queues[i], i, token);
                        var emailsTask = new List<Task>();
                        for (var k = 0; k < NumberOfEmailPerSession; k++)
                        {
                            if (counter >= m_LimitPerIp)
                            {
                                TraceLog.WriteInfo($"{ServiceName} ip {j} reach hour peak");
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
                                        message.MailCategory), token);
                            }
                            else
                            {
                                t1 = m_MailComponent.SendSpanGunEmailAsync(message.Email, BuildIpPool(j),
                                   new GreekPartnerMailParams(message.MailBody,
                                       message.UniversityUrl, message.FirstName.UppercaseFirst(), message.MailSubject,
                                       message.MailCategory, greekMessage.School, greekMessage.Chapter), token);
                            }
                            await m_ZboxWriteService.UpdateSpamGunSendAsync(message.Id, token);
                            counter++;
                            emailsTask.Add(t1);
                        }
                        await Task.WhenAll(emailsTask);
                    }
                    totalCount += counter;
                    TraceLog.WriteInfo($"{ServiceName} send via ip {counter}");
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("spam gun error", ex);
                await m_MailComponent.GenerateSystemEmailAsync("spam gun error", $"error {ex}");
            }
            if (totalCount > 0)
            {
                await m_MailComponent.GenerateSystemEmailAsync("spam gun", $"send {totalCount} emails");
            }
           
            TraceLog.WriteInfo($"{ServiceName} going not running.");
            return true;
        }

        private async Task BuildQueueDataAsync(Queue<SpamGunDto> queue, int queueUniversityId, CancellationToken token)
        {
            queue.Clear();
            var data = await m_ZboxReadService.GetSpamGunDataAsync(++queueUniversityId, token);
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

        //public static string BuidQueueName(int i)
        //{
        //    return $"{SpanGunQueuePrefix}{i}";
        //}

        //public void Stop()
        //{

        //}

    }
}
