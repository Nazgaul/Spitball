using System;
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
                try
                {
                    TraceLog.WriteInfo("delete stuff starting to work");
                    await m_MailComponent.GenerateSystemEmailAsync("delete stuff", "starting to work");
                    var result =
                        await
                            DoDeleteAsync(cancellationToken, "deleteOldUpdates",
                                m_ZboxWorkerRoleService.DeleteOldUpdatesAsync);
                    TraceLog.WriteInfo("delete stuff -finish delete old updates");
                    var result2 =
                        await
                            DoDeleteAsync(cancellationToken, "deleteOldItems",
                                m_ZboxWorkerRoleService.DeleteOldItemAsync);
                    //TraceLog.WriteInfo("delete stuff -finish delete items");
                    //var result3 =
                    //    await
                    //        DoDeleteAsync(cancellationToken, "deleteOldBoxes",
                    //            m_ZboxWorkerRoleService.DeleteOldBoxAsync);
                    //TraceLog.WriteInfo("delete stuff -finish delete boxes");
                    await m_MailComponent.GenerateSystemEmailAsync("delete stuff", result + result2 /*+ result3*/);

                    TraceLog.WriteInfo("delete stuff going to sleep");

                    await Task.Delay(TimeSpan.FromHours(6), cancellationToken);
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError(ex);
                }
            }



        }

        private async Task<string> DoDeleteAsync(CancellationToken cancellationToken, string prefix, Func<CancellationToken, Task<int>> func)
        {
            var needLoop = true;
            var mailContent = new StringBuilder();
            while (needLoop)
            {

                try
                {
                    var counter = await func(cancellationToken);
                    needLoop = false;
                    mailContent.AppendLine($"{prefix} number: {counter}");
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("delete old updates", ex);
                    //await m_MailComponent.GenerateSystemEmailAsync("delete stuff", ex.ToString());
                    mailContent.AppendLine($"{prefix} exception: {ex}");
                }
            }
            return mailContent.ToString();
        }
    }
}
