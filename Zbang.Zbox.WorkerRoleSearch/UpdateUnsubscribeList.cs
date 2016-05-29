﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly IIntercomApiManager m_IntercomManager;
        private DateTime m_DateTime;
        private string m_LeaseId = string.Empty;
        private readonly TimeSpan m_SleepTime = TimeSpan.FromMinutes(30);
        private const string Prefix = "SendGridApi";
        private readonly IMailComponent m_MailComponent;

        private CloudBlockBlob m_Blob;

        private readonly IEnumerable<JobPerApi> m_Jobs;

        public UpdateUnsubscribeList(IMailComponent mailComponent, IZboxWorkerRoleService zboxWorkerRoleService, ICloudBlockProvider cloudBlockProvider, IIntercomApiManager intercomManager)
        {
            m_ZboxWorkerRoleService = zboxWorkerRoleService;
            m_CloudBlockProvider = cloudBlockProvider;
            m_IntercomManager = intercomManager;
            m_DateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            m_Jobs = new List<JobPerApi>
            {
                new JobPerApi {Func = mailComponent.GetUnsubscribesAsync, Type = EmailSend.Unsubscribe},
                new JobPerApi {Func = mailComponent.GetInvalidEmailsAsync, Type = EmailSend.Invalid},
            };
            m_MailComponent = mailComponent;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {


            while (!cancellationToken.IsCancellationRequested)
            {
                m_Blob = await ReadBlobDataAsync(cancellationToken);
                try
                {
                    TraceLog.WriteInfo($"{Prefix} running unsubscribe process");
                    if (DateTime.UtcNow.AddDays(-1) < m_DateTime.AddHours(6))
                    {
                        TraceLog.WriteInfo($"{Prefix} running unsubscribe ran recently going to sleep {DateTime.UtcNow.AddDays(-1)} < {m_DateTime.AddHours(6)}");
                        await Task.Delay(m_SleepTime, cancellationToken);
                        continue;
                    }
                    try
                    {
                        m_LeaseId = await m_Blob.AcquireLeaseAsync(TimeSpan.FromSeconds(60), null, cancellationToken);
                    }
                    catch (StorageException e)
                    {
                        if (e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict)
                        {
                            TraceLog.WriteInfo($"{Prefix} running unsubscribe is locked going to sleep");
                            await Task.Delay(m_SleepTime, cancellationToken);
                            continue;
                        }
                    }

                    //var needToContinueRun = true;
                    TraceLog.WriteInfo($"{Prefix} update unsubscribe list data {m_DateTime}");
                    var mailContent = new StringBuilder();
                    //await m_MailComponent.GenerateSystemEmailAsync("update satellite", $"{Prefix} starting to run ");
                    foreach (var job in m_Jobs)
                    {
                        var page = 0;
                        while (true)
                        {

                            var result = await job.Func(m_DateTime, page++, cancellationToken);
                            var resultList = result.ToList();
                            if (resultList.Count == 0)
                            {
                                mailContent.AppendLine($"{job.Type} got to page {page}");
                                break;
                            }
                            m_ZboxWorkerRoleService.UpdateUserFromUnsubscribe(
                                new Domain.Commands.UnsubscribeUsersFromEmailCommand(resultList,
                                    job.Type));
                            await RenewLeaseAsync(cancellationToken);
                        }
                    }
                    var sw = new Stopwatch();
                    sw.Start();
                    await ProcessIntercomAsync(cancellationToken);
                    sw.Stop();
                    mailContent.AppendLine($"process intercom finish and took {sw.ElapsedMilliseconds}");
                    sw.Restart();
                    var counter = m_ZboxWorkerRoleService.DeleteOldUpdates();
                    sw.Stop();

                    mailContent.AppendLine($"delete old updates number: {counter} and took {sw.ElapsedMilliseconds}");
                    await RenewLeaseAsync(cancellationToken);
                    sw.Restart();
                    m_ZboxWorkerRoleService.UpdateUniversityStats(m_DateTime);
                    sw.Stop();
                    mailContent.AppendLine($"update university took {sw.ElapsedMilliseconds}");
                    await RenewLeaseAsync(cancellationToken);


                    TraceLog.WriteInfo($"{Prefix} update unsubscribe list complete");
                    m_DateTime = DateTime.UtcNow.AddDays(-10);
                    await m_Blob.UploadTextAsync(m_DateTime.ToFileTimeUtc().ToString(), Encoding.Default, new AccessCondition { LeaseId = m_LeaseId }, new BlobRequestOptions(), new OperationContext(), cancellationToken);
                    await ReleaseLeaseAsync(cancellationToken);
                    await m_MailComponent.GenerateSystemEmailAsync("sendgrid api", mailContent.ToString());
                    await Task.Delay(m_SleepTime, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    if (!string.IsNullOrEmpty(m_LeaseId))
                    {
                        // ReSharper disable once MethodSupportsCancellation task is cancelled so we not transfer that
                        await ReleaseLeaseAsync();
                    }
                }
                catch (Exception ex)
                {
                    await m_MailComponent.GenerateSystemEmailAsync("sendgrid api", $"{Prefix} with errors {ex}");
                    TraceLog.WriteError("unsubscribe list ", ex);
                }


            }
        }

        private async Task ProcessIntercomAsync(CancellationToken token)
        {
            var page = 1;
            while (true)
            {
                var list = (await m_IntercomManager.GetUnsubscribersAsync(page++, token)).ToList();
                if (!list.Any())
                {
                    break;
                }
                m_ZboxWorkerRoleService.UpdateUserFromUnsubscribe(
                               new Domain.Commands.UnsubscribeUsersFromEmailCommand(list.Select(s=>s.Email),
                                   EmailSend.Unsubscribe));
                await RenewLeaseAsync(token);

            }
        }

        private Task RenewLeaseAsync(CancellationToken cancellationToken)
        {
            return m_Blob.RenewLeaseAsync(new AccessCondition { LeaseId = m_LeaseId }, cancellationToken);
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

        private async Task ReleaseLeaseAsync( CancellationToken cancellationToken = default(CancellationToken))
        {
            await m_Blob.ReleaseLeaseAsync(new AccessCondition
            {
                LeaseId = m_LeaseId
            }, cancellationToken);
            m_Blob = null;
            m_LeaseId = string.Empty;
        }

        public void Stop()
        {
        }

        public class JobPerApi
        {
            public Func<DateTime, int, CancellationToken, Task<IEnumerable<string>>> Func { get; set; }
            public EmailSend Type { get; set; }
        }



    }
}
