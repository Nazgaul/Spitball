using System;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Azure.Queue;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class ProcessFile : IJob
    {
        readonly private IZboxWriteService m_ZboxWriteService;
        private readonly IQueueProviderExtract m_QueueProvider;
        private readonly IFileProcessorFactory m_FileProcessorFactory;
        private int m_TimeToSleep = 1;

        private const string PrefixLog = "FileProcess";

        public ProcessFile(IZboxWriteService zboxWriteService, IFileProcessorFactory fileProcessorFactory, IQueueProviderExtract queueProvider)
        {
            m_ZboxWriteService = zboxWriteService;
            m_FileProcessorFactory = fileProcessorFactory;
            m_QueueProvider = queueProvider;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            TraceLog.WriteInfo(PrefixLog, "run job process file ");
            var cacheQueueName = new CacheQueueName();
            while (!cancellationToken.IsCancellationRequested)
            {
                TraceLog.WriteInfo(PrefixLog, "starting update process file cycle");
                var result = await m_QueueProvider.RunQueueAsync(cacheQueueName, msg =>
                  {
                      var msgData = msg.FromMessageProto<FileProcessData>();
                      if (msgData == null)
                      {
                          TraceLog.WriteInfo(PrefixLog, "GenerateDocumentCache - message is not in the correct format ");
                          return Task.FromResult(true);

                      }
                      TraceLog.WriteInfo(PrefixLog, "Processing from queue file: " + msgData.ItemId);
                      try
                      {
                          return Task.FromResult(PreProcessFile(msgData));
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
                  }, TimeSpan.FromHours(1), 5);
                if (result)
                {
                    m_TimeToSleep = 1;
                    TraceLog.WriteInfo(PrefixLog, "finish update process file cycle with result sleeping:" + m_TimeToSleep);
                    await Task.Delay(TimeSpan.FromSeconds(m_TimeToSleep), cancellationToken);
                }
                else
                {
                    m_TimeToSleep = m_TimeToSleep * 2;
                    TraceLog.WriteInfo(PrefixLog, "finish update process file cycle without result sleeping: " + m_TimeToSleep);
                    await Task.Delay(TimeSpan.FromSeconds(m_TimeToSleep), cancellationToken);
                }
            }
        }

        private bool PreProcessFile(FileProcessData msgData)
        {
            var processor = m_FileProcessorFactory.GetProcessor(msgData.BlobName);
            if (processor == null) return true;
            //taken from : http://blogs.msdn.com/b/nikhil_agarwal/archive/2014/04/02/10511934.aspx
            var wait = new ManualResetEvent(false);

            var work = new Thread(async () =>
            {
                try
                {
                    var tokenSource = new CancellationTokenSource();
                    tokenSource.CancelAfter(TimeSpan.FromMinutes(29));
                    //some long running method requiring synchronization
                    var retVal = await processor.PreProcessFile(msgData.BlobName, tokenSource.Token);

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
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("on itemid: " + msgData.ItemId, ex);
                    wait.Set();
                }
            });
            work.Start();
            Boolean signal = wait.WaitOne(TimeSpan.FromMinutes(30));
            if (!signal)
            {
                work.Abort();
                TraceLog.WriteError("blob url aborting process" + msgData.BlobName);
            }
            return true;
        }
    }
}
