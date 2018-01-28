using System;
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
using Cloudents.Core.Interfaces;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateUnsubscribeList : IJob
    {
        private readonly IZboxWorkerRoleService _zboxWorkerRoleService;
        private readonly ICloudBlockProvider _cloudBlockProvider;
        private readonly IIntercomApiManager _intercomManager;
        private DateTime _dateTime;
        private string _leaseId = string.Empty;
        private readonly TimeSpan _sleepTime = TimeSpan.FromMinutes(30);
        //private readonly IMailComponent _mMailComponent;
        private readonly ILogger _logger;

        private CloudBlockBlob _mBlob;

        private readonly IEnumerable<JobPerApi> _mJobs;

        public string Name => nameof(UpdateUnsubscribeList);
        public UpdateUnsubscribeList(IMailComponent mailComponent, IZboxWorkerRoleService zboxWorkerRoleService, ICloudBlockProvider cloudBlockProvider, IIntercomApiManager intercomManager, ILogger logger)
        {
            _zboxWorkerRoleService = zboxWorkerRoleService;
            _cloudBlockProvider = cloudBlockProvider;
            _intercomManager = intercomManager;
            _logger = logger;
            _dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            _mJobs = new List<JobPerApi>
            {
                new JobPerApi {Func = mailComponent.GetUnsubscribesAsync, Type = EmailSend.Unsubscribe},
                new JobPerApi {Func = mailComponent.GetInvalidEmailsAsync, Type = EmailSend.Invalid},
                new JobPerApi {Func = mailComponent.GetBouncesAsync, Type = EmailSend.Bounce},
            };
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _mBlob = await ReadBlobDataAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    //TraceLog.WriteInfo($"{Prefix} running unsubscribe process");
                    if (DateTime.UtcNow.AddDays(-1) < _dateTime.AddHours(6))
                    {
                        //TraceLog.WriteInfo($"{Name} running unsubscribe ran recently going to sleep {DateTime.UtcNow.AddDays(-1)} < {m_DateTime.AddHours(6)}");
                        await Task.Delay(_sleepTime, cancellationToken).ConfigureAwait(false);
                        continue;
                    }
                    try
                    {
                        _leaseId = await _mBlob.AcquireLeaseAsync(TimeSpan.FromSeconds(60), null, cancellationToken).ConfigureAwait(false);
                    }
                    catch (StorageException e)
                    {
                        if (e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict)
                        {
                           // TraceLog.WriteInfo($"{Prefix} running unsubscribe is locked going to sleep");
                            await Task.Delay(_sleepTime, cancellationToken).ConfigureAwait(false);
                            continue;
                        }
                    }

                    _logger.Info($"{Name} update unsubscribe list data {_dateTime}");
                    var mailContent = new StringBuilder();
                    foreach (var job in _mJobs)
                    {
                        var page = 0;
                        while (true)
                        {
                            var result = await job.Func(_dateTime, page++, cancellationToken).ConfigureAwait(false);
                            var resultList = result.ToList();
                            if (resultList.Count == 0)
                            {
                                mailContent.Append(job.Type).Append(" got to page ").AppendLine(page.ToString());
                                break;
                            }
                            _zboxWorkerRoleService.UpdateUserFromUnsubscribe(
                                new Domain.Commands.UnsubscribeUsersFromEmailCommand(resultList,
                                    job.Type));
                            await RenewLeaseAsync(cancellationToken).ConfigureAwait(false);
                        }
                    }
                    var sw = new Stopwatch();
                    sw.Start();
                    await ProcessIntercomAsync(cancellationToken).ConfigureAwait(false);
                    sw.Stop();
                    mailContent.AppendLine($"process intercom finish and took {sw.ElapsedMilliseconds}");
                    await RenewLeaseAsync(cancellationToken).ConfigureAwait(false);
                    sw.Restart();
                    _zboxWorkerRoleService.UpdateUniversityStats(_dateTime);
                    sw.Stop();
                    mailContent.AppendLine($"update university took {sw.ElapsedMilliseconds}");
                    await RenewLeaseAsync(cancellationToken).ConfigureAwait(false);

                    _dateTime = DateTime.UtcNow.AddDays(-1);
                    await _mBlob.UploadTextAsync(_dateTime.ToFileTimeUtc().ToString(), Encoding.Default, new AccessCondition { LeaseId = _leaseId }, new BlobRequestOptions(), new OperationContext(), cancellationToken).ConfigureAwait(false);
                    await ReleaseLeaseAsync(cancellationToken).ConfigureAwait(false);
                    await Task.Delay(_sleepTime, cancellationToken).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    if (!string.IsNullOrEmpty(_leaseId))
                    {
                        // ReSharper disable once MethodSupportsCancellation task is cancelled so we not transfer that
                        await ReleaseLeaseAsync().ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    //await m_MailComponent.GenerateSystemEmailAsync("sendGrid api", $"{Name} with errors {ex}").ConfigureAwait(false);
                    _logger.Exception(ex);
                }
            }
        }

        private async Task ProcessIntercomAsync(CancellationToken token)
        {
            var page = 1;
            while (true)
            {
                var result = await _intercomManager.GetUnsubscribesAsync(page++, token).ConfigureAwait(false);
                if (result == null)
                {
                    break;
                }
                var list = result.ToList();
                if (list.Count == 0)
                {
                    break;
                }
                _zboxWorkerRoleService.UpdateUserFromUnsubscribe(
                               new Domain.Commands.UnsubscribeUsersFromEmailCommand(list.Select(s=>s.Email),
                                   EmailSend.Unsubscribe));
                await RenewLeaseAsync(token).ConfigureAwait(false);
            }
        }

        private Task RenewLeaseAsync(CancellationToken cancellationToken)
        {
            return _mBlob.RenewLeaseAsync(new AccessCondition { LeaseId = _leaseId }, cancellationToken);
        }

        private async Task<CloudBlockBlob> ReadBlobDataAsync(CancellationToken cancellationToken)
        {
            // ReSharper disable once StringLiteralTypo
            var blob = _cloudBlockProvider.GetFile("sendGridApiQuery", "zboxidgenerator");
            if (!await blob.ExistsAsync(cancellationToken).ConfigureAwait(false))
            {
                await blob.UploadTextAsync(_dateTime.ToFileTimeUtc().ToString(), cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var txt = await blob.DownloadTextAsync(cancellationToken).ConfigureAwait(false);
                if (long.TryParse(txt, out long num))
                {
                    _dateTime = DateTime.FromFileTimeUtc(num);
                }
            }
            return blob;
        }

        private async Task ReleaseLeaseAsync( CancellationToken cancellationToken = default(CancellationToken))
        {
            await _mBlob.ReleaseLeaseAsync(new AccessCondition
            {
                LeaseId = _leaseId
            }, cancellationToken).ConfigureAwait(false);
            _mBlob = null;
            _leaseId = string.Empty;
        }

        public class JobPerApi
        {
            public Func<DateTime, int, CancellationToken, Task<IEnumerable<string>>> Func { get; set; }
            public EmailSend Type { get; set; }
        }
    }
}
