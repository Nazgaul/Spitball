using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.DTOs.Emails;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class DigestEmail2 : IJob
    {
        private readonly NotificationSettings m_DigestEmailHourBack;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IMailComponent m_MailComponent;

        private readonly ICache m_Cache;

        private bool m_KeepRunning;
        private readonly TimeSpan m_TimeToSleepAfterExcecuting;

        private readonly string CacheRegionName;

        //private Dictionary<long, IEnumerable<UpdateMailParams.BoxUpdateDetails>> m_Cache = new Dictionary<long, IEnumerable<UpdateMailParams.BoxUpdateDetails>>();

        public DigestEmail2(NotificationSettings hourForEmailDigest, IZboxReadServiceWorkerRole zboxService,
            IMailComponent mailComponent, ICache cache)
        {
            m_DigestEmailHourBack = hourForEmailDigest;
            m_ZboxReadService = zboxService;
            m_MailComponent = mailComponent;
            m_Cache = cache;
            CacheRegionName = "DigestEmails" + m_DigestEmailHourBack;
            if (m_DigestEmailHourBack == NotificationSettings.OnEveryChange)
            {
                m_TimeToSleepAfterExcecuting = TimeSpan.FromMinutes(Zbang.Zbox.ViewModel.Queries.Emails.BaseDigestLastUpdateQuery.OnEveryChangeTimeToQueryInMInutes);
            }
            else
            {
                m_TimeToSleepAfterExcecuting = TimeSpan.FromHours(1);
            }

        }
        public void Run()
        {
            try
            {
                m_KeepRunning = true;
                while (m_KeepRunning)
                {
                    Execute();
                }
            }
            catch (Exception ex)
            {
                Zbang.Zbox.Infrastructure.Trace.TraceLog.WriteError("On Run DigestEmail2", ex);
                throw;
            }

        }

        private void Execute()
        {
            if (!ShouldRunReport())
            {
                Thread.Sleep(TimeSpan.FromMinutes(5));
                return;
            }
            var users = m_ZboxReadService.GetUsersByNotificationSettings(new ViewModel.Queries.Emails.GetUserByNotificationQuery(m_DigestEmailHourBack));
            foreach (var user in users)
            {

                try
                {
                    buildUserReport(user.UserId, user.Email, user.Culture, user.UserName);
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError(string.Format("Digest email2 report:{0} user {1}", m_DigestEmailHourBack, user), ex);
                }
            }
            m_Cache.RemoveFromCache(CacheRegionName, null);
            // m_Cache.Clear();
            Thread.Sleep(m_TimeToSleepAfterExcecuting);
        }

        private void buildUserReport(long userid, string email, string culture, string userName)
        {
            var updates = new List<UpdateMailParams.BoxUpdate>();
            var boxes = m_ZboxReadService.GetBoxesLastUpdates(new ViewModel.Queries.Emails.GetBoxesLastUpdateQuery(m_DigestEmailHourBack, userid));
            int numOfQuestion = 0, numOfAnswers = 0, numOfItems = 0, numOfUsers = 0;
            boxes = boxes.Select(s =>
            {
                if (string.IsNullOrEmpty(s.UniversityName))
                {
                    s.Url = string.Format(UrlConsts.BoxUrl, s.BoxId, UrlConsts.NameToQueryString(s.BoxName));
                }
                else
                {
                    s.Url = string.Format(UrlConsts.CourseUrl, s.BoxId, UrlConsts.NameToQueryString(s.BoxName), UrlConsts.NameToQueryString(s.UniversityName));
                }

                return s;
            });

            foreach (var box in boxes)
            {
                var boxupdates = GetBoxData(box);
                var userSpecificUpdates = boxupdates.Where(w => w.UserId != userid);
                numOfQuestion += userSpecificUpdates.OfType<UpdateMailParams.QuestionUpdate>().Count();
                numOfAnswers += userSpecificUpdates.OfType<UpdateMailParams.AnswerUpdate>().Count();
                numOfItems += userSpecificUpdates.OfType<UpdateMailParams.ItemUpdate>().Count();
                numOfUsers += userSpecificUpdates.OfType<UpdateMailParams.UserJoin>().Count();
                if (userSpecificUpdates.Count() > 0)
                {
                    updates.Add(new UpdateMailParams.BoxUpdate(box.BoxName, userSpecificUpdates.Take(4), box.Url, userSpecificUpdates.Count() - 4));
                }
            }

            if (updates.Count == 0)
            {
                //empty report
                return;
            }

            m_MailComponent.GenerateAndSendEmail(email, new UpdateMailParams(updates,
                new System.Globalization.CultureInfo(culture), userName,
                numOfQuestion,
                numOfAnswers,
                numOfItems, numOfUsers));

        }
        private IEnumerable<UpdateMailParams.BoxUpdateDetails> GetBoxData(BoxDigestDto box)
        {
            var cacheItem = m_Cache.GetFromCache(box.BoxId.ToString(), CacheRegionName) as IEnumerable<UpdateMailParams.BoxUpdateDetails>;
            if (cacheItem != null)
            {
                return cacheItem;
            }
            //if (m_Cache.ContainsKey(box.BoxId))
            //{
            //    return m_Cache[box.BoxId];
            //}
            var items = m_ZboxReadService.GetItemsLastUpdates(new ViewModel.Queries.Emails.GetItemsLastUpdateQuery(m_DigestEmailHourBack, box.BoxId));


            var itemsUpdate = items.Select(s => new UpdateMailParams.ItemUpdate(s.Name,
                s.Picture
                , s.UserName,
               string.Format(UrlConsts.ItemUrl,
               UrlConsts.NameToQueryString(box.UniversityName ?? "my"), box.BoxId,
               UrlConsts.NameToQueryString(box.BoxName), s.Id,
               UrlConsts.NameToQueryString(s.Name))
                , s.UserId));

            var quizes = m_ZboxReadService.GetQuizLastpdates(new ViewModel.Queries.Emails.GetItemsLastUpdateQuery(m_DigestEmailHourBack, box.BoxId));
            var quizUpdate = quizes.Select(s => new UpdateMailParams.ItemUpdate(s.Name,
                s.Picture
                , s.UserName,
               string.Format(UrlConsts.QuizUrl,
               UrlConsts.NameToQueryString(box.UniversityName ?? "my"), box.BoxId,
               UrlConsts.NameToQueryString(box.BoxName), s.Id,
               UrlConsts.NameToQueryString(s.Name))
                , s.UserId));


            var questions = m_ZboxReadService.GetQuestionsLastUpdates(new ViewModel.Queries.Emails.GetCommentsLastUpdateQuery(m_DigestEmailHourBack, box.BoxId));
            var questionUpdate = questions.Select(s => new UpdateMailParams.QuestionUpdate(s.UserName, s.Text, box.BoxPicture, box.Url, s.UserId));

            var answers = m_ZboxReadService.GetAnswersLastUpdates(new ViewModel.Queries.Emails.GetCommentsLastUpdateQuery(m_DigestEmailHourBack, box.BoxId));
            var answersUpdate = answers.Select(s => new UpdateMailParams.AnswerUpdate(s.UserName, s.Text, box.BoxPicture, box.Url, s.UserId));

            var disucssion = m_ZboxReadService.GetQuizDiscussion(new ViewModel.Queries.Emails.GetCommentsLastUpdateQuery(m_DigestEmailHourBack, box.BoxId));
            var discussionUpdate = disucssion.Select(s => new UpdateMailParams.DiscussionUpdate(s.UserName, s.Text, box.BoxPicture,
                string.Format(UrlConsts.QuizUrl,
               UrlConsts.NameToQueryString(box.UniversityName ?? "my"), box.BoxId,
               UrlConsts.NameToQueryString(box.BoxName), s.QuizId,
               UrlConsts.NameToQueryString(s.QuizName)), s.UserId));

            var memebers = m_ZboxReadService.GetNewMembersLastUpdates(new ViewModel.Queries.Emails.GetMembersLastUpdateQuery(m_DigestEmailHourBack, box.BoxId));
            var membersUpdate = memebers.Select(s =>
                new UpdateMailParams.UserJoin(s.Name, s.Picture, box.BoxName,
                    string.Format(UrlConsts.UserUrl,
                    s.Id,
                    UrlConsts.NameToQueryString(s.Name))
                    , s.Id));

            var boxupdates = new List<UpdateMailParams.BoxUpdateDetails>();
            boxupdates.AddRange(itemsUpdate);
            boxupdates.AddRange(questionUpdate);
            boxupdates.AddRange(membersUpdate);
            boxupdates.AddRange(answersUpdate);
            boxupdates.AddRange(quizUpdate);
            boxupdates.AddRange(discussionUpdate);
            // m_Cache.Add(box.BoxId, boxupdates);
            m_Cache.AddToCache(box.BoxId.ToString(), boxupdates, TimeSpan.FromDays(1), CacheRegionName);

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
