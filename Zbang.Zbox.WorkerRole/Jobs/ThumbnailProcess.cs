using Microsoft.WindowsAzure.Storage;
using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.File;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer;
//using Microsoft.WindowsAzure.StorageClient;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class ThumbnailProcess : IJob
    {
        readonly private IZboxWriteService m_ZboxWriteService;
        readonly private IThumbnailProvider m_ThumbnailProvider;
        readonly private IQueueProvider m_QueueProvider;
        readonly private QueueProcess m_QueueProcess;
        private bool m_KeepRunning;
        readonly IShortCodesCache m_ShortToLongCode;
        private readonly IFileProcessorFactory m_FileProcessorFactory;


        public ThumbnailProcess(IThumbnailProvider thumbnailProvider,
            IQueueProvider queueProvider, IZboxWriteService zboxService, 
            IShortCodesCache shortToLongCode,
            IFileProcessorFactory fileProcessorFactory)
        {
            m_ZboxWriteService = zboxService;
            m_ThumbnailProvider = thumbnailProvider;
            m_QueueProvider = queueProvider;
            m_FileProcessorFactory = fileProcessorFactory;
            m_ShortToLongCode = shortToLongCode;
            m_QueueProcess = new QueueProcess(queueProvider, TimeSpan.FromSeconds(30));
        }
        public void Run()
        {
            try
            {
                m_KeepRunning = true;
                while (m_KeepRunning)
                {
                    Excecute();
                }
            }
            catch (Exception ex)
            {
                Zbang.Zbox.Infrastructure.Trace.TraceLog.WriteError("On Run ThumbnailProcess", ex);
                throw;
            }

        }

        private void Excecute()
        {

            m_QueueProcess.RunQueue(new ThumbnailQueueName(), msg =>
            {
                try
                {
                    var msgData = msg.FromMessage<GenerateThumbnail>();
                    var blobUri = new Uri(BlobProvider.GetBlobUrl(msgData.BlobName));
                    var processor = m_FileProcessorFactory.GetProcessor(blobUri);
                    if (processor != null)
                    {

                        var t = processor.PreProcessFile(blobUri);
                        t.Wait();
                        var retVal = t.Result;
                        if (retVal == null)
                        {
                            return true;
                        }
                        var oldBlobName = blobUri.Segments[blobUri.Segments.Length - 1];
                        var command = new UpdateThumbnailCommand(msgData.ItemId, retVal.ThumbnailName, retVal.BlobName, oldBlobName);
                        m_ZboxWriteService.UpdateThumbnailPicture(command);
                    }
                    return true;

                }
                catch (StorageException ex)
                {
                    TraceLog.WriteError("GenerateThumb storage exception run " + msg.Id, ex);
                    if (msg.DequeueCount > 3)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("GenerateThumb run " + msg.Id, ex);
                }
                return false;
            }, TimeSpan.FromMinutes(1));
        }

        public void Stop()
        {
            m_KeepRunning = false;
        }
    }
}
