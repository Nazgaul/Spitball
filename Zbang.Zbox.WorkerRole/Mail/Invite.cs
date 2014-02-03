using System;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Mail.Parameters;
using Zbang.Zbox.Infrastructure.Routes;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries.Notification;

namespace Zbang.Zbox.WorkerRole.Mail
{
    internal class Invite : IMail
    {
        readonly IZboxReadService m_ZboxReadService;
        readonly IMailManager m_MailManager;
        readonly IShortCodesCache m_ShortToLongCode;

        public Invite(IZboxReadService zboxService, IMailManager mailManager, IShortCodesCache shortToLongCode)
        {
            m_ZboxReadService = zboxService;
            m_MailManager = mailManager;
            m_ShortToLongCode = shortToLongCode;
        }
        public bool Excecute(MailQueueData data)
        {
            var inviteData = data as InviteMail;
            if (inviteData == null)
            {
                throw new ArgumentException("Invite mail did not receive proper stream");
            }
            var query = new GetBoxInviteDataQuery(inviteData.BoxId);
            var boxData = m_ZboxReadService.GetBoxDateForInvite(query);

            var htmlPersonalNote = TextManipulation.EncodeComment(inviteData.PersonalNote).Replace("\n", "<br>");

            var parametes = new InviteToBox(inviteData.SenderUserName, htmlPersonalNote,
                boxData.Name, new Uri(RoutesCollectionZbox.ShareBoxUri(m_ShortToLongCode.LongToShortCode(inviteData.BoxId))),
                boxData.UpdateTime,boxData.NumOfItems,boxData.Image);                

            foreach (var email in inviteData.To)
            {
                m_MailManager.SendEmail(parametes, email);
            }
            return true;
        }
    }

   
}
