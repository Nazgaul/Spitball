using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Azure.Blob;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateUnsubscribeList : IJob
    {
        private readonly IZboxWorkerRoleService m_ZboxWorkerRoleService;
        private readonly ICloudBlockProvider m_CloudBlockProvider;
        private DateTime m_DateTime;
        private string m_LeaseId = string.Empty;
        private readonly TimeSpan m_SleepTime = TimeSpan.FromMinutes(30);

        private readonly IEnumerable<JobPerApi> m_Jobs;

        public UpdateUnsubscribeList(IMailComponent mailComponent, IZboxWorkerRoleService zboxWorkerRoleService, ICloudBlockProvider cloudBlockProvider)
        {
            m_ZboxWorkerRoleService = zboxWorkerRoleService;
            m_CloudBlockProvider = cloudBlockProvider;
            m_DateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            m_Jobs = new List<JobPerApi>
            {
                new JobPerApi {Func = mailComponent.GetUnsubscribesAsync, Type = EmailSend.Unsubscribe},
                new JobPerApi {Func = mailComponent.GetInvalidEmailsAsync, Type = EmailSend.Invalid},
            };
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {


            while (!cancellationToken.IsCancellationRequested)
            {
                var blob = await ReadBlobDataAsync(cancellationToken);
                try
                {
                    TraceLog.WriteInfo($"running unsubscribe process");
                    if (DateTime.UtcNow.AddDays(-1) < m_DateTime.AddHours(6))
                    {
                        TraceLog.WriteInfo($"running unsubscribe ran recently going to sleep");
                        await Task.Delay(m_SleepTime, cancellationToken);
                        continue;
                    }
                    try
                    {
                        m_LeaseId = await blob.AcquireLeaseAsync(TimeSpan.FromSeconds(60), null, cancellationToken);
                    }
                    catch (StorageException e)
                    {
                        if (e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict)
                        {
                            TraceLog.WriteInfo($"running unsubscribe is locked going to sleep");
                            await Task.Delay(m_SleepTime, cancellationToken);
                            continue;
                        }
                    }

                    //var needToContinueRun = true;
                    TraceLog.WriteInfo($"update unsubscribe list data {m_DateTime}");
                    
                    foreach (var job in m_Jobs)
                    {
                        var page = 0;
                        while (true)
                        {

                            var result = await job.Func(m_DateTime, page++, cancellationToken);
                            var resultList = result.ToList();
                            if (resultList.Count == 0)
                            {
                                break;
                            }
                            m_ZboxWorkerRoleService.UpdateUserFromUnsubscribe(
                                new Domain.Commands.UnsubscribeUsersFromEmailCommand(resultList,
                                    job.Type));
                            var acc = new AccessCondition {LeaseId = m_LeaseId};
                            await blob.RenewLeaseAsync(acc, cancellationToken);
                        }
                    }
                    TraceLog.WriteInfo("update unsubscribe list complete");
                    m_DateTime = DateTime.UtcNow.AddDays(-1);
                    await blob.UploadTextAsync(m_DateTime.ToFileTimeUtc().ToString(), Encoding.Default, new AccessCondition { LeaseId = m_LeaseId }, new BlobRequestOptions(), new OperationContext(), cancellationToken);
                    await ReleaseLeaseAsync(blob, cancellationToken);

                    await Task.Delay(m_SleepTime, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    if (!string.IsNullOrEmpty(m_LeaseId))
                    {
                        // ReSharper disable once MethodSupportsCancellation task is cancelled so we not transfer that
                        await ReleaseLeaseAsync(blob);
                    }
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("unsubscribe list ", ex);
                }


            }
        }

        private async Task<CloudBlockBlob> ReadBlobDataAsync(CancellationToken cancellationToken)
        {
            // ReSharper disable once StringLiteralTypo
            var blob = m_CloudBlockProvider.GetFile("sendGridApiQuery", "zboxidgenerator");
            if (!await blob.ExistsAsync(cancellationToken))
            {

                await blob.UploadTextAsync(m_DateTime.ToFileTimeUtc().ToString(), cancellationToken);
            }
            else
            {
                var txt = await blob.DownloadTextAsync(cancellationToken);
                long num;
                if (long.TryParse(txt, out num))
                {
                    m_DateTime = DateTime.FromFileTimeUtc(num);
                }
            }
            return blob;
        }

        private async Task ReleaseLeaseAsync(CloudBlockBlob blob, CancellationToken cancellationToken = default(CancellationToken))
        {
            await blob.ReleaseLeaseAsync(new AccessCondition
            {
                LeaseId = m_LeaseId
            }, cancellationToken);
            m_LeaseId = string.Empty;
        }

        public void Stop()
        {
        }

        public class JobPerApi
        {
            public Func<DateTime,int,CancellationToken,Task<IEnumerable<string>>> Func { get; set; }
            public EmailSend Type { get; set; }
        }



    }
}
