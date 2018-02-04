using Autofac;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Azure.Queue;
using Zbang.Zbox.Infrastructure.Storage;
using Cloudents.Core.Interfaces;
using Zbang.Zbox.WorkerRoleSearch.DomainProcess;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class TransactionQueueProcess : IJob
    {
        private readonly IQueueProviderExtract _queueProviderExtract;
        private readonly ILifetimeScope m_LifetimeScope;
        private readonly ILogger _logger;

        public TransactionQueueProcess(IQueueProviderExtract queueProviderExtract, ILifetimeScope lifetimeScope, ILogger logger)
        {
            _queueProviderExtract = queueProviderExtract;
            m_LifetimeScope = lifetimeScope;
            _logger = logger;
        }

        public string Name => nameof(TransactionQueueProcess);

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var queueName = new UpdateDomainQueueName();
                    var result = await _queueProviderExtract.RunQueueAsync(queueName, async msg =>
                    {
                        var msgData = msg.FromMessageProto<Infrastructure.Transport.DomainProcess>();
                        if (msgData == null)
                        {
                            _logger.Error($"{Name} run - msg cannot transfer to DomainProcess");
                            return true;
                        }
                        var process = m_LifetimeScope.ResolveNamed<IDomainProcess>(msgData.ProcessResolver);
                        if (process == null)
                        {
                            _logger.Error($"{Name} run - process is null msgData.ProcessResolver: {msgData.ProcessResolver}");
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
                    _logger.Exception(ex, new Dictionary<string, string> {["service"] = Name });
                }
            }
        }
    }
}
