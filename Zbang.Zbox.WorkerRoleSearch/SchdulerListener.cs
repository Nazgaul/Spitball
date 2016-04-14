using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.WindowsAzure.Scheduler.Models;
using Zbang.Zbox.Infrastructure.Azure.Queue;
using Zbang.Zbox.Infrastructure.Storage;

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

                await m_QueueProviderExtract.RunQueueAsync(new SchedulerQueueName(), msg =>
                 {

                     using (MemoryStream xmlstream = new MemoryStream(Encoding.Unicode.GetBytes(msg.AsString)))

                     {
                         StorageQueueMessage message = (StorageQueueMessage)m_Dcs.Deserialize(xmlstream);

                         var jobs = message.Message.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                         foreach (var job in jobs)
                         {
                             
                         }




                         //var process = IocFactory.IocWrapper.Resolve<IDomainProcess>(msgData.ProcessResolver);
                         //if (process == null)
                         //{
                         //    TraceLog.WriteError("UpdateDomainProcess run - process is null msgData.ProcessResolver:" + msgData.ProcessResolver);
                         //    return true;
                         //}
                         //return await process.ExecuteAsync(msgData);
                         //"university-not-selected"




                     }
                     return Task.FromResult(false);
                 }, TimeSpan.FromMinutes(1), int.MaxValue);
                await Task.Delay(TimeSpan.FromHours(1), cancellationToken);

            }

        }
    }
}
