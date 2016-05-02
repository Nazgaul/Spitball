using System;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Azure.Queue;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.WorkerRoleSearch.DomainProcess;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class MailQueueProcess : IJob
    {
        private readonly IQueueProviderExtract m_QueueProviderExtract;

        public MailQueueProcess(IQueueProviderExtract queueProviderExtract)
        {
            m_QueueProviderExtract = queueProviderExtract;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var queueName = new MailQueueNameNew();
                    var result = await m_QueueProviderExtract.RunQueueAsync(queueName, async msg =>
                    {
                        var msgData = msg.FromMessageJson<Infrastructure.Transport.DomainProcess>();
                        if (msgData == null)
                        {
                            TraceLog.WriteError("UpdateDomainProcess run - msg cannot transfer to DomainProcess");
                            return true;
                        }
                        var process = Infrastructure.Ioc.IocFactory.IocWrapper.Resolve<IDomainProcess>(msgData.ProcessResolver);
                        if (process == null)
                        {
                            TraceLog.WriteError("UpdateDomainProcess run - process is null msgData.ProcessResolver:" + msgData.ProcessResolver);
                            return true;
                        }
                        return await process.ExecuteAsync(msgData, cancellationToken);
                    }, TimeSpan.FromMinutes(1), 5, cancellationToken);
                    if (!result)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                    }
                }
                catch (TaskCanceledException)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("Update UpdateDomainProcess", ex);
                }
            }
        }

        public void Stop()
        {
        }
    }
}
