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
    public class ThumbnailQueueProcess : IJob
    {
        private readonly IQueueProviderExtract m_QueueProviderExtract;
        private const string Prefix = "ThumbnailProcess";
        public ThumbnailQueueProcess(IQueueProviderExtract queueProviderExtract)
        {
            m_QueueProviderExtract = queueProviderExtract;
        }

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
                        var msgData = msg.FromMessageProto<Infrastructure.Transport.FileProcess>();
                         if (msgData == null)
                         {
                             TraceLog.WriteError($"{Prefix} run - msg cannot transfer to FileProcess");
                             return true;
                         }
                         var process = Infrastructure.Ioc.IocFactory.IocWrapper.Resolve<IFileProcess>(msgData.ProcessResolver);
                         if (process != null) return await process.ExecuteAsync(msgData, cancellationToken);
                         TraceLog.WriteError($"{Prefix} run - process is null msgData.ProcessResolver:" + msgData.ProcessResolver);
                         return true;
                     }, TimeSpan.FromMinutes(1), 5, cancellationToken);
                    if (!result)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
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
