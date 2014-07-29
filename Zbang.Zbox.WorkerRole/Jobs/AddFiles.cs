using System;
using System.Threading;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class AddFiles : IJob
    {
        private bool m_KeepRunning;
        // ReSharper disable once NotAccessedField.Local
        private readonly IBlobProvider m_BlobProvider; // to trigger ctor
        private readonly QueueProcess m_QueueProcess;
        private readonly IZboxWriteService m_ZboxWriteService;


        public AddFiles(IQueueProvider queueProvider, IZboxWriteService zboxWriteService, IBlobProvider blobProvider)
        {
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
                    try
                    {
                        Execute();
                    }
                    catch (Exception ex)
                    {
                        TraceLog.WriteError("On Run AddFiles", ex);
                        Thread.Sleep(TimeSpan.FromSeconds(10));
                    }
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On Run AddFiles", ex);
                throw;
            }
        }

        private void Execute()
        {
            m_QueueProcess.RunQueue(new DownloadQueueName(), msg =>
            {
                var msgData = msg.FromMessageProto<UrlToDownloadData>();
                if (msgData == null)
                {
                    TraceLog.WriteInfo("AddFiles - message is not in the correct format " + msg.Id);
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
                      msgData.Size.HasValue ? msgData.Size.Value : 0, msgData.TablId);
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
