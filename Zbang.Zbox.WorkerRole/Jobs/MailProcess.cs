using System;
using Microsoft.Practices.Unity;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer;
using Zbang.Zbox.WorkerRole.Mail;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class MailProcess : IJob
    {
        private readonly IUnityContainer m_UnityContainer;
        private readonly IQueueProvider m_QueueProvider;
        private bool m_KeepRunning;
        private readonly QueueProcess m_QueueProcess;

        public MailProcess(IQueueProvider queueProvider, IUnityContainer unityContainer)
        {
            m_QueueProvider = queueProvider;
            m_UnityContainer = unityContainer;
            m_QueueProcess = new QueueProcess(queueProvider, TimeSpan.FromSeconds(30));
        }
        public void Run()
        {
            m_KeepRunning = true;

            while (m_KeepRunning)
            {
                Execute();
            }
        }

        private void Execute()
        {
            m_QueueProcess.RunQueue(new MailQueueName(), msg =>
            {
                var msgData = msg.FromMessage<MailQueueData>();
                if (msgData != null)
                {
                    try
                    {
                        var mail = m_UnityContainer.Resolve<IMail>(msgData.ResolverName);
                        return mail.Excecute(msgData);
                    }
                    catch (NullReferenceException ex)
                    {
                        TraceLog.WriteError("MailProcess run " + msg.AsString, ex);
                        return true;
                    }
                    catch (ArgumentException ex)
                    {
                        TraceLog.WriteError("MailProcess run " + msg.AsString, ex);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        TraceLog.WriteError("MailProcess run " + msg.AsString, ex);
                        return false;
                    }
                }
                TraceLog.WriteInfo("MailProcess - message is not in the currect format " + msg.AsString);
                return true;
            }, TimeSpan.FromMinutes(20));
        }




        public void Stop()
        {
            m_KeepRunning = false;
        }
    }


}
