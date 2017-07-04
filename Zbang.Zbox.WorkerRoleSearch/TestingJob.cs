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

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class TestingJob : IJob
    {
        private readonly IZboxWorkerRoleService m_ZboxWorkerRoleService;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly IBlobProvider2<FilesContainerName> m_BlobProvider;


        public TestingJob(IZboxWorkerRoleService zboxWorkerRoleService, 
            IZboxReadServiceWorkerRole zboxReadService, IBlobProvider2<FilesContainerName> blobProvider, IZboxWriteService zboxWriteService)
        {
            m_ZboxWorkerRoleService = zboxWorkerRoleService;
            m_ZboxReadService = zboxReadService;
            m_BlobProvider = blobProvider;
            m_ZboxWriteService = zboxWriteService;
        }

        public string Name => nameof(TestingJob);
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Tuple<long, string>> documents;
            var lastId = 0L;
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
            TraceLog.WriteInfo("one time job stop to work");
        }
    }
}
