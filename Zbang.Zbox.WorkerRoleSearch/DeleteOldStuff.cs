using System;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.MediaServices;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.WorkerRoleSearch.Mail;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class DeleteOldStuff : ISchedulerProcess
    {
        private readonly IZboxWorkerRoleService _zboxWorkerRoleService;
        private readonly IMailComponent _mailComponent;
        private readonly IMediaServicesProvider _mediaService;
        private readonly ILogger _logger;

        public DeleteOldStuff(IZboxWorkerRoleService zboxWorkerRoleService, IMailComponent mailComponent,
            IMediaServicesProvider mediaService, ILogger logger)
        {
            _zboxWorkerRoleService = zboxWorkerRoleService;
            _mailComponent = mailComponent;
            _mediaService = mediaService;
            _logger = logger;
        }

        private async Task DoDeleteAsync(CancellationToken cancellationToken, string prefix, Func<CancellationToken, Task<int>> func)
        {
            var needLoop = true;
            while (needLoop && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (func == null) continue;
                    var counter = await func(cancellationToken).ConfigureAwait(false);
                    _logger.TrackMetric(prefix, counter);
                    needLoop = false;
                }
                catch (TaskCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.Exception(ex);
                    await _mailComponent.GenerateSystemEmailAsync("delete old stuff", ex.ToString()).ConfigureAwait(false);
                    break;
                }
            }
            // return mailContent.ToString();
        }

        public async Task<bool> ExecuteAsync(int index, Func<int, TimeSpan, Task> progressAsync, CancellationToken token)
        {
            using (var timeoutToken = new CancellationTokenSource(TimeSpan.FromMinutes(20)))
            {
                var newToken = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutToken.Token);
                try
                {
                    //await _mediaService.DeleteOldAssetsAsync().ConfigureAwait(false);
                    //_logger.Info("delete stuff starting to work");
                   // await _zboxWorkerRoleService.DoDirtyUpdateAsync(newToken.Token).ConfigureAwait(false);

                    //await
                    //    DoDeleteAsync(newToken.Token, "deleteOldUpdates",
                    //        _zboxWorkerRoleService.DeleteOldUpdatesAsync).ConfigureAwait(false);

                    await
                        DoDeleteAsync(newToken.Token, "deleteOldItems",
                            _zboxWorkerRoleService.DeleteOldItemAsync).ConfigureAwait(false);

                    await
                        DoDeleteAsync(newToken.Token, "DeleteOldFlashcard",
                            _zboxWorkerRoleService.DeleteOldFlashcardAsync).ConfigureAwait(false);

                    await
                        DoDeleteAsync(newToken.Token, "deleteOldQuiz",
                            _zboxWorkerRoleService.DeleteOldQuizAsync).ConfigureAwait(false);

                  await
                        DoDeleteAsync(newToken.Token, "deleteOldBoxes",
                            _zboxWorkerRoleService.DeleteOldBoxAsync).ConfigureAwait(false);

                    await
                        DoDeleteAsync(newToken.Token, "deleteOldUniversity",
                            _zboxWorkerRoleService.DeleteOldUniversityAsync).ConfigureAwait(false);
                    return true;
                }
                catch (TaskCanceledException)
                {
                    await progressAsync.Invoke(0, TimeSpan.FromHours(1)).ConfigureAwait(false);
                    return false;
                }
                catch (Exception ex)
                {
                    _logger.Exception(ex);
                    await progressAsync.Invoke(0, TimeSpan.FromHours(1)).ConfigureAwait(false);
                    return false;
                }
            }
        }
    }
}
