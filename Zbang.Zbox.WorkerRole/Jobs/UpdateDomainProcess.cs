using System;
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


        public UpdateDomainProcess(IQueueProvider queueProvider)
        {
            m_QueueProcess = new QueueProcess(queueProvider, TimeSpan.FromSeconds(15));
        }


        public void Run()
        {

            m_KeepRunning = true;
            while (m_KeepRunning)
            {
                try
                {
                    Excecute();
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("Update UpdateDomainProcess", ex);
                }
            }


        }

        private void Excecute()
        {

            m_QueueProcess.RunQueue(new UpdateDomainQueueName(), msg =>
            {
                try
                {
                    var msgData = msg.FromMessageProto<Infrastructure.Transport.DomainProcess>();
                    if (msgData == null)
                    {
                        TraceLog.WriteError("UpdateDomainProcess run - msg cannot transfer to DomainProcess msgid:" + msg.Id);
                        return true;
                    }
                    var process = IocFactory.Unity.Resolve<IDomainProcess>(msgData.ProcessResolver);
                    if (process == null)
                    {
                        TraceLog.WriteError("UpdateDomainProcess run - process is null msgid:" + msg.Id + " msgData.ProcessResolver:" + msgData.ProcessResolver);
                        return true;
                    }
                    return process.Excecute(msgData);
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("UpdateDomainProcess run " + msg.Id, ex);
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
