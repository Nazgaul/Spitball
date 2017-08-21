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
        private readonly IZboxWorkerRoleService m_ZboxWorkerRoleService;
        private readonly IMailComponent m_MailComponent;
        private readonly IMediaServicesProvider m_MediaService;
        private readonly ILogger m_Logger;

        public DeleteOldStuff(IZboxWorkerRoleService zboxWorkerRoleService, IMailComponent mailComponent, IMediaServicesProvider mediaService, ILogger logger)
        {
            m_ZboxWorkerRoleService = zboxWorkerRoleService;
            m_MailComponent = mailComponent;
            m_MediaService = mediaService;
            m_Logger = logger;
        }

        private async Task DoDeleteAsync(CancellationToken cancellationToken, string prefix, Func<CancellationToken, Task<int>> func)
        {
            var needLoop = true;
            // var mailContent = new StringBuilder();
            while (needLoop && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (func == null) continue;
                    var counter = await func(cancellationToken).ConfigureAwait(false);
                    m_Logger.TrackMetric(prefix, counter);
                    needLoop = false;
                    // mailContent.AppendLine($"{prefix} number: {counter}");
                }
                catch (Exception ex)
                {
                    m_Logger.Exception(ex);
                    await m_MailComponent.GenerateSystemEmailAsync("delete old stuff", ex.ToString()).ConfigureAwait(false);
                    // mailContent.AppendLine($"{prefix} exception: {ex}");
                    break;
                }
            }
            // return mailContent.ToString();
        }

        public async Task<bool> ExecuteAsync(int index, Func<int, TimeSpan, Task> progressAsync, CancellationToken token)
        {
            try
            {
                await m_MediaService.DeleteOldAssetsAsync().ConfigureAwait(false);
                m_Logger.Info("delete stuff starting to work");
                await m_ZboxWorkerRoleService.DoDirtyUpdateAsync(token).ConfigureAwait(false);

                await
                    DoDeleteAsync(token, "deleteOldUpdates",
                        m_ZboxWorkerRoleService.DeleteOldUpdatesAsync).ConfigureAwait(false);

                await
                    DoDeleteAsync(token, "deleteOldItems",
                        m_ZboxWorkerRoleService.DeleteOldItemAsync).ConfigureAwait(false);

                await
                    DoDeleteAsync(token, "DeleteOldFlashcard",
                        m_ZboxWorkerRoleService.DeleteOldFlashcardAsync).ConfigureAwait(false);

                await
                    DoDeleteAsync(token, "deleteOldQuiz",
                        m_ZboxWorkerRoleService.DeleteOldQuizAsync).ConfigureAwait(false);

                await
                    DoDeleteAsync(token, "deleteOldBoxes",
                        m_ZboxWorkerRoleService.DeleteOldBoxAsync).ConfigureAwait(false);

                await
                    DoDeleteAsync(token, "deleteOldUniversity",
                        m_ZboxWorkerRoleService.DeleteOldUniversityAsync).ConfigureAwait(false);
                //_Logger.TrackMetric("delete old stuff", result + result2 + result4 + result3 + result5 + result6);
                //await
                //    m_MailComponent.GenerateSystemEmailAsync("delete old stuff", result + result2 + result4 + result3 + result5 + result6).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                m_Logger.Exception(ex);
                await progressAsync.Invoke(0, TimeSpan.FromHours(1)).ConfigureAwait(false);
                return false;
            }
        }
    }
}
