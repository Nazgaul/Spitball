﻿using System;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Azure.Queue;
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

        public MailProcess2(IQueueProviderExtract queueProvider)
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
                    ExecuteAsync().Wait();
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On Run MailProcess2", ex);
                throw;
            }
        }

        private async Task ExecuteAsync()
        {
            //TraceLog.WriteInfo("Running mail queue");
            await m_QueueProcess.RunQueue(new MailQueueNameNew(), msg =>
             {
                 try
                 {
                     var msgData = msg.FromMessageProto<BaseMailData>();
                     if (msgData == null)
                     {
                         TraceLog.WriteInfo("New MailProcess - message is not in the correct format " + msg.Id);
                         return Task.FromResult(true);
                     }

                     var mail = IocFactory.IocWrapper.Resolve<IMail2>(msgData.MailResover);
                     return mail.ExecuteAsync(msgData);
                 }
                 catch (NullReferenceException ex)
                 {
                     TraceLog.WriteError("New MailProcess run " + msg.Id, ex);
                     return Task.FromResult(true);
                 }
                 catch (ArgumentException ex)
                 {
                     TraceLog.WriteError("New MailProcess run " + msg.Id, ex);
                     return Task.FromResult(true);
                 }
                 catch (Exception ex)
                 {
                     TraceLog.WriteError("New MailProcess run " + msg.Id, ex);
                     return Task.FromResult(false);
                 }


             }, TimeSpan.FromSeconds(5));
        }

        public void Stop()
        {
            m_KeepRunning = false;
        }
    }
}
