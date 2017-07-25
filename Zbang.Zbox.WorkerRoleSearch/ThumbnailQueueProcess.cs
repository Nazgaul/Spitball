﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Azure.Queue;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.WorkerRoleSearch.DomainProcess;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class ThumbnailQueueProcess : IJob
    {
        private readonly IQueueProviderExtract m_QueueProviderExtract;
        private readonly ILifetimeScope m_ComponentContent;
        private readonly ILogger m_Logger;

        public ThumbnailQueueProcess(IQueueProviderExtract queueProviderExtract, ILifetimeScope componentContent, ILogger logger)
        {
            m_QueueProviderExtract = queueProviderExtract;
            m_ComponentContent = componentContent;
            m_Logger = logger;
        }

        public string Name => nameof(ThumbnailQueueProcess);

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var queueName = new ThumbnailQueueName();
                    var result = await m_QueueProviderExtract.RunQueueAsync(queueName, async msg =>
                    {
                        //m_FileProcessorFactory.GetProcessor(msg.AsString)
                        var msgData = msg.FromMessageProto<FileProcess>();
                        if (msgData == null)
                        {
                            m_Logger.Error($"{Name} run - msg cannot transfer to FileProcess");
                            return true;
                        }
                        var process = m_ComponentContent.ResolveOptionalNamed<IFileProcess>(msgData.ProcessResolver);
                        if (process != null) return await process.ExecuteAsync(msgData, cancellationToken).ConfigureAwait(false);

                        m_Logger.Error($"{Name} run - process is null msgData.ProcessResolver: {msgData.ProcessResolver}");
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
                    m_Logger.Exception(ex, new Dictionary<string, string> { { "service", Name } });
                }
            }
        }
    }
}
