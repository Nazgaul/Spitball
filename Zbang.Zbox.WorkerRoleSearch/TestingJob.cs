using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Interfaces;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class TestingJob : IJob
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ILogger _logger;

        public TestingJob(ILifetimeScope lifetimeScope, ILogger logger)
        {
            _lifetimeScope = lifetimeScope;
            _logger = logger;
        }

        public string Name => nameof(TestingJob);
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var process = _lifetimeScope.ResolveKeyed<ISchedulerProcess>("spamGun");
            await process.ExecuteAsync(0, (a, b) => Task.CompletedTask, cancellationToken).ConfigureAwait(false);
            _logger.Info("finish test");
        }

        //private async Task RemoveDuplicatesFilesAsync()
        //{
        //    IEnumerable<Tuple<long, decimal>> documents;
        //    while ((documents = (await _zboxReadService.GetDuplicateDocumentsAsync().ConfigureAwait(false)).ToList())
        //        .Any())
        //    {
        //        foreach (var document in documents.Skip(1))
        //        {
        //            var deleteItemCommand = new DeleteItemCommand(document.Item1, 1);
        //            await _zboxWriteService.DeleteItemAsync(deleteItemCommand).ConfigureAwait(false);
        //        }
        //    }
        //}

        //private async Task Md5ProcessAsync(CancellationToken cancellationToken)
        //{
        //    IEnumerable<Tuple<long, string>> documents;
        //    var lastId = 0L;
        //    while ((documents = (await _zboxReadService.GetDocumentsWithoutMd5Async(lastId).ConfigureAwait(false)).ToList())
        //        .Any())
        //    {
        //        if (cancellationToken.IsCancellationRequested)
        //        {
        //            break;
        //        }

        //        foreach (var document in documents)
        //        {
        //            try
        //            {
        //                lastId = document.Item1;
        //                var md5 = await _blobProvider2.MD5Async(document.Item2).ConfigureAwait(false);
        //                var command = new UpdateThumbnailCommand(document.Item1, null,
        //                    null, md5);
        //                _zboxWorkerRoleService.UpdateThumbnailPicture(command);
        //            }
        //            catch (StorageException ex)
        //            {
        //                if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
        //                {
        //                    var deleteItemCommand = new DeleteItemCommand(document.Item1, 1);
        //                    await _zboxWriteService.DeleteItemAsync(deleteItemCommand).ConfigureAwait(false);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                var properties = new Dictionary<string, string>
        //                {["section"] = "md5",["itemId"] = document.Item1.ToString() };
        //                _logger.Exception(ex, properties);
        //            }
        //        }
        //    }
        //}
    }
}
