using System;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.WorkerRole.Mail;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    class MailProcess2 : IJob
    {
        private bool m_KeepRunning;
        //private readonly IQueueProvider m_QueueProvider;
        private readonly QueueProcess m_QueueProcess;

        public MailProcess2(IQueueProvider queueProvider)
        {
            //m_QueueProvider = queueProvider;
            m_QueueProcess = new QueueProcess(queueProvider, TimeSpan.FromSeconds(5));
        }

        public void Run()
        {
            try
            {
                m_KeepRunning = true;

                while (m_KeepRunning)
                {
                    Execute();
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On Run MailProcess2", ex);
                throw;
            }
        }

        private void Execute()
        {
            m_QueueProcess.RunQueue(new MailQueueNameNew(), msg =>
            {
                var msgData = msg.FromMessageProto<BaseMailData>();
                if (msgData == null)
                {
                    TraceLog.WriteInfo("New MailProcess - message is not in the correct format " + msg.Id);
                    return true;
                }
                try
                {
                    var mail = IocFactory.Unity.Resolve<IMail2>(msgData.MailResover);
                    return mail.Execute(msgData);
                }
                catch (NullReferenceException ex)
                {
                    TraceLog.WriteError("New MailProcess run " + msg.Id, ex);
                    return true;
                }
                catch (ArgumentException ex)
                {
                    TraceLog.WriteError("New MailProcess run " + msg.Id, ex);
                    return true;
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("New MailProcess run " + msg.Id, ex);
                    return false;
                }


            }, TimeSpan.FromMinutes(1));
        }

        public void Stop()
        {
            m_KeepRunning = false;
        }
    }
}
