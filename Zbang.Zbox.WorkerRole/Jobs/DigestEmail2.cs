using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Zbang.Zbox.Infrastructure.Cache;
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

        private readonly ICache m_Cache;

        private bool m_KeepRunning;
        private readonly TimeSpan m_TimeToSleepAfterExcecuting;

        private readonly string m_CacheRegionName;

        //private Dictionary<long, IEnumerable<UpdateMailParams.BoxUpdateDetails>> m_Cache = new Dictionary<long, IEnumerable<UpdateMailParams.BoxUpdateDetails>>();

        public DigestEmail2(NotificationSettings hourForEmailDigest, IZboxReadServiceWorkerRole zboxService,
            IMailComponent mailComponent, ICache cache)
        {
            m_DigestEmailHourBack = hourForEmailDigest;
            m_ZboxReadService = zboxService;
            m_MailComponent = mailComponent;
            m_Cache = cache;
            m_CacheRegionName = "DigestEmails" + m_DigestEmailHourBack;
            if (m_DigestEmailHourBack == NotificationSettings.OnEveryChange)
            {
                m_TimeToSleepAfterExcecuting = TimeSpan.FromMinutes(BaseDigestLastUpdateQuery.OnEveryChangeTimeToQueryInMInutes);
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
                TraceLog.WriteError("On Run DigestEmail2", ex);
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
            var users = m_ZboxReadService.GetUsersByNotificationSettings(new GetUserByNotificationQuery(m_DigestEmailHourBack));
            foreach (var user in users)
            {

                try
                {
                    BuildUserReport(user.UserId, user.Email, user.Culture, user.UserName);
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError(string.Format("Digest email2 report:{0} user {1}", m_DigestEmailHourBack, user), ex);
                }
            }
            m_Cache.RemoveFromCache(m_CacheRegionName, null);
            // m_Cache.Clear();
            Thread.Sleep(m_TimeToSleepAfterExcecuting);
        }

        private void BuildUserReport(long userid, string email, string culture, string userName)
        {
            var updates = new List<UpdateMailParams.BoxUpdate>();
            var boxes = m_ZboxReadService.GetBoxesLastUpdates(new GetBoxesLastUpdateQuery(m_DigestEmailHourBack, userid));
            int numOfQuestion = 0, numOfAnswers = 0, numOfItems = 0;//, numOfUsers = 0;

            foreach (var box in boxes)
            {
                box.Url = BuildUrl(box);
                var boxupdates = GetBoxData(box);
                var userSpecificUpdates = boxupdates.Where(w => w.UserId != userid);
                var boxUpdateDetailses = userSpecificUpdates as UpdateMailParams.BoxUpdateDetails[] ?? userSpecificUpdates.ToArray();
                numOfQuestion += boxUpdateDetailses.OfType<UpdateMailParams.QuestionUpdate>().Count();
                numOfAnswers += boxUpdateDetailses.OfType<UpdateMailParams.AnswerUpdate>().Count();
                numOfItems += boxUpdateDetailses.OfType<UpdateMailParams.ItemUpdate>().Count();
                // numOfUsers += boxUpdateDetailses.OfType<UpdateMailParams.UserJoin>().Count();
                if (boxUpdateDetailses.Any())
                {
                    updates.Add(new UpdateMailParams.BoxUpdate(box.BoxName, boxUpdateDetailses.Take(4), box.Url, boxUpdateDetailses.Count() - 4));
                }
            }

            if (updates.Count == 0)
            {
                //empty report
                return;
            }

            m_MailComponent.GenerateAndSendEmail(email, new UpdateMailParams(updates,
                new CultureInfo(culture), userName,
                numOfQuestion,
                numOfAnswers,
                numOfItems));

        }

        private string BuildUrl(BoxDigestDto s)
        {
            return UrlConsts.BuildBoxUrl(s.BoxId, s.BoxName, s.UniversityName, true);
        }

        private IEnumerable<UpdateMailParams.BoxUpdateDetails> GetBoxData(BoxDigestDto box)
        {
            var cacheItem = m_Cache.GetFromCache(box.BoxId.ToString(CultureInfo.InvariantCulture), m_CacheRegionName) as IEnumerable<UpdateMailParams.BoxUpdateDetails>;
            if (cacheItem != null)
            {
                return cacheItem;
            }
            var items = m_ZboxReadService.GetItemsLastUpdates(new GetItemsLastUpdateQuery(m_DigestEmailHourBack, box.BoxId));


            var itemsUpdate = items.Select(s => new UpdateMailParams.ItemUpdate(s.Name,
                s.Picture
                , s.UserName,
                UrlConsts.BuildItemUrl(box.BoxId, box.BoxName, s.Id, s.Name, box.UniversityName ?? "my", true)
                , s.UserId));

            var quizes = m_ZboxReadService.GetQuizLastpdates(new GetItemsLastUpdateQuery(m_DigestEmailHourBack, box.BoxId));
            var quizUpdate = quizes.Select(s => new UpdateMailParams.ItemUpdate(s.Name,
                s.Picture
                , s.UserName,
                UrlConsts.BuildQuizUrl(box.BoxId, box.BoxName, s.Id, s.Name, box.UniversityName ?? "my", true)
                , s.UserId));


            var questions = m_ZboxReadService.GetQuestionsLastUpdates(new GetCommentsLastUpdateQuery(m_DigestEmailHourBack, box.BoxId));
            var questionUpdate = questions.Select(s => new UpdateMailParams.QuestionUpdate(s.UserName, s.Text, box.BoxPicture, box.Url, s.UserId));

            var answers = m_ZboxReadService.GetAnswersLastUpdates(new GetCommentsLastUpdateQuery(m_DigestEmailHourBack, box.BoxId));
            var answersUpdate = answers.Select(s => new UpdateMailParams.AnswerUpdate(s.UserName, s.Text, box.BoxPicture, box.Url, s.UserId));

            var disucssion = m_ZboxReadService.GetQuizDiscussion(new GetCommentsLastUpdateQuery(m_DigestEmailHourBack, box.BoxId));
            var discussionUpdate = disucssion.Select(s => new UpdateMailParams.DiscussionUpdate(s.UserName, s.Text, box.BoxPicture,
                 UrlConsts.BuildQuizUrl(box.BoxId, box.BoxName, s.QuizId, s.QuizName, box.UniversityName ?? "my", true),
                s.UserId));

            //var memebers = m_ZboxReadService.GetNewMembersLastUpdates(new GetMembersLastUpdateQuery(m_DigestEmailHourBack, box.BoxId));
            //var membersUpdate = memebers.Select(s =>
            //    new UpdateMailParams.UserJoin(s.Name, s.Picture, box.BoxName,
            //        UrlConsts.BuildUserUrl(s.Id, s.Name, true), s.Id));

            var boxupdates = new List<UpdateMailParams.BoxUpdateDetails>();
            boxupdates.AddRange(itemsUpdate);
            boxupdates.AddRange(questionUpdate);
            //boxupdates.AddRange(membersUpdate);
            boxupdates.AddRange(answersUpdate);
            boxupdates.AddRange(quizUpdate);
            boxupdates.AddRange(discussionUpdate);
            // m_Cache.Add(box.BoxId, boxupdates);
            m_Cache.AddToCache(box.BoxId.ToString(CultureInfo.InvariantCulture), boxupdates, TimeSpan.FromDays(1), m_CacheRegionName);

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
