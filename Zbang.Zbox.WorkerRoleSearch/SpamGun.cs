using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Azure.Queue;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class SpamGun : IJob
    {
        private readonly IQueueProviderExtract m_QueueProvider;
        private readonly IMailComponent m_MailComponent;
        public const int SpanGunNumberOfQueues = 10;
        private const int NumberOfIps = 3;
        public const string SpanGunQueuePrefix = "spangun-";
        private readonly CloudQueue[] m_CloudQueues = new CloudQueue[SpanGunNumberOfQueues];
        //211,295,413,579,810,1000,1587,2222,3111,4356,6098,8583,11953,16734,23427,32798,45917,64284,89998,125997,176395,246953,345735,484029,677640,948696,1328175,1859444,2603222,3644511,5102316,7143242,10000539,14000754,19601056
        private readonly int m_LimitPerIp = int.Parse(ConfigFetcher.Fetch("NumberOfEmailsPerHour"));
        private const string ServiceName = "SpamGunService";


        public SpamGun(IQueueProviderExtract queueProvider, IMailComponent mailComponent)
        {
            m_QueueProvider = queueProvider;
            m_MailComponent = mailComponent;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            TraceLog.WriteWarning($"{ServiceName} starting with number of emails {m_LimitPerIp}");
            var tasks = new List<Task>();
            for (int i = 0; i < SpanGunNumberOfQueues; i++)
            {
                var queueName = BuidQueueName(i);
                var queue = m_QueueProvider.GetQueue(queueName);
                m_CloudQueues[i] = queue;
                tasks.Add(queue.CreateIfNotExistsAsync(cancellationToken));
            }
            await Task.WhenAll(tasks);
            while (!cancellationToken.IsCancellationRequested)
            {

                var reachHourLimit = false;
                var totalCount = 0;
                for (int j = 0; j < NumberOfIps; j++)
                {
                    var counter = 0;
                    for (int i = 0; i < SpanGunNumberOfQueues; i++)
                    {
                        var queue = m_QueueProvider.GetQueue(BuidQueueName(i));
                        var emailsTask = new List<Task>();
                        for (int k = 0; k < 50; k++)
                        {
                            if (counter >= m_LimitPerIp)
                            {
                                TraceLog.WriteInfo($"{ServiceName} ip {j} reach hour peak");
                                reachHourLimit = true;
                                break;
                            }
                            var message = await queue.GetMessageAsync(TimeSpan.FromMinutes(30), new QueueRequestOptions(), new Microsoft.WindowsAzure.Storage.OperationContext(), cancellationToken);
                            if (message == null)
                            {
                                TraceLog.WriteWarning($"{ServiceName} message is null {i}");
                                break;
                            }
                            var emailMessage = message.FromMessageProto<SpamGunData>();
                            counter++;
                            var t1 = m_MailComponent.SendSpanGunEmailAsync(emailMessage.Email, BuildIpPool(j));
                            var t2 = queue.DeleteMessageAsync(message, cancellationToken);
                            emailsTask.Add(Task.WhenAll(t1, t2));
                        }
                        //counter += emailsTask.Count;
                        //TraceLog.WriteInfo($"{ServiceName} send email to {emailsTask.Count}");
                        await Task.WhenAll(emailsTask);
                    }
                    totalCount += counter;
                    TraceLog.WriteInfo($"{ServiceName} send via ip {counter}");
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
                    await Task.Delay(TimeSpan.FromSeconds(2500), cancellationToken);
                }
            }
        }

        private static string BuildIpPool(int i)
        {
            return $"ip{i + 1}";
        }

        public static string BuidQueueName(int i)
        {
            return $"{SpanGunQueuePrefix}{i}";
        }

        public void Stop()
        {

        }
    }
}
