using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Azure.Queue;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.WorkerRoleSearch.Mail;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class MailQueueProcess : IJob
    {
        private readonly IQueueProviderExtract m_QueueProviderExtract;
        private readonly ILifetimeScope m_ComponentContent;
        private readonly ILogger _logger;
        public MailQueueProcess(IQueueProviderExtract queueProviderExtract, ILifetimeScope componentContent, ILogger logger)
        {
            m_QueueProviderExtract = queueProviderExtract;
            m_ComponentContent = componentContent;
            _logger = logger;
        }

        public string Name => nameof(MailQueueProcess);
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var queueName = new MailQueueName();
                    var result = await m_QueueProviderExtract.RunQueueAsync(queueName, async msg =>
                    {
                        _logger.Info($"{Name} is doing process");
                        var msgData = msg.FromMessageProto<BaseMailData>();
                        if (msgData == null)
                        {
                            _logger.Error($"{Name} run - msg cannot transfer to DomainProcess");
                            return true;
                        }
                        var process = m_ComponentContent.ResolveOptionalNamed<IMail2>(msgData.MailResolver);
                        if (process == null)
                        {
                            _logger.Error($"{Name} run - process is null msgData.ProcessResolver:" + msgData.MailResolver);
                            return true;
                        }
                        return await process.ExecuteAsync(msgData, cancellationToken).ConfigureAwait(false);
                    }, TimeSpan.FromMinutes(1), 5, cancellationToken).ConfigureAwait(false);
                    if (!result)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);
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
                    _logger.Exception(ex, new Dictionary<string, string>
                    {
                        ["service"] = Name
                    });
                }
            }
        }
    }
}
