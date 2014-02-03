using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly IShortCodesCache m_ShortCodesCache;

        private bool m_KeepRunning;
        private readonly TimeSpan m_TimeToSleepAfterExcecuting;

        private Dictionary<long, IEnumerable<UpdateMailParams.BoxUpdateDetails>> m_Cache = new Dictionary<long, IEnumerable<UpdateMailParams.BoxUpdateDetails>>();

        public DigestEmail2(NotificationSettings hourForEmailDigest, IZboxReadServiceWorkerRole zboxService,
            IMailComponent mailComponent, IShortCodesCache shortCodesCache)
        {
            m_DigestEmailHourBack = hourForEmailDigest;
            m_ZboxReadService = zboxService;
            m_MailComponent = mailComponent;
            m_ShortCodesCache = shortCodesCache;
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
                    buildUserReport(user.UserId, user.Email, user.Culture);
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError(string.Format("Digest email2 report:{0} user {1}", m_DigestEmailHourBack, user), ex);
                }
            }
            m_Cache.Clear();
            Thread.Sleep(m_TimeToSleepAfterExcecuting);
        }

        private void buildUserReport(long userid, string email, string culture)
        {
            var updates = new List<UpdateMailParams.BoxUpdate>();
            var boxes = m_ZboxReadService.GetBoxesLastUpdates(new ViewModel.Queries.Emails.GetBoxesLastUpdateQuery(m_DigestEmailHourBack, userid));
            boxes = boxes.Select(s =>
            {
                s.BoxUid = m_ShortCodesCache.LongToShortCode(s.BoxId);
                return s;
            });

            foreach (var box in boxes)
            {
                var boxupdates = GetBoxData(box);
                var userSpecificUpdates = boxupdates.Where(w => w.UserId != userid);

                if (userSpecificUpdates.Count() > 0)
                {
                    updates.Add(new UpdateMailParams.BoxUpdate(box.BoxName, userSpecificUpdates));
                }
            }

            if (updates.Count == 0)
            {
                //empty report
                return;
            }

            m_MailComponent.GenerateAndSendEmail(email, new UpdateMailParams(updates, new System.Globalization.CultureInfo(culture)));
        }
        private IEnumerable<UpdateMailParams.BoxUpdateDetails> GetBoxData(BoxDigestDto box)
        {
            if (m_Cache.ContainsKey(box.BoxId))
            {
                return m_Cache[box.BoxId];
            }
            var items = m_ZboxReadService.GetItemsLastUpdates(new ViewModel.Queries.Emails.GetItemsLastUpdateQuery(m_DigestEmailHourBack, box.BoxId));

            var itemsUpdate = items.Select(s => new UpdateMailParams.BoxUpdateDetails(s.UserId, s.UserName,
                s.Name, EmailAction.AddedItem,
                  string.Format(UrlConsts.ItemUrl, box.BoxUid, s.Uid)));


            var comments = m_ZboxReadService.GetQuestionsLastUpdates(new ViewModel.Queries.Emails.GetCommentsLastUpdateQuery(m_DigestEmailHourBack, box.BoxId));
            var commentsUpdate = comments.Select(s => new UpdateMailParams.BoxUpdateDetails(s.UserId, s.UserName,
               string.Empty,
                EmailAction.AskedQuestion,
                string.Format(UrlConsts.BoxUrl, box.BoxUid)
             ));

            var answers = m_ZboxReadService.GetAnswersLastUpdates(new ViewModel.Queries.Emails.GetCommentsLastUpdateQuery(m_DigestEmailHourBack, box.BoxId));
            var answersUpdate = answers.Select(s => new UpdateMailParams.BoxUpdateDetails(s.UserId, s.UserName,
               string.Empty,
                EmailAction.Answered,
                string.Format(UrlConsts.BoxUrl, box.BoxUid)
             ));


            var memebers = m_ZboxReadService.GetNewMembersLastUpdates(new ViewModel.Queries.Emails.GetMembersLastUpdateQuery(m_DigestEmailHourBack, box.BoxId));
            var membersUpdate = memebers.Select(s =>
                new UpdateMailParams.BoxUpdateDetails(s.UserId, s.UserName, box.BoxName, EmailAction.Join, string.Format(UrlConsts.BoxUrl, box.BoxUid)));

            var boxupdates = itemsUpdate.Union(commentsUpdate).Union(membersUpdate).Union(answersUpdate);
            m_Cache.Add(box.BoxId, boxupdates);
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
