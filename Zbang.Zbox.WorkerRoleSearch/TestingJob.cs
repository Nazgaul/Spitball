using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.ReadServices;
using Autofac;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.WorkerRoleSearch.Mail;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class TestingJob : IJob
    {
        private readonly IZboxWorkerRoleService _zboxWorkerRoleService;
        private readonly IZboxReadServiceWorkerRole _zboxReadService;
        private readonly IZboxWriteService _zboxWriteService;
        private readonly IBlobProvider2<FilesContainerName> _blobProvider2;
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ILogger _logger;

        public TestingJob(IZboxWorkerRoleService zboxWorkerRoleService,
            IZboxReadServiceWorkerRole zboxReadService, IBlobProvider2<FilesContainerName> blobProvider, IZboxWriteService zboxWriteService, ILifetimeScope lifetimeScope, ILogger logger)
        {
            _zboxWorkerRoleService = zboxWorkerRoleService;
            _zboxReadService = zboxReadService;
            _blobProvider2 = blobProvider;
            _zboxWriteService = zboxWriteService;
            _lifetimeScope = lifetimeScope;
            _logger = logger;
        }

        public string Name => nameof(TestingJob);
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            //var process = m_LifetimeScope.ResolveOptionalNamed<ISchedulerProcess>("careerBuilder");
            // ReSharper disable once AsyncConverter.AsyncAwaitMayBeElidedHighlighting
            //await process.ExecuteAsync(0, (a, b) => Task.CompletedTask, cancellationToken).ConfigureAwait(false);
            //_zboxWorkerRoleService.OneTimeDbi();
            var process = _lifetimeScope.ResolveKeyed<ISchedulerProcess>("careerBuilder");
            await process.ExecuteAsync(0, (a, b) => Task.CompletedTask, cancellationToken).ConfigureAwait(false);

            _logger.Info("finish test");

            //var msgData = new BoxFileProcessData(70197);
            //var process = m_LifetimeScope.ResolveOptionalNamed<IFileProcess>(msgData.ProcessResolver);
            //var t =  await process.ExecuteAsync(msgData, cancellationToken).ConfigureAwait(false);

            //await RemoveDuplicatesFilesAsync().ConfigureAwait(false);
            // await Md5ProcessAsync(cancellationToken).ConfigureAwait(false);
        }

        private async Task RemoveDuplicatesFilesAsync()
        {
            IEnumerable<Tuple<long, decimal>> documents;
            while ((documents = (await _zboxReadService.GetDuplicateDocumentsAsync().ConfigureAwait(false)).ToList())
                .Any())
            {
                foreach (var document in documents.Skip(1))
                {
                    var deleteItemCommand = new DeleteItemCommand(document.Item1, 1);
                    await _zboxWriteService.DeleteItemAsync(deleteItemCommand).ConfigureAwait(false);
                }
            }
        }

        private async Task Md5ProcessAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Tuple<long, string>> documents;
            var lastId = 0L;
            while ((documents = (await _zboxReadService.GetDocumentsWithoutMd5Async(lastId).ConfigureAwait(false)).ToList())
                .Any())
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                foreach (var document in documents)
                {
                    try
                    {
                        lastId = document.Item1;
                        var md5 = await _blobProvider2.MD5Async(document.Item2).ConfigureAwait(false);
                        var command = new UpdateThumbnailCommand(document.Item1, null,
                            null, md5);
                        _zboxWorkerRoleService.UpdateThumbnailPicture(command);
                    }
                    catch (StorageException ex)
                    {
                        if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                        {
                            var deleteItemCommand = new DeleteItemCommand(document.Item1, 1);
                            await _zboxWriteService.DeleteItemAsync(deleteItemCommand).ConfigureAwait(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        //var telemetry = new TelemetryClient();
                        //var properties = new Dictionary<string, string>
                        //{["section"] = "md5",["itemId"] = document.Item1.ToString() };

                        //telemetry.TrackException(ex, properties);
                    }
                }
            }
        }
    }
}
