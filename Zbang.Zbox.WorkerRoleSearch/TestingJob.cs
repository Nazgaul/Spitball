using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.WindowsAzure.Storage;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Autofac;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.WorkerRoleSearch.DomainProcess;
using Zbang.Zbox.WorkerRoleSearch.Mail;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class TestingJob : IJob
    {
        private readonly IZboxWorkerRoleService m_ZboxWorkerRoleService;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly IBlobProvider2<FilesContainerName> m_BlobProvider;
        private readonly ILifetimeScope m_LifetimeScope;


        public TestingJob(IZboxWorkerRoleService zboxWorkerRoleService,
            IZboxReadServiceWorkerRole zboxReadService, IBlobProvider2<FilesContainerName> blobProvider, IZboxWriteService zboxWriteService, ILifetimeScope lifetimeScope)
        {
            m_ZboxWorkerRoleService = zboxWorkerRoleService;
            m_ZboxReadService = zboxReadService;
            m_BlobProvider = blobProvider;
            m_ZboxWriteService = zboxWriteService;
            m_LifetimeScope = lifetimeScope;
        }

        public string Name => nameof(TestingJob);
        public async Task RunAsync(CancellationToken cancellationToken)
        {

            var process = m_LifetimeScope.ResolveOptionalNamed<ISchedulerProcess>("downloadXml");
            await process.ExecuteAsync(10, (a, b) =>
            {
                return Task.CompletedTask;

            }, cancellationToken).ConfigureAwait(false);
            //var msgData = new BoxFileProcessData(70197);
            //var process = m_LifetimeScope.ResolveOptionalNamed<IFileProcess>(msgData.ProcessResolver);
            //var t =  await process.ExecuteAsync(msgData, cancellationToken).ConfigureAwait(false);

            //await RemoveDuplicatesFilesAsync().ConfigureAwait(false);
           // await Md5ProcessAsync(cancellationToken).ConfigureAwait(false);

            TraceLog.WriteInfo("one time job stop to work");
        }

        private async Task RemoveDuplicatesFilesAsync()
        {
            IEnumerable<Tuple<long, decimal>> documents;
            while ((documents = (await m_ZboxReadService.GetDuplicateDocumentsAsync().ConfigureAwait(false)).ToList())
                .Any())
            {
                TraceLog.WriteInfo($"md5 duplicates {string.Join(",", documents)}");
                foreach (var document in documents.Skip(1))
                {
                    var deleteItemCommand = new DeleteItemCommand(document.Item1, 1);
                    await m_ZboxWriteService.DeleteItemAsync(deleteItemCommand).ConfigureAwait(false);
                }
            }
        }

        private async Task Md5ProcessAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Tuple<long, string>> documents;
            var lastId = 0L;
            while ((documents = (await m_ZboxReadService.GetDocumentsWithoutMd5Async(lastId).ConfigureAwait(false)).ToList())
                .Any())
            {
                TraceLog.WriteInfo($"one time job process batch {string.Join(",", documents.Select(s => s.Item1))}");
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                foreach (var document in documents)
                {
                    try
                    {
                        lastId = document.Item1;
                        var md5 = await m_BlobProvider.MD5Async(document.Item2).ConfigureAwait(false);
                        var command = new UpdateThumbnailCommand(document.Item1, null,
                            null, md5);
                        m_ZboxWorkerRoleService.UpdateThumbnailPicture(command);
                    }
                    catch (StorageException ex)
                    {
                        if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                        {
                            var deleteItemCommand = new DeleteItemCommand(document.Item1, 1);
                            await m_ZboxWriteService.DeleteItemAsync(deleteItemCommand).ConfigureAwait(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        var telemetry = new TelemetryClient();
                        var properties = new Dictionary<string, string>
                            {{"section", "md5"}, {"itemId", document.Item1.ToString()}};

                        telemetry.TrackException(ex, properties);
                        TraceLog.WriteError($"error md5 item Id : {document.Item1}", ex);
                    }
                }
            }
        }
    }
}
