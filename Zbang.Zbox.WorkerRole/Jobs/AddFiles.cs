using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class AddFiles : IJob
    {
        private bool m_KeepRunning;
        private readonly IQueueProvider m_QueueProvider;
        private readonly IBlobProvider m_BlobProvider; // to trigger ctor
        private readonly QueueProcess m_QueueProcess;
        private readonly IZboxWriteService m_ZboxWriteService;


        public AddFiles(IQueueProvider queueProvider, IZboxWriteService zboxWriteService,IBlobProvider blobProvider)
        {
            m_QueueProvider = queueProvider;
            m_ZboxWriteService = zboxWriteService;
            m_BlobProvider = blobProvider;
            m_QueueProcess = new QueueProcess(queueProvider, TimeSpan.FromSeconds(120));
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
                Zbang.Zbox.Infrastructure.Trace.TraceLog.WriteError("On Run AddFiles", ex);
                throw;
            }
        }

        private void Execute()
        {
            m_QueueProcess.RunQueue(new DownloadQueueName(), msg =>
            {
                var msgData = msg.FromMessageProto<Zbang.Zbox.Infrastructure.Transport.UrlToDownloadData>();
                if (msgData == null)
                {
                    TraceLog.WriteInfo("AddFiles - message is not in the currect format " + msg.Id);
                    return false;
                }
                try
                {
                    if (string.IsNullOrEmpty(msgData.BlobUrl))
                    {
                        return false;
                    }

                    var command = new AddFileToBoxCommand(msgData.UserId, msgData.BoxId, msgData.BlobUrl,
                       msgData.FileName,
                        msgData.Size.Value, msgData.TablId);
                   m_ZboxWriteService.AddFileToBox(command);
                    return true;
                }
                
                catch (Exception ex)
                {
                    TraceLog.WriteError("AddFiles run " + msg.Id, ex);
                }
                return false;


            }, TimeSpan.FromHours(1));
        }

        public void Stop()
        {
            m_KeepRunning = false;
        }
    }
}
