using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ApplicationInsights;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Azure.Queue;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.WorkerRoleSearch.DomainProcess;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class TestingJob : IJob
    {
        private readonly IZboxWorkerRoleService m_ZboxWorkerRoleService;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IMailComponent m_MailComponent;
        private readonly IQueueProviderExtract m_QueueProvider;
        private readonly ILifetimeScope m_LifetimeScope;
        private readonly IBlobProvider2<FilesContainerName> m_BlobProvider;


        public TestingJob(IZboxWorkerRoleService zboxWorkerRoleService, IMailComponent mailComponent, IQueueProviderExtract queueProvider, IZboxReadServiceWorkerRole zboxReadService, ILifetimeScope lifetimeScope, IBlobProvider2<FilesContainerName> blobProvider)
        {
            m_ZboxWorkerRoleService = zboxWorkerRoleService;
            m_MailComponent = mailComponent;
            m_QueueProvider = queueProvider;
            m_ZboxReadService = zboxReadService;
            m_LifetimeScope = lifetimeScope;
            m_BlobProvider = blobProvider;
        }

        public string Name => nameof(TestingJob);
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Tuple<long, string>> documents;
            var lastId = 0L;
            return;
            while ((documents = (await m_ZboxReadService.GetDocumentsWithoutMd5Async(lastId).ConfigureAwait(false)).ToList()).Any())
            {
                TraceLog.WriteInfo($"one time job process batch {string.Join( ",", documents.Select(s=>s.Item1))}");
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                foreach (var document in documents)
                {
                    try
                    {
                        lastId = document.Item1;
                        var md5 = await m_BlobProvider.Md5Async(document.Item2).ConfigureAwait(false);
                        var command = new UpdateThumbnailCommand(document.Item1, null,
                            null, md5);
                        m_ZboxWorkerRoleService.UpdateThumbnailPicture(command);
                    }
                    catch (Exception ex)
                    {
                        var telemetry = new TelemetryClient();
                        var properties = new Dictionary<string, string>
                            {{"section", "md5"}, {"itemId", document.Item1.ToString()}};

                        telemetry.TrackException(ex, properties);
                        await m_MailComponent.GenerateSystemEmailAsync("error md5", $"item Id : {document.Item1} ex {ex}").ConfigureAwait(false);
                    }
                }
            }
            TraceLog.WriteInfo("one time job stop to work");
        }
    }
}
