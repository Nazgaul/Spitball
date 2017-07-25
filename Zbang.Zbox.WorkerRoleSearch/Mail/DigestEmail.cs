using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.Emails;
using Zbang.Zbox.ViewModel.Queries.Emails;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    public class DigestEmail : ISchedulerProcess
    {
        private readonly IMailComponent m_MailComponent;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private const string ServiceName = "Digest email";
        private readonly NotificationSetting m_DigestEmailHourBack;
        private readonly int m_UtcTimeOffset;
        private readonly ILogger m_Logger;

        private readonly HashSet<string> m_EmailHash = new HashSet<string>();

        public DigestEmail(IMailComponent mailComponent, IZboxReadServiceWorkerRole zboxReadService,
            NotificationSetting hourForEmailDigest, int utcTimeOffset, ILogger logger)
        {
            m_MailComponent = mailComponent;
            m_ZboxReadService = zboxReadService;
            m_DigestEmailHourBack = hourForEmailDigest;
            m_UtcTimeOffset = utcTimeOffset;
            m_Logger = logger;
        }

        private string GetServiceName()
        {
            return $"{ServiceName} {m_DigestEmailHourBack}";
        }

        private Task SendEmailStatusAsync(string message)
        {
            return m_MailComponent.GenerateSystemEmailAsync(GetServiceName(), message);
        }

        public async Task<bool> ExecuteAsync(int index, Func<int, TimeSpan, Task> progressAsync, CancellationToken token)
        {
            if (progressAsync == null) throw new ArgumentNullException(nameof(progressAsync));
            var page = index;
            var needToContinueRun = true;
            var list = new List<Task>();
            while (needToContinueRun)
            {
                try
                {
                    needToContinueRun = false;
                    var pageSize = 100;
                    if (RoleIndexProcessor.IsEmulated)
                    {
                        pageSize = 10;
                    }
                    var usersQuery = new GetUserByNotificationQuery(m_DigestEmailHourBack, page, pageSize, m_UtcTimeOffset);
                    var users =
                        (await
                            m_ZboxReadService.GetUsersByNotificationSettingsAsync(
                                usersQuery, token).ConfigureAwait(false)).ToList();
                    m_Logger.Info(
                        $"{GetServiceName()} query: {usersQuery} going to send emails to {string.Join("\n", users)}");
                    foreach (var user in users)
                    {
                        try
                        {
                            needToContinueRun = true;
                            await ProcessUserEmailAsync(token, user, list).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            await SendEmailStatusAsync($"error digest email {ex}").ConfigureAwait(false);
                            m_Logger.Exception(ex,new Dictionary<string,string> {{ "service", GetServiceName() } });

                        }
                    }
                    await Task.WhenAll(list).ConfigureAwait(false);
                    list.Clear();
                    page++;
                    if (RoleIndexProcessor.IsEmulated)
                    {
                        needToContinueRun = false;
                    }
                    await progressAsync(page, TimeSpan.FromMinutes(15)).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    page++;
                    await progressAsync(page, TimeSpan.FromMinutes(5)).ConfigureAwait(false);
                    await SendEmailStatusAsync($"error digest email {ex}").ConfigureAwait(false);
                    m_Logger.Exception(ex, new Dictionary<string, string> { { "service", GetServiceName() } });

                    //return false;
                }
            }
            await SendEmailStatusAsync($"finish to run  with page {page} utc {m_UtcTimeOffset} total: {m_EmailHash.Count}").ConfigureAwait(false);
            m_Logger.Info($"{GetServiceName()} finish running  mail page {page}");
            return true;
        }

        private async Task ProcessUserEmailAsync(CancellationToken token, UserDigestDto user, List<Task> list)
        {
            var email = user.Email;
            if (RoleIndexProcessor.IsEmulated)
            {
                email = "ram@cloudents.com";
            }
            if (!m_EmailHash.Add(email))
            {
                m_Logger.Warning($"{email} is already sent");
                return;
            }
            var culture = string.IsNullOrEmpty(user.Culture)
                ? new CultureInfo("en-US")
                : new CultureInfo(user.Culture);
            var updates =
                await
                    m_ZboxReadService.GetUserUpdatesAsync(
                        new GetBoxesLastUpdateQuery(m_DigestEmailHourBack, user.UserId), token).ConfigureAwait(false);

            var updatesList = updates.ToList();
            var query = new GetUpdatesQuery(
                updatesList.GroupBy(g => g.BoxId).Select(s => s.Key),
                updatesList.GroupBy(g => g.ItemId).Where(s => s.Key != null).Select(s => s.Key),
                updatesList.GroupBy(g => g.QuizId).Where(s => s.Key != null).Select(s => s.Key),
                updatesList.Where(
                        w =>
                            !w.ItemId.HasValue && !w.QuizId.HasValue && !w.AnswerId.HasValue &&
                            !w.QuizDiscussionId.HasValue)
                    .GroupBy(g => g.QuestionId).Where(s => s.Key != null).Select(s => s.Key),
                updatesList.GroupBy(g => g.AnswerId).Where(s => s.Key != null).Select(s => s.Key),
                updatesList.GroupBy(g => g.QuizDiscussionId).Where(s => s.Key != null).Select(s => s.Key));

            var updatesData = await m_ZboxReadService.GetUpdatesAsync(query, token).ConfigureAwait(false);


            var updatesEmail = new List<UpdateMailParams.BoxUpdate>();
            foreach (var box in updatesData.Boxes)
            {
                if (box.Url == null)
                {
                    m_Logger.Error($"{box.BoxId} url is null");
                    continue;
                }
                ProcessBoxUpdates(box, updatesData, updatesEmail, updatesList);
            }

            list.Add(m_MailComponent.GenerateAndSendEmailAsync(
                email, new UpdateMailParams(updatesEmail,
                    culture, user.UserName,
                    updatesList.Count(
                        c =>
                            c.QuestionId.HasValue && !c.ItemId.HasValue && !c.QuizId.HasValue &&
                            !c.AnswerId.HasValue && !c.QuizDiscussionId.HasValue),
                    updatesList.Count(c => c.AnswerId.HasValue),
                    updatesList.Count(c => c.ItemId.HasValue),
                    updatesList.Count), token, $"update {m_DigestEmailHourBack.GetStringValue()} "));
        }

        private static void ProcessBoxUpdates(BoxDigestDto box, BoxUpdatesDigestDto updatesData, List<UpdateMailParams.BoxUpdate> updatesEmail,
            List<UserUpdatesDigestDto> updatesList)
        {
            box.Url = UrlConst.AppendCloudentsUrl(box.Url);
            var itemsUpdates =
                updatesData.Items.Where(w => w.BoxId == box.BoxId && !string.IsNullOrEmpty(w.Url))
                    .Take(4)
                    .Select(s => new UpdateMailParams.ItemUpdate(s.Name,
                        s.Picture =
                            "https://az779114.vo.msecnd.net/preview/" +
                            WebUtility.UrlEncode(s.Picture) +
                            ".jpg?width=64&height=90&mode=crop"
                        , s.UserName,
                        UrlConst.AppendCloudentsUrl(s.Url)
                        , s.UserId));

            var quizUpdate =
                updatesData.Quizzes.Where(w => w.BoxId == box.BoxId && !string.IsNullOrEmpty(w.Url))
                    .Take(4)
                    .Select(s => new UpdateMailParams.ItemUpdate(s.Name,
                        s.Picture
                        , s.UserName,
                        UrlConst.AppendCloudentsUrl(s.Url)
                        , s.UserId));

            const string somePicture =
                "http://az32006.vo.msecnd.net/mailcontainer/user-email-default.jpg";
            var questionUpdate =
                updatesData.Comments.Where(w => w.BoxId == box.BoxId).Take(4).Select(s =>
                {
                    if (string.IsNullOrEmpty(s.UserImage))
                    {
                        s.UserImage = somePicture;
                    }
                    return new UpdateMailParams.QuestionUpdate(
                        s.UserName, s.Text, s.UserImage, box.Url, s.UserId);
                });

            var answersUpdate =
                updatesData.Replies.Where(w => w.BoxId == box.BoxId).Take(4).Select(s =>
                {
                    if (string.IsNullOrEmpty(s.UserImage))
                    {
                        s.UserImage = somePicture;
                    }
                    return new UpdateMailParams.AnswerUpdate(
                        s.UserName, s.Text, s.UserImage, box.Url,
                        s.UserId);
                });

            var discussionUpdate =
                updatesData.QuizDiscussions.Where(w => w.BoxId == box.BoxId).Take(4).Select(s =>
                {
                    if (string.IsNullOrEmpty(s.UserImage))
                    {
                        s.UserImage = somePicture;
                    }
                    return new UpdateMailParams.DiscussionUpdate(
                        s.UserName, s.Text, s.UserImage,
                        UrlConst.AppendCloudentsUrl(s.Url),
                        s.UserId);
                });
            var boxUpdates = new List<UpdateMailParams.BoxUpdateDetails>();
            boxUpdates.AddRange(itemsUpdates);
            boxUpdates.AddRange(questionUpdate);
            boxUpdates.AddRange(answersUpdate);
            boxUpdates.AddRange(quizUpdate);
            boxUpdates.AddRange(discussionUpdate);

            updatesEmail.Add(new UpdateMailParams.BoxUpdate(box.BoxName, boxUpdates.Take(4),
                UrlConst.AppendCloudentsUrl(box.Url),
                updatesList.Count(g => g.BoxId == box.BoxId) - 4));
        }
    }
}
