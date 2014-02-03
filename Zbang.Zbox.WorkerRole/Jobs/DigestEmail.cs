using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Mail.Parameters;
using Zbang.Zbox.Infrastructure.Routes;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.Queries.Notification;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class DigestEmail : IJob
    {
        private readonly NotificationSettings m_DigestEmailHourBack;
        private bool m_KeepRunning;
        readonly IZboxReadService m_ZboxReadService;
        readonly IMailManager m_MailManager;
        readonly IShortCodesCache m_ShortToLongCode;

        public DigestEmail(NotificationSettings hourForEmailDigest, IZboxReadService zboxService, IMailManager mailManager, IShortCodesCache shortToLongCode)
        {
            m_DigestEmailHourBack = hourForEmailDigest;
            m_ZboxReadService = zboxService;
            m_MailManager = mailManager;
            m_ShortToLongCode = shortToLongCode;
        }

        public void Run()
        {
            m_KeepRunning = true;
            while (m_KeepRunning)
            {
                Execute();
            }
        }

        private void Execute()
        {
            var query = new GetUserDigestQuery(m_DigestEmailHourBack);
            var result = m_ZboxReadService.GetUserListForDigestEmail(query);
            foreach (var user in result)
            {
                try
                {
                    BuildUserDigestEmail(user);
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError(string.Format("Digest email for {0} user {1}", m_DigestEmailHourBack, user), ex);
                }
            }

            Thread.Sleep(TimeSpan.FromHours((int)m_DigestEmailHourBack));
        }

        private void BuildUserDigestEmail(UserDto user)
        {
            var userid = m_ShortToLongCode.ShortCodeToLong(user.Uid, ShortCodesType.User);
            var query = new GetBoxesDigestQuery(m_DigestEmailHourBack, userid);
            var result = m_ZboxReadService.GetBoxIdList(query);
            var boxesUpdates = new List<Updates>();
            foreach (var boxid in result)
            {
                var boxQuery = new GetBoxDataForDigestEmailQuery(m_DigestEmailHourBack, boxid.Id);
                var boxdata = m_ZboxReadService.GetBoxDataForDigestEmail(boxQuery);

                var update = new Updates(boxdata.Name, boxdata.Owner,
                                      new Uri(RoutesCollectionZbox.ShareBoxUri(
                                                m_ShortToLongCode.LongToShortCode(boxdata.Id))),
                                       boxdata.Comments.Select(s => new CommentDetails(s.AuthorName, DateTime.UtcNow - s.UpdateTime,
                                           s.CommentText, s.UserImage)).ToList(),
                                       boxdata.Files.Select(
                                           s => new FileDetails(s.UploaderName, DateTime.UtcNow - s.CreationTime,
                                                                s.UploaderImage, s.Name, s.ThumbnailBlobUrl)).ToList());
                boxesUpdates.Add(update);
            }
            var parameters = new Digest(boxesUpdates);

            m_MailManager.SendEmail(parameters, user.Email);

        }

        public void Stop()
        {
            m_KeepRunning = false;
        }
    }
}
