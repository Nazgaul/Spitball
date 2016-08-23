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

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class SpamGun : IJob
    {
        //private readonly IQueueProviderExtract m_QueueProvider;
        private readonly IMailComponent m_MailComponent;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IZboxWorkerRoleService m_ZboxWriteService;


        public const int NumberOfEmailPerSession = 30;
        //public readonly TimeSpan NumberOfTimeToSleep = TimeSpan.FromSeconds(2500);
        public readonly TimeSpan NumberOfTimeToSleep = TimeSpan.FromHours(1);

        public const int SpanGunNumberOfQueues = 10;
        private const int NumberOfIps = 3;
        public const string SpanGunQueuePrefix = "spangun-";
        //private readonly CloudQueue[] m_CloudQueues = new CloudQueue[SpanGunNumberOfQueues];
        private readonly Queue<SpamGunDto>[] m_Queues = new Queue<SpamGunDto>[SpanGunNumberOfQueues];

        //295,413,579,810,1000,1587,2222,3111,4356,6098,8583,11953,16734,23427,32798,45917,64284,89998,125997,176395,246953,345735,484029,677640,948696,1328175,1859444,2603222,3644511,5102316,7143242,10000539,14000754,19601056
        private readonly int m_LimitPerIp = int.Parse(ConfigFetcher.Fetch("NumberOfEmailsPerHour"));
        private const string ServiceName = "SpamGunService";


        public SpamGun(/*IQueueProviderExtract queueProvider,*/ IMailComponent mailComponent, IZboxReadServiceWorkerRole zboxReadService, IZboxWorkerRoleService zboxWriteService)
        {
            //m_QueueProvider = queueProvider;
            m_MailComponent = mailComponent;
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteService = zboxWriteService;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            TraceLog.WriteWarning($"{ServiceName} starting with number of emails {m_LimitPerIp}");
            for (int i = 0; i < SpanGunNumberOfQueues; i++)
            {
                m_Queues[i] = new Queue<SpamGunDto>();
                //var queueName = BuidQueueName(i);
                //var queue = m_QueueProvider.GetQueue(queueName);
                //m_CloudQueues[i] = queue;
                //tasks.Add(queue.CreateIfNotExistsAsync(cancellationToken));
            }
            //await Task.WhenAll(tasks);
            while (!cancellationToken.IsCancellationRequested && string.Equals(ConfigFetcher.Fetch("SpamGunRun"), bool.TrueString, StringComparison.InvariantCultureIgnoreCase))
            {

                var reachHourLimit = false;
                var totalCount = 0;
                try
                {
                    for (var j = 0; j < NumberOfIps; j++)
                    {
                        var counter = 0;
                        for (var i = 0; i < SpanGunNumberOfQueues; i++)
                        {
                            //var queue = m_QueueProvider.GetQueue(BuidQueueName(i));
                            //await queue.FetchAttributesAsync(cancellationToken);
                            //if (queue.ApproximateMessageCount < 50)
                            //{
                            await BuildQueueDataAsync(m_Queues[i], i, cancellationToken);
                            //}

                            var emailsTask = new List<Task>();
                            for (var k = 0; k < NumberOfEmailPerSession; k++)
                            {
                                if (counter >= m_LimitPerIp)
                                {
                                    TraceLog.WriteInfo($"{ServiceName} ip {j} reach hour peak");
                                    reachHourLimit = true;
                                    break;
                                }
                                if (m_Queues[i].Count == 0)
                                {
                                    TraceLog.WriteWarning($"{ServiceName} queue {i} is empty");
                                    break;
                                }
                                var message = m_Queues[i].Dequeue();
                                //var message = await queue.GetMessageAsync(TimeSpan.FromMinutes(30), new QueueRequestOptions(), new Microsoft.WindowsAzure.Storage.OperationContext(), cancellationToken);
                                if (message == null)
                                {
                                    TraceLog.WriteWarning($"{ServiceName} message is null {i}");
                                    break;
                                }
                                //var emailMessage = message.FromMessageProto<SpamGunData>();

                                var t1 = m_MailComponent.SendSpanGunEmailAsync(message.Email, BuildIpPool(j),
                                    message.MailBody, message.MailSubject, message.FirstName, message.MailCategory);
                                await m_ZboxWriteService.UpdateSpamGunSendAsync(message.Id, cancellationToken);
                                //var t2 = queue.DeleteMessageAsync(message, cancellationToken);
                                counter++;
                                emailsTask.Add(t1);
                            }
                            //counter += emailsTask.Count;
                            //TraceLog.WriteInfo($"{ServiceName} send email to {emailsTask.Count}");
                            await Task.WhenAll(emailsTask);
                        }
                        totalCount += counter;
                        TraceLog.WriteInfo($"{ServiceName} send via ip {counter}");
                    }
                }
                catch (Exception ex)
                {
                    await m_MailComponent.GenerateSystemEmailAsync("spam gun error", $"error {ex}");
                }
                await m_MailComponent.GenerateSystemEmailAsync("spam gun", $"send {totalCount} emails");
                if (reachHourLimit)
                {
                    TraceLog.WriteInfo($"{ServiceName} going to sleep for an hour");
                    await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
                }
                else
                {
                    TraceLog.WriteInfo($"{ServiceName} going to sleep for 2500 seconds");
                    await Task.Delay(NumberOfTimeToSleep, cancellationToken);
                }
            }
            TraceLog.WriteInfo($"{ServiceName} going not running.");
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
            return $"ip{i + 1}";
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
