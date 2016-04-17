using System;
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
                var queueName = new SchedulerQueueName();
                await m_QueueProviderExtract.RunQueueAsync(queueName, async msg =>
                {
                    StorageQueueMessage message;
                    using (var xmlstream = new MemoryStream(Encoding.Unicode.GetBytes(msg.AsString)))
                    {
                        message = (StorageQueueMessage)m_Dcs.Deserialize(xmlstream);
                    }
                    var messageContent = JObject.Parse(message.Message);
                    var properties = messageContent.Properties();
                    foreach (var propery in properties)
                    {
                        var t = (int?) propery;
                        //messageContent
                        // var namedjob = sep(job);
                        var process = Infrastructure.Ioc.IocFactory.IocWrapper.Resolve<IMailProcess>(propery.Name);
                       await process.ExcecuteAsync(t ?? 0, async p =>
                         {
                             propery.Value = p;
                             message.Message = JsonConvert.SerializeObject(messageContent);
                             using (var memoryStream = new MemoryStream())
                             {
                                 m_Dcs.Serialize(memoryStream, message);
                                 msg.SetMessageContent(memoryStream.ToArray());
                                 await m_QueueProviderExtract.UpdateMessageAsync(queueName, msg);
                             }

                         }, cancellationToken);
                    }

                    return false;
                }, TimeSpan.FromMinutes(1), int.MaxValue);
                await Task.Delay(TimeSpan.FromHours(1), cancellationToken);

            }

        }
    }
}
