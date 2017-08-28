using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Autofac;
using Microsoft.WindowsAzure.Scheduler.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Zbang.Zbox.Infrastructure.Azure.Queue;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.WorkerRoleSearch.Mail;
using TaskExtensions = Zbang.Zbox.Infrastructure.Extensions.TaskExtensions;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class SchedulerListener : IJob, IDisposable
    {
        private readonly IQueueProviderExtract m_QueueProviderExtract;
        private readonly ILifetimeScope m_LifetimeScope;
        private readonly XmlSerializer m_Dcs = new XmlSerializer(typeof(StorageQueueMessage));
        private readonly SemaphoreSlim m_CriticalCode = new SemaphoreSlim(1);
        private readonly ILogger m_Logger;

        public SchedulerListener(IQueueProviderExtract queueProviderExtract, ILifetimeScope lifetimeScope, ILogger logger)
        {
            m_QueueProviderExtract = queueProviderExtract;
            m_LifetimeScope = lifetimeScope;
            m_Logger = logger;
        }

        public string Name => nameof(SchedulerListener);

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var queueName = new SchedulerQueueName();

                    await m_QueueProviderExtract.RunQueueAsync(queueName, async msg =>
                    {
                        StorageQueueMessage message;
                        using (var xmlStream = new MemoryStream(Encoding.Unicode.GetBytes(msg.AsString)))
                        {
                            message = (StorageQueueMessage)m_Dcs.Deserialize(xmlStream);
                        }
                        m_Logger.Info($"{Name} found message {msg.AsString}");
                        var messageContent = JObject.Parse(message.Message);
                        var properties = messageContent.Properties();
                        var list = new List<Task<bool>>();
                        foreach (var property in properties)
                        {
                            var t = (int?)property;
                            var process = m_LifetimeScope.ResolveOptionalNamed<ISchedulerProcess>(property.Name);
                            if (process != null)
                            {
                                list.Add(process.ExecuteAsync(t ?? 0, async (progress,visible) =>
                                {
                                    property.Value = progress;
                                    message.Message = JsonConvert.SerializeObject(messageContent);
                                    using (var memoryStream = new MemoryStream())
                                    {
                                        await m_CriticalCode.WaitAsync(cancellationToken).ConfigureAwait(false);
                                        try
                                        {
                                            m_Dcs.Serialize(memoryStream, message);
                                            msg.SetMessageContent(memoryStream.ToArray());
                                            await m_QueueProviderExtract.UpdateMessageAsync(queueName, msg, visible, cancellationToken).ConfigureAwait(false);
                                        }
                                        catch (Exception ex)
                                        {
                                            m_Logger.Exception(ex, new Dictionary<string, string> {["service"] = Name });
                                        }
                                        finally
                                        {
                                            m_CriticalCode.Release();
                                        }
                                    }
                                }, cancellationToken));
                            }
                            else
                            {
                                list.Add(TaskExtensions.CompletedTaskFalse);
                                m_Logger.Warning($"cant resolve {property.Name}");
                            }
                        }
                        await Task.WhenAll(list).ConfigureAwait(false);
                        var result = list.All(a => a.Result);
                        return result;
                    }, TimeSpan.FromMinutes(15), 3, cancellationToken).ConfigureAwait(false);
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
                    m_Logger.Exception(ex, new Dictionary<string, string> {["service"] = Name });
                }
                await Task.Delay(TimeSpan.FromMinutes(2), cancellationToken).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            m_CriticalCode?.Dispose();
            m_LifetimeScope?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
