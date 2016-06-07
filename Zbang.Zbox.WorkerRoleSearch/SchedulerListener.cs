using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.WindowsAzure.Scheduler.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Zbang.Zbox.Infrastructure.Azure.Queue;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.WorkerRoleSearch.Mail;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class SchedulerListener : IJob
    {
        private readonly IQueueProviderExtract m_QueueProviderExtract;
        private readonly XmlSerializer m_Dcs = new XmlSerializer(typeof(StorageQueueMessage));
        private readonly SemaphoreSlim m_CriticalCode = new SemaphoreSlim(1);

        public SchedulerListener(IQueueProviderExtract queueProviderExtract)
        {
            m_QueueProviderExtract = queueProviderExtract;
        }



        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    TraceLog.WriteInfo("Scheduler Listener run");
                    var queueName = new SchedulerQueueName();
                    await m_QueueProviderExtract.RunQueueAsync(queueName, async msg =>
                    {
                        TraceLog.WriteInfo($"schduler lister message  {msg.AsString}");
                        StorageQueueMessage message;
                        using (var xmlstream = new MemoryStream(Encoding.Unicode.GetBytes(msg.AsString)))
                        {
                            message = (StorageQueueMessage)m_Dcs.Deserialize(xmlstream);
                        }
                        var messageContent = JObject.Parse(message.Message);
                        var properties = messageContent.Properties();
                        var list = new List<Task<bool>>();
                        foreach (var property in properties)
                        {

                            var t = (int?)property;
                            var process = Infrastructure.Ioc.IocFactory.IocWrapper.TryResolve<IMailProcess>(property.Name);
                            if (process != null)
                            {
                                list.Add(process.ExecuteAsync(t ?? 0, async p =>
                                {
                                    property.Value = p;
                                    message.Message = JsonConvert.SerializeObject(messageContent);
                                    using (var memoryStream = new MemoryStream())
                                    {
                                       
                                        await m_CriticalCode.WaitAsync(cancellationToken);
                                        try
                                        {
                                            m_Dcs.Serialize(memoryStream, message);
                                            msg.SetMessageContent(memoryStream.ToArray());
                                            await m_QueueProviderExtract.UpdateMessageAsync(queueName, msg, cancellationToken);
                                            //TraceLog.WriteInfo($"SchedulerListener - updating queue {message}");
                                        }
                                        catch (Exception ex)
                                        {
                                            TraceLog.WriteError("SchedulerListener - trying to update queue", ex);
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
                                list.Add(Task.FromResult(false));
                                TraceLog.WriteWarning($"cant resolve {property.Name}");
                            }

                        }
                        await Task.WhenAll(list);
                        var result = list.All(a => a.Result);
                        TraceLog.WriteInfo($"schduler lister delete message: {result}");
                        return result;

                    }, TimeSpan.FromMinutes(15), int.MaxValue, cancellationToken);
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
                    TraceLog.WriteError("on SchedulerListener", ex);
                }
                TraceLog.WriteInfo("Scheduler Listener going to sleep");
                await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);

            }

        }

        public void Stop()
        {
        }
    }
}
