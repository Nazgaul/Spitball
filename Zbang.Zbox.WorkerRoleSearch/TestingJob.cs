using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Azure.Queue;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.WorkerRoleSearch.DomainProcess;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class TestingJob : IJob
    {
        private readonly IZboxWorkerRoleService m_ZboxWorkerRoleService;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IMailComponent m_MailComponent;
        private readonly IQueueProviderExtract m_QueueProvider;


        public TestingJob(IZboxWorkerRoleService zboxWorkerRoleService, IMailComponent mailComponent, IQueueProviderExtract queueProvider, IZboxReadServiceWorkerRole zboxReadService)
        {
            m_ZboxWorkerRoleService = zboxWorkerRoleService;
            m_MailComponent = mailComponent;
            m_QueueProvider = queueProvider;
            m_ZboxReadService = zboxReadService;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            // await m_MailComponent.SendSpanGunEmailAsync("ram@cloudents.com", "ip1");
            //var tasks = new List<Task>();
            //for (int i = 0; i < SpamGun.SpanGunNumberOfQueues; i++)
            //{
            //    var queueName = SpamGun.BuidQueueName(i);
            //    var queue = m_QueueProvider.GetQueue(queueName);
            //    tasks.Add(queue.CreateIfNotExistsAsync(cancellationToken));
            //}
            //await Task.WhenAll(tasks);
            //tasks.Clear();
            //int page = 0;
            //var mails = (await m_ZboxReadService.GetEmailsAsync(page)).ToList();
            //do
            //{
            //    TraceLog.WriteInfo($"page: {page}");
            //    var queue = m_QueueProvider.GetQueue(SpamGun.BuidQueueName(page % SpamGun.SpanGunNumberOfQueues));
            //    tasks.AddRange(mails.Select(mail => queue.InsertToQueueProtoAsync(new SpamGunData { Email = mail })));
            //    page++;

            //    if (page % 20 == 0)
            //    {
            //        TraceLog.WriteInfo("waiting task to complete");
            //        await Task.WhenAll(tasks);
            //        tasks.Clear();
            //    }
            //    mails = (await m_ZboxReadService.GetEmailsAsync(page)).ToList();
            //} while (mails.Any() && page < 3000);
            //await m_MailComponent.GenerateSystemEmailAsync("stop populating", "stop " + page);
            //await Task.Delay(TimeSpan.FromDays(1), cancellationToken);
            //while (!cancellationToken.IsCancellationRequested)
            //{
            //    var proxy = await SignalrClient.GetProxyAsync();
            //    await proxy.Invoke("UpdateThumbnail", 1, 111239);
            //    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            //}
            //var amount = await m_ZboxWorkerRoleService.UpdateFileSizesAsync(() =>
            //{

            //});
            var updateDate = new UpdateData(1028091, 8417, itemId: 606505);
            var y = Infrastructure.Ioc.IocFactory.IocWrapper.Resolve<IDomainProcess>(updateDate.ProcessResolver);
            await y.ExecuteAsync(updateDate, cancellationToken);

            //await y.GetUnsubscribersAsync(1, cancellationToken);

            //var x = new List<Task<bool>>();

            //var process = Infrastructure.Ioc.IocFactory.IocWrapper.TryResolve<ISchedulerProcess>("digestOnceADay_3");
            //if (process != null)
            //{
            //    await process.ExecuteAsync(0, p => Task.FromResult(true), cancellationToken);
            //}

            //process = Infrastructure.Ioc.IocFactory.IocWrapper.TryResolve<IMailProcess>("likesReport");
            //if (process != null)
            //{
            //    x.Add( process.ExecuteAsync(0, p =>
            //    {
            //    }, cancellationToken));
            //}
            //process = Infrastructure.Ioc.IocFactory.IocWrapper.TryResolve<IMailProcess>("universityLowActivity");
            //if (process != null)
            //{
            //    x.Add( process.ExecuteAsync(0, p =>
            //    {
            //    }, cancellationToken));
            //}

            //process = Infrastructure.Ioc.IocFactory.IocWrapper.TryResolve<IMailProcess>("followLowActivity");
            //if (process != null)
            //{
            //    x.Add( process.ExecuteAsync(0, p =>
            //    {
            //    }, cancellationToken));
            //}

            //await Task.WhenAll(x);
            //var result = x.All(a => a.Result);

        }

        public void Stop()
        {
            // throw new NotImplementedException();
        }
    }
}
