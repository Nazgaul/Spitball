using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.DomainProcess
{
    public class PreProcessFile : IFileProcess
    {
        private readonly IFileProcessorFactory m_FileProcessorFactory;
        private readonly IBlobProvider m_BlobProvider;


        public PreProcessFile(IFileProcessorFactory fileProcessorFactory, 
             IBlobProvider blobProvider)
        {
            m_FileProcessorFactory = fileProcessorFactory;
            m_BlobProvider = blobProvider;
        }

        public async Task<bool> ExecuteAsync(FileProcess data, CancellationToken token)
        {
            var parameters = data as ChatFileProcessData;
            if (parameters == null) return true;

            var processor = m_FileProcessorFactory.GetProcessor<PreviewChatContainerName>(parameters.BlobUri);
            
            if (await m_BlobProvider.ExistsAsync(parameters.BlobUri))
            {
                return true;
            }
            ProcessBlob(processor, parameters);
            return true;
        }

        private void ProcessBlob(IContentProcessor processor, ChatFileProcessData parameters)
        {
            //taken from : http://blogs.msdn.com/b/nikhil_agarwal/archive/2014/04/02/10511934.aspx
            var wait = new ManualResetEvent(false);

            var work = new Thread(async () =>
            {
                try
                {
                    var tokenSource = new CancellationTokenSource();
                    tokenSource.CancelAfter(TimeSpan.FromMinutes(10));
                    //some long running method requiring synchronization
                    var retVal = await processor.PreProcessFileAsync(parameters.BlobUri, tokenSource.Token);
                    if (retVal == null)
                    {
                        wait.Set();
                        return;
                    }

                    wait.Set();
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError($"on blob uri: {parameters.BlobUri}", ex);
                    wait.Set();
                }
            });
            work.Start();
            var signal = wait.WaitOne(TimeSpan.FromMinutes(10));
            if (!signal)
            {
                work.Abort();
                TraceLog.WriteError($"blob url aborting process {parameters.BlobUri}");
            }
        }
    }
}
