using System;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.File;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class ProcessFile : IJob
    {
        readonly private IZboxWriteService m_ZboxWriteService;
        private bool m_KeepRunning;
        private readonly QueueProcess m_QueueProcess;
        private readonly IFileProcessorFactory m_FileProcessorFactory;

        public ProcessFile(IQueueProvider queueProvider,
            IFileProcessorFactory fileProcessorFactory,
            IZboxWriteService zboxService)
        {
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
                TraceLog.WriteError("On Run ProcessFile", ex);
                throw;
            }
        }

        private void Execute()
        {
            m_QueueProcess.RunQueue(new CacheQueueName(), msg =>
            {
                var msgData = msg.FromMessageProto<Infrastructure.Transport.FileProcessData>();
                if (msgData == null)
                {
                    TraceLog.WriteInfo("GenerateDocumentCache - message is not in the currect format " + msg.Id);
                    return true;

                }
                try
                {
                    var processor = m_FileProcessorFactory.GetProcessor(msgData.BlobName);
                    if (processor == null) return true;

                    var tokenSource = new CancellationTokenSource();
                    tokenSource.CancelAfter(TimeSpan.FromMinutes(240));
                    using (var t = Task.Factory.StartNew(() => processor.PreProcessFile(msgData.BlobName),
                            tokenSource.Token))
                    {


                       // var t = processor.PreProcessFile();
                        t.Wait(tokenSource.Token);
                        var retVal = t.Result.Result;
                        if (retVal == null)
                        {
                            return true;
                        }
                        var oldBlobName = msgData.BlobName.Segments[msgData.BlobName.Segments.Length - 1];
                        var command = new UpdateThumbnailCommand(msgData.ItemId, retVal.ThumbnailName, retVal.BlobName,
                            oldBlobName, retVal.FileTextContent);
                        m_ZboxWriteService.UpdateThumbnailPicture(command);
                        return true;
                    }
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


            }, TimeSpan.FromHours(1), 10);
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
