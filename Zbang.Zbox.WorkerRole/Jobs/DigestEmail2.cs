using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.Emails;
using Zbang.Zbox.ViewModel.Queries.Emails;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class DigestEmail2 : IJob
    {
        private readonly NotificationSettings m_DigestEmailHourBack;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IMailComponent m_MailComponent;


        private bool m_KeepRunning;
        private readonly TimeSpan m_TimeToSleepAfterExecuting;



        public DigestEmail2(NotificationSettings hourForEmailDigest, IZboxReadServiceWorkerRole zboxService,
            IMailComponent mailComponent)
        {
            m_DigestEmailHourBack = hourForEmailDigest;
            m_ZboxReadService = zboxService;
            m_MailComponent = mailComponent;
            m_TimeToSleepAfterExecuting = m_DigestEmailHourBack == NotificationSettings.OnEveryChange ? TimeSpan.FromMinutes(BaseDigestLastUpdateQuery.OnEveryChangeTimeToQueryInMInutes) : TimeSpan.FromHours(1);

        }
        public void Run()
        {
            try
            {
                m_KeepRunning = true;
                while (m_KeepRunning)
                {
                    ExecuteAsync().Wait();
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On Run DigestEmail2", ex);
                throw;
            }

        }

        private async Task ExecuteAsync()
        {
            if (!ShouldRunReport())
            {
                Thread.Sleep(TimeSpan.FromMinutes(5));
                return;
            }
            var users = await m_ZboxReadService.GetUsersByNotificationSettings(new GetUserByNotificationQuery(m_DigestEmailHourBack));
            foreach (var user in users)
            {

                try
                {
                    await BuildUserReport(user.UserId, user.Email, user.Culture, user.UserName);
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError(string.Format("Digest email2 report:{0} user {1}", m_DigestEmailHourBack, user), ex);
                }
            }
           // m_Cache.RemoveFromCache(m_CacheRegionName);
            // m_Cache.Clear();
            Thread.Sleep(m_TimeToSleepAfterExecuting);
        }

        private async Task BuildUserReport(long userid, string email, string culture, string userName)
        {
            var updates = new List<UpdateMailParams.BoxUpdate>();
            var boxes = await m_ZboxReadService.GetBoxesLastUpdates(new GetBoxesLastUpdateQuery(m_DigestEmailHourBack, userid));

            int numOfQuestion = 0, numOfAnswers = 0, numOfItems = 0;

            foreach (var box in boxes.Select(s =>
            {
                s.Url = UrlConsts.AppendCloudentsUrl(s.Url);
                return s;
            }))
            {
                var boxupdates = await GetBoxData(box);
                var userSpecificUpdates = boxupdates.Where(w => w.UserId != userid).ToList();
                if (!userSpecificUpdates.Any()) continue;
                numOfQuestion += userSpecificUpdates.OfType<UpdateMailParams.QuestionUpdate>().Count();
                numOfAnswers += userSpecificUpdates.OfType<UpdateMailParams.AnswerUpdate>().Count();
                numOfItems += userSpecificUpdates.OfType<UpdateMailParams.ItemUpdate>().Count();

                updates.Add(new UpdateMailParams.BoxUpdate(box.BoxName,
                    userSpecificUpdates.Take(4),
                    box.Url,
                    userSpecificUpdates.Count() - 4));
            }

            if (updates.Count == 0)
            {
                return;
            }

            m_MailComponent.GenerateAndSendEmail(email, new UpdateMailParams(updates,
                new CultureInfo(culture), userName,
                numOfQuestion,
                numOfAnswers,
                numOfItems));

        }



        private async Task<IEnumerable<UpdateMailParams.BoxUpdateDetails>> GetBoxData(BoxDigestDto box)
        {
            var boxUpdates =
                await m_ZboxReadService.GetBoxLastUpdates(new GetBoxLastUpdateQuery(m_DigestEmailHourBack, box.BoxId));


            var itemsUpdate = boxUpdates.Items.Select(s => new UpdateMailParams.ItemUpdate(s.Name,
                s.Picture
                , s.UserName,
                UrlConsts.AppendCloudentsUrl(s.Url)
                , s.UserId));

            var quizUpdate = boxUpdates.Quizzes.Select(s => new UpdateMailParams.ItemUpdate(s.Name,
                s.Picture
                , s.UserName,
                UrlConsts.AppendCloudentsUrl(s.Url)
                , s.UserId));


            var questionUpdate = boxUpdates.BoxComments.Select(s => new UpdateMailParams.QuestionUpdate(
                s.UserName, s.Text, box.BoxPicture, box.Url, s.UserId));

            var answersUpdate = boxUpdates.BoxReplies.Select(s => new UpdateMailParams.AnswerUpdate(
                s.UserName, s.Text, box.BoxPicture, box.Url, s.UserId));

            var discussionUpdate = boxUpdates.QuizDiscussions.Select(s => new UpdateMailParams.DiscussionUpdate(
                s.UserName, s.Text, box.BoxPicture,
                  UrlConsts.AppendCloudentsUrl(s.Url),
                s.UserId));



            var boxupdates = new List<UpdateMailParams.BoxUpdateDetails>();
            boxupdates.AddRange(itemsUpdate);
            boxupdates.AddRange(questionUpdate);
            boxupdates.AddRange(answersUpdate);
            boxupdates.AddRange(quizUpdate);
            boxupdates.AddRange(discussionUpdate);
            //m_Cache.AddToCache(box.BoxId.ToString(CultureInfo.InvariantCulture), boxupdates,
            //    TimeSpan.FromHours(1), m_CacheRegionName);

            return boxupdates;

        }

        private bool ShouldRunReport()
        {
            if (m_DigestEmailHourBack == NotificationSettings.OnceADay)
            {
                if (DateTime.UtcNow.Hour == 0)
                {
                    return true;
                }
            }
            if (m_DigestEmailHourBack == NotificationSettings.OnceAWeek)
            {
                if (DateTime.UtcNow.Hour == 5 && DateTime.UtcNow.DayOfWeek == DayOfWeek.Sunday)
                {
                    return true;
                }
            }
            if (m_DigestEmailHourBack == NotificationSettings.OnEveryChange)
            {
                return true;
            }
            return false;
        }
        public void Stop()
        {
            m_KeepRunning = false;
        }
    }
}
