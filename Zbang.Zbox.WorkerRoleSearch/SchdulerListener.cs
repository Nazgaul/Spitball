using System;
using System.Collections.Generic;
using System.IO;
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
    public class SchdulerListener : IJob
    {
        private readonly IQueueProviderExtract m_QueueProviderExtract;
        private readonly XmlSerializer m_Dcs = new XmlSerializer(typeof(StorageQueueMessage));

        public SchdulerListener(IQueueProviderExtract queueProviderExtract)
        {
            m_QueueProviderExtract = queueProviderExtract;
        }



        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    TraceLog.WriteInfo("schduler lister run");
                    var queueName = new SchedulerQueueName();
                    await m_QueueProviderExtract.RunQueueAsync(queueName, async msg =>
                    {
                        TraceLog.WriteInfo("schduler lister message" + msg.AsString);
                        StorageQueueMessage message;
                        using (var xmlstream = new MemoryStream(Encoding.Unicode.GetBytes(msg.AsString)))
                        {
                            message = (StorageQueueMessage)m_Dcs.Deserialize(xmlstream);
                        }
                        var messageContent = JObject.Parse(message.Message);
                        var properties = messageContent.Properties();
                        var list = new List<Task>();
                        foreach (var propery in properties)
                        {

                            var t = (int?)propery;
                            //messageContent
                            // var namedjob = sep(job);
                            var process = Infrastructure.Ioc.IocFactory.IocWrapper.TryResolve<IMailProcess>(propery.Name);
                            if (process != null)
                            {
                                list.Add(process.ExcecuteAsync(t ?? 0, async p =>
                               {
                                   propery.Value = p;
                                   message.Message = JsonConvert.SerializeObject(messageContent);
                                   using (var memoryStream = new MemoryStream())
                                   {
                                       m_Dcs.Serialize(memoryStream, message);
                                       msg.SetMessageContent(memoryStream.ToArray());
                                       await m_QueueProviderExtract.UpdateMessageAsync(queueName, msg);
                                   }

                               }, cancellationToken));
                            }

                        }
                        await Task.WhenAll(list);

                        return false;
                    }, TimeSpan.FromMinutes(30), int.MaxValue);
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("on SchdulerListener", ex);
                }
                TraceLog.WriteInfo("schduler lister going to sleep");
                await Task.Delay(TimeSpan.FromHours(1), cancellationToken);

            }

        }

        public void Stop()
        {
        }
    }
}
