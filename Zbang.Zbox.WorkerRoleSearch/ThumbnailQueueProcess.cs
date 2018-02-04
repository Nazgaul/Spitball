using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Azure.Queue;
using Zbang.Zbox.Infrastructure.Storage;
using Cloudents.Core.Interfaces;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.WorkerRoleSearch.DomainProcess;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class ThumbnailQueueProcess : IJob
    {
        private readonly IQueueProviderExtract _queueProviderExtract;
        private readonly ILifetimeScope _componentContent;
        private readonly ILogger _logger;

        public ThumbnailQueueProcess(IQueueProviderExtract queueProviderExtract, ILifetimeScope componentContent, ILogger logger)
        {
            _queueProviderExtract = queueProviderExtract;
            _componentContent = componentContent;
            _logger = logger;
        }

        public string Name => nameof(ThumbnailQueueProcess);

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var queueName = new ThumbnailQueueName();
                    var result = await _queueProviderExtract.RunQueueAsync(queueName, async msg =>
                    {
                        //m_FileProcessorFactory.GetProcessor(msg.AsString)
                        var msgData = msg.FromMessageProto<FileProcess>();
                        if (msgData == null)
                        {
                            _logger.Error($"{Name} run - msg cannot transfer to FileProcess");
                            return true;
                        }
                        var process = _componentContent.ResolveOptionalNamed<IFileProcess>(msgData.ProcessResolver);
                        if (process != null) return await process.ExecuteAsync(msgData, cancellationToken).ConfigureAwait(false);

                        _logger.Error($"{Name} run - process is null msgData.ProcessResolver: {msgData.ProcessResolver}");
                        return true;
                    }, TimeSpan.FromMinutes(1), 5, cancellationToken).ConfigureAwait(false);
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
