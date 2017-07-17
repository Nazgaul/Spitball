using Autofac;
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
    public class TransactionQueueProcess : IJob
    {
        private readonly IQueueProviderExtract m_QueueProviderExtract;
        private readonly ILifetimeScope m_LifetimeScope;

        public TransactionQueueProcess(IQueueProviderExtract queueProviderExtract, ILifetimeScope lifetimeScope)
        {
            m_QueueProviderExtract = queueProviderExtract;
            m_LifetimeScope = lifetimeScope;
        }

        public string Name => nameof(TransactionQueueProcess);

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var queueName = new UpdateDomainQueueName();
                    var result = await m_QueueProviderExtract.RunQueueAsync(queueName, async msg =>
                    {
                        var msgData = msg.FromMessageProto<Infrastructure.Transport.DomainProcess>();
                        if (msgData == null)
                        {
                            TraceLog.WriteError($"{Name} run - msg cannot transfer to DomainProcess");
                            return true;
                        }
                        var process = m_LifetimeScope.ResolveNamed<IDomainProcess>(msgData.ProcessResolver);
                        if (process == null)
                        {
                            TraceLog.WriteError($"{Name} run - process is null msgData.ProcessResolver:" + msgData.ProcessResolver);
                            return true;
                        }
                        return await process.ExecuteAsync(msgData, cancellationToken).ConfigureAwait(false);
                    }, TimeSpan.FromMinutes(45), 3, cancellationToken).ConfigureAwait(false);
                    if (!result)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken).ConfigureAwait(false);
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

        
    }
}
