using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Azure.Queue;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.WorkerRoleSearch.Mail;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class MailQueueProcess : IJob
    {
        private readonly IQueueProviderExtract m_QueueProviderExtract;
        private readonly ILifetimeScope m_ComponentContent;
        private const string Prefix = "MailProcess";
        public MailQueueProcess(IQueueProviderExtract queueProviderExtract, ILifetimeScope componentContent)
        {
            m_QueueProviderExtract = queueProviderExtract;
            m_ComponentContent = componentContent;
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
                        var msgData = msg.FromMessageProto<Infrastructure.Transport.BaseMailData>();
                        if (msgData == null)
                        {
                            TraceLog.WriteError($" {Prefix} run - msg cannot transfer to DomainProcess");
                            return true;
                        }
                        var process = m_ComponentContent.ResolveOptionalNamed<IMail2>(msgData.MailResover);
                        //var process = Infrastructure.Ioc.IocFactory.IocWrapper.Resolve<IMail2>(msgData.MailResover);
                        if (process == null)
                        {
                            TraceLog.WriteError($"{Prefix} run - process is null msgData.ProcessResolver:" + msgData.MailResover);
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
                    TraceLog.WriteError($" {Prefix}", ex);
                }
            }
        }

        public void Stop()
        {
        }
    }
}
