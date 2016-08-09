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
using Zbang.Zbox.ViewModel.Queries.Emails;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    public class DigestEmail : IMailProcess
    {
        private readonly IMailComponent m_MailComponent;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private const string ServiceName = "Digest email";
        private readonly NotificationSettings m_DigestEmailHourBack;

        private readonly HashSet<string> m_EmailHash = new HashSet<string>();

        public DigestEmail(IMailComponent mailComponent, IZboxReadServiceWorkerRole zboxReadService,
            NotificationSettings hourForEmailDigest)
        {
            m_MailComponent = mailComponent;
            m_ZboxReadService = zboxReadService;
            m_DigestEmailHourBack = hourForEmailDigest;
        }

        private string GetServiceName()
        {
            return $"{ServiceName} {m_DigestEmailHourBack}";
        }

        private async Task SendEmailStatusAsync(string message)
        {
            if (NotificationSettings.OnEveryChange == m_DigestEmailHourBack)
            {
                return;
            }
            await m_MailComponent.GenerateSystemEmailAsync(GetServiceName(), message);

        }

        public async Task<bool> ExecuteAsync(int index, Func<int, Task> progressAsync, CancellationToken token)
        {
            var page = index;
            var needToContinueRun = true;
            var list = new List<Task>();
            while (needToContinueRun)
            {
                try
                {
                    TraceLog.WriteInfo($"{GetServiceName()} running  mail page {page}");
                    needToContinueRun = false;
                    var pageSize = 500;
                    if (RoleIndexProcessor.IsEmulated)
                    {
                        pageSize = 10;
                    }
                    var users =
                        await
                            m_ZboxReadService.GetUsersByNotificationSettingsAsync(
                                new GetUserByNotificationQuery(m_DigestEmailHourBack, page, pageSize), token);
                    foreach (var user in users)
                    {
                        try
                        {
                            if (!m_EmailHash.Add(user.Email))
                            {
                                TraceLog.WriteError($"{user.Email} is already sent");
                                await SendEmailStatusAsync($"{user.Email} error digest email already sent");
                                continue;
                            }
                            needToContinueRun = true;
                            var email = user.Email;
                            if (RoleIndexProcessor.IsEmulated)
                            {
                                email = "ram@cloudents.com";
                            }
                            var culture = string.IsNullOrEmpty(user.Culture)
                                ? new CultureInfo("en-US")
                                : new CultureInfo(user.Culture);
                            var updates =
                                await
                                    m_ZboxReadService.GetUserUpdatesAsync(
                                        new GetBoxesLastUpdateQuery(m_DigestEmailHourBack, user.UserId), token);

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

                            var updatesData = await m_ZboxReadService.GetUpdatesAsync(query, token);


                            var updatesEmail = new List<UpdateMailParams.BoxUpdate>();
                            foreach (var box in updatesData.Boxes)
                            {
                                if (box.Url == null)
                                {
                                    TraceLog.WriteError($"{box.BoxId} url is null");
                                    continue;
                                }
                                box.Url = UrlConst.AppendCloudentsUrl(box.Url);
                                var itemsUpdates =
                                    updatesData.Items.Where(w => w.BoxId == box.BoxId)
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
                                    updatesData.Quizzes.Where(w => w.BoxId == box.BoxId)
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
                                var boxupdates = new List<UpdateMailParams.BoxUpdateDetails>();
                                boxupdates.AddRange(itemsUpdates);
                                boxupdates.AddRange(questionUpdate);
                                boxupdates.AddRange(answersUpdate);
                                boxupdates.AddRange(quizUpdate);
                                boxupdates.AddRange(discussionUpdate);

                                updatesEmail.Add(new UpdateMailParams.BoxUpdate(box.BoxName, boxupdates.Take(4),
                                    UrlConst.AppendCloudentsUrl(box.Url),
                                    updatesList.Count(g => g.BoxId == box.BoxId) - 4));
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
                                    updatesList.Count), token));
                        }
                        catch (Exception ex)
                        {
                            await SendEmailStatusAsync($"error digest email {ex}");
                            TraceLog.WriteError($"{GetServiceName()} error digest email {ex}");

                        }
                    }
                    await Task.WhenAll(list);
                    list.Clear();
                    page++;
                    if (RoleIndexProcessor.IsEmulated)
                    {
                        needToContinueRun = false;
                    }
                    await progressAsync(page);
                }
                catch (Exception ex)
                {
                    await SendEmailStatusAsync($"error digest email {ex}");
                    TraceLog.WriteError($"{GetServiceName()} error digest email {ex}");
                    return false;
                }
            }
            await SendEmailStatusAsync($"finish to run  with page {page}");
            TraceLog.WriteInfo($"{GetServiceName()} finish running  mail page {page}");
            return true;
        }
    }
}
