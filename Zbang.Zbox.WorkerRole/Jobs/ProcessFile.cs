using System;
using System.IO;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.File;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
//using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.FileConvert;
using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class ProcessFile : IJob
    {
        private readonly IBlobProvider m_BlobProvider;
        private readonly IQueueProvider m_QueueProvider;
        readonly private IZboxWriteService m_ZboxWriteService;
        private bool m_KeepRunning;
        private readonly QueueProcess m_QueueProcess;
        private readonly IFileProcessorFactory m_FileProcessorFactory;

        public ProcessFile(IBlobProvider blobProvider, IQueueProvider queueProvider,
            IFileProcessorFactory fileProcessorFactory,
            IZboxWriteService zboxService)
        {
            m_BlobProvider = blobProvider;
            m_QueueProvider = queueProvider;
            m_ZboxWriteService = zboxService;
            m_FileProcessorFactory = fileProcessorFactory;
            m_QueueProcess = new QueueProcess(queueProvider, TimeSpan.FromSeconds(30));
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
                Zbang.Zbox.Infrastructure.Trace.TraceLog.WriteError("On Run ProcessFile", ex);
                throw;
            }
        }

        private void Execute()
        {
            m_QueueProcess.RunQueue(new CacheQueueName(), msg =>
            {
                var msgData = msg.FromMessageProto<Zbang.Zbox.Infrastructure.Transport.FileProcessData>();
                if (msgData == null)
                {
                    TraceLog.WriteInfo("GenerateDocumentCache - message is not in the currect format " + msg.Id);
                    return true;

                }
                try
                {
                    var processor = m_FileProcessorFactory.GetProcessor(msgData.BlobName);
                    if (processor != null)
                    {
                        var t = processor.PreProcessFile(msgData.BlobName);
                        t.Wait();
                        var retVal = t.Result;
                        if (retVal == null)
                        {
                            return true;
                        }
                        var oldBlobName = msgData.BlobName.Segments[msgData.BlobName.Segments.Length - 1];
                        var command = new UpdateThumbnailCommand(msgData.ItemId, retVal.ThumbnailName, retVal.BlobName, oldBlobName, retVal.FileTextContent);
                        m_ZboxWriteService.UpdateThumbnailPicture(command);
                    }
                    return true;
                }
                catch (ItemNotFoundException ex)
                {
                    TraceLog.WriteError("GenerateDocumentCache Cache storage exception run " + msg.Id, ex);
                    if (msg.DequeueCount > 3)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("GenerateDocumentCache run " + msg.Id, ex);
                }
                return false;


            }, TimeSpan.FromHours(1));
        }

        //private void GenerateCopyRights(string blobName, IFileProcessor processor)
        //{
        //    var stream = m_BlobProvider.DownloadFile(blobName);
        //    var retVal = processor.GenerateCopyRight(stream);
        //    var blob = m_BlobProvider.GetFile(blobName);
        //    using (var ms = new MemoryStream(retVal, false))
        //    {
        //        blob.UploadFromStream(ms);
        //    }
        //    //blob.UploadByteArray(retVal);

        //}

        //private void GenerateFileCache(string blobName, IFileProcessor processor)
        //{
        //    var blobCacheName = string.Format("{0}V3.pdf", System.IO.Path.GetFileNameWithoutExtension(blobName));
        //    var stream = m_BlobProvider.DownloadFile(blobName);
        //    var retVal = processor.ConvertFileToWebSitePreview(stream, 0, 0);
        //    Compress compressor = new Compress();
        //    var byteArray = compressor.CompressToGzip(retVal);

        //    m_BlobProvider.UploadFileToCache(blobCacheName, byteArray, "application/pdf", true);

        //}

        public void Stop()
        {
            m_KeepRunning = false;
        }
    }


}
