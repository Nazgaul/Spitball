using System;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Azure.Queue;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.WorkerRole.DomainProcess;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class UpdateDomainProcess : IJob
    {

        readonly private QueueProcess m_QueueProcess;
        private bool m_KeepRunning;


        public UpdateDomainProcess(IQueueProviderExtract queueProvider)
        {
            m_QueueProcess = new QueueProcess(queueProvider, TimeSpan.FromSeconds(15));
        }


        public void Run()
        {

            m_KeepRunning = true;
            while (m_KeepRunning)
            {
                TraceLog.WriteInfo("Running update domain process");
                try
                {
                    ExecuteAsync().Wait();
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("Update UpdateDomainProcess", ex);
                }
            }


        }

        private async Task ExecuteAsync()
        {

            await m_QueueProcess.RunQueue(new UpdateDomainQueueName(), async msg =>
              {
                 
                  try
                  {
                      var msgData = msg.FromMessageProto<Infrastructure.Transport.DomainProcess>();
                      if (msgData == null)
                      {
                          TraceLog.WriteError("UpdateDomainProcess run - msg cannot transfer to DomainProcess");
                          return true;
                      }
                      TraceLog.WriteInfo("Running update domain process - processing -" + msgData.ProcessResolver);
                      var process = IocFactory.IocWrapper.Resolve<IDomainProcess>(msgData.ProcessResolver);
                      if (process == null)
                      {
                          TraceLog.WriteError("UpdateDomainProcess run - process is null msgData.ProcessResolver:" + msgData.ProcessResolver);
                          return true;
                      }
                      return await process.ExecuteAsync(msgData);
                  }
                  catch (Exception ex)
                  {
                      TraceLog.WriteError("UpdateDomainProcess run", ex);
                  }
                  return false;
              }, TimeSpan.FromMinutes(1), 5);
        }

        public void Stop()
        {
            m_KeepRunning = false;
        }
    }
}
