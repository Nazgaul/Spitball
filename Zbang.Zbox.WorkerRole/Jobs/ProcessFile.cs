﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Azure.Queue;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.File;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class ProcessFile : IJob
    {
        readonly private IZboxWriteService m_ZboxWriteService;
        private bool m_KeepRunning;
        private readonly QueueProcess m_QueueProcess;
        private readonly IFileProcessorFactory m_FileProcessorFactory;

        public ProcessFile(IQueueProviderExtract queueProvider,
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
                    TraceLog.WriteInfo("Running process file");
                    ExecuteAsync().Wait();
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On Run ProcessFile", ex);
                throw;
            }
        }

        private async Task ExecuteAsync()
        {
            await m_QueueProcess.RunQueue(new CacheQueueName(), msg =>
            {
                var msgData = msg.FromMessageProto<FileProcessData>();
                if (msgData == null)
                {
                    TraceLog.WriteInfo("GenerateDocumentCache - message is not in the correct format ");
                    return Task.FromResult(true);

                }
                TraceLog.WriteInfo("Processing file: " + msgData.ItemId);
                try
                {
                    return PreProcessFile(msgData);
                }
                catch (ItemNotFoundException ex)
                {
                    TraceLog.WriteError("GenerateDocumentCache Cache storage exception run ", ex);
                    if (msg.DequeueCount > 3)
                    {
                        return Task.FromResult(true);
                    }
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("GenerateDocumentCache run ", ex);
                }
                return Task.FromResult(false);
            }, TimeSpan.FromHours(1), 10);
        }

        private Task<bool> PreProcessFile(FileProcessData msgData)
        {
            var processor = m_FileProcessorFactory.GetProcessor(msgData.BlobName);
            if (processor == null) return Task.FromResult(true);
            //taken from : http://blogs.msdn.com/b/nikhil_agarwal/archive/2014/04/02/10511934.aspx
            var wait = new ManualResetEvent(false);

            var work = new Thread(async () =>
            {
                //some long running method requiring synchronization
                var retVal = await processor.PreProcessFile(msgData.BlobName);

                if (retVal == null)
                {
                    wait.Set();
                    return;
                }
                var oldBlobName = msgData.BlobName.Segments[msgData.BlobName.Segments.Length - 1];
                var command = new UpdateThumbnailCommand(msgData.ItemId, retVal.ThumbnailName, retVal.BlobName,
                    oldBlobName, retVal.FileTextContent);
                m_ZboxWriteService.UpdateThumbnailPicture(command);
                wait.Set();
            });
            work.Start();
            Boolean signal = wait.WaitOne(TimeSpan.FromMinutes(30));
            if (!signal)
            {
                work.Abort();
                TraceLog.WriteError("blob url aborting process" + msgData.BlobName);
            }
            return Task.FromResult(true);
        }
        //private async Task Execute()
        //{
        //    await m_QueueProcess.RunQueue(new CacheQueueName(), msg =>
        //     {
        //         var msgData = msg.FromMessageProto<FileProcessData>();
        //         if (msgData == null)
        //         {
        //             TraceLog.WriteInfo("GenerateDocumentCache - message is not in the correct format " + msg.Id);
        //             return Task.FromResult(true);

        //         }
        //         try
        //         {
        //             var processor = m_FileProcessorFactory.GetProcessor(msgData.BlobName);
        //             if (processor == null) return Task.FromResult(true);

        //             var tokenSource = new CancellationTokenSource();
        //             tokenSource.CancelAfter(TimeSpan.FromMinutes(20));
        //             var t = Task.Factory.StartNew(async () => await processor.PreProcessFile(msgData.BlobName),
        //                 tokenSource.Token);


        //             t.Wait(tokenSource.Token);
        //             var retVal = t.Result.Result;
        //             if (retVal == null)
        //             {
        //                 return Task.FromResult(true);
        //             }
        //             var oldBlobName = msgData.BlobName.Segments[msgData.BlobName.Segments.Length - 1];
        //             var command = new UpdateThumbnailCommand(msgData.ItemId, retVal.ThumbnailName, retVal.BlobName,
        //                 oldBlobName, retVal.FileTextContent);
        //             m_ZboxWriteService.UpdateThumbnailPicture(command);
        //             return Task.FromResult(true);

        //         }
        //         catch (ItemNotFoundException ex)
        //         {
        //             TraceLog.WriteError("GenerateDocumentCache Cache storage exception run " + msg.Id, ex);
        //             if (msg.DequeueCount > 3)
        //             {
        //                 return Task.FromResult(true);
        //             }
        //         }
        //         catch (Exception ex)
        //         {
        //             TraceLog.WriteError("GenerateDocumentCache run " + msg.Id, ex);
        //         }
        //         return Task.FromResult(false);


        //     }, TimeSpan.FromHours(1), 10);
        //}

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
