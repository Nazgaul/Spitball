﻿using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class DeleteOldStuff : IJob
    {
        private readonly IZboxWorkerRoleService m_ZboxWorkerRoleService;
        private readonly IMailComponent m_MailComponent;

        public DeleteOldStuff(IZboxWorkerRoleService zboxWorkerRoleService, IMailComponent mailComponent)
        {
            m_ZboxWorkerRoleService = zboxWorkerRoleService;
            m_MailComponent = mailComponent;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await DeleteDatabaseOldAsync(cancellationToken).ConfigureAwait(false);
            }



        }

        private async Task DeleteDatabaseOldAsync(CancellationToken cancellationToken)
        {
            try
            {
                TraceLog.WriteInfo("delete stuff starting to work");
                await m_ZboxWorkerRoleService.DoDirtyUpdateAsync(cancellationToken).ConfigureAwait(false);
                var result =
                    await
                        DoDeleteAsync(cancellationToken, "deleteOldUpdates",
                            m_ZboxWorkerRoleService.DeleteOldUpdatesAsync).ConfigureAwait(false);
                var result2 =
                    await
                        DoDeleteAsync(cancellationToken, "deleteOldItems",
                            m_ZboxWorkerRoleService.DeleteOldItemAsync).ConfigureAwait(false);

                var result6 =
                   await
                       DoDeleteAsync(cancellationToken, "DeleteOldFlashcard",
                           m_ZboxWorkerRoleService.DeleteOldFlashcardAsync).ConfigureAwait(false);
                var result4 =
                    await
                        DoDeleteAsync(cancellationToken, "deleteOldQuiz",
                            m_ZboxWorkerRoleService.DeleteOldQuizAsync).ConfigureAwait(false);
                var result3 =
                    await
                        DoDeleteAsync(cancellationToken, "deleteOldBoxes",
                            m_ZboxWorkerRoleService.DeleteOldBoxAsync).ConfigureAwait(false);
                var result5 =
                    await
                        DoDeleteAsync(cancellationToken, "deleteOldUniversity",
                            m_ZboxWorkerRoleService.DeleteOldUniversityAsync).ConfigureAwait(false);
                await
                    m_MailComponent.GenerateSystemEmailAsync("delete old stuff", result + result2 + result4 + result3 + result5 + result6).ConfigureAwait(false);

                await Task.Delay(TimeSpan.FromDays(1), cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(ex);
                await Task.Delay(TimeSpan.FromHours(4), cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task<string> DoDeleteAsync(CancellationToken cancellationToken, string prefix, Func<CancellationToken, Task<int>> func)
        {
            var needLoop = true;
            var mailContent = new StringBuilder();
            while (needLoop && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (func != null)
                    {
                        var counter = await func(cancellationToken).ConfigureAwait(false);
                        needLoop = false;
                        mailContent.AppendLine($"{prefix} number: {counter}");
                    }
                }
                
                catch (Exception ex)
                {
                    TraceLog.WriteError("delete old updates", ex);
                    await m_MailComponent.GenerateSystemEmailAsync("delete old stuff", ex.ToString()).ConfigureAwait(false);
                    mailContent.AppendLine($"{prefix} exception: {ex}");
                    break;
                }
            }
            return mailContent.ToString();
        }
    }
}
