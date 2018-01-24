using System;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Storage;
using Cloudents.Core.Interfaces;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.DomainProcess
{
    public class PreProcessFile : IFileProcess
    {
        private readonly IFileProcessorFactory m_FileProcessorFactory;
        private readonly IMailComponent m_MailComponent;
        private readonly ILogger _logger;

        public PreProcessFile(IFileProcessorFactory fileProcessorFactory, IMailComponent mailComponent, ILogger logger)
        {
            m_FileProcessorFactory = fileProcessorFactory;
            m_MailComponent = mailComponent;
            _logger = logger;
        }

        public async Task<bool> ExecuteAsync(FileProcess data, CancellationToken token)
        {
            var parameters = data as ChatFileProcessData;
            if (parameters == null) return true;// Infrastructure.Extensions.TaskExtensions.CompletedTaskTrue;

            var processor = m_FileProcessorFactory.GetProcessor<PreviewChatContainerName>(parameters.BlobUri);

            //if (await m_BlobProvider.ExistsAsync(parameters.BlobUri))
            //{
            //    return true;
            //}
            ProcessBlob(processor, parameters);

            try
            {
                var proxy = await SignalrClient.GetProxyAsync().ConfigureAwait(false);

                var blobName = parameters.BlobUri.Segments[parameters.BlobUri.Segments.Length - 1];
                if (parameters.Users != null)
                {
                    await proxy.Invoke("UpdateImage", blobName, parameters.Users).ConfigureAwait(false);
                }
                else
                {
                    _logger.Error($"users is null on {blobName}");
                }
            }
            catch (Exception ex)
            {
                await m_MailComponent.GenerateSystemEmailAsync("signalR error", ex.Message).ConfigureAwait(false);
                _logger.Exception(ex);
            }

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
                    var retVal = await processor.PreProcessFileAsync(parameters.BlobUri, tokenSource.Token).ConfigureAwait(false);
                    if (retVal == null)
                    {
                        wait.Set();
                        return;
                    }

                    wait.Set();
                }
                catch (Exception ex)
                {
                    _logger.Exception(ex);
                    wait.Set();
                }
            });
            work.Start();
            var signal = wait.WaitOne(TimeSpan.FromMinutes(10));
            if (!signal)
            {
                work.Abort();
                _logger.Error($"blob url aborting process {parameters.BlobUri}");
            }
        }
    }
}
