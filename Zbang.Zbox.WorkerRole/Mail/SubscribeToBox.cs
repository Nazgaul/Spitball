using System;
using System.Linq;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Mail.Parameters;
using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries.Notification;

namespace Zbang.Zbox.WorkerRole.Mail
{
    internal class SubscribeToBox : IMail
    {
        readonly IZboxReadService m_ZboxReadService;
        readonly IMailManager m_MailManager;

        public SubscribeToBox(IZboxReadService zboxService, IMailManager mailManager)
        {
            m_ZboxReadService = zboxService;
            m_MailManager = mailManager;
        }
        public bool Excecute(MailQueueData data)
        {
            var subscribeData = data as SubscribeToBoxMail;
            if (subscribeData == null)
            {
                throw new ArgumentException("subscribe to box mail did not receive proper stream");
            }
            var query = new GetBoxSubscribers(subscribeData.BoxId, Infrastructure.Enums.NotificationSettings.OnEveryChange);
            var emailList = m_ZboxReadService.GetBoxSubscribersToMail(query);

            var parameters = new Subscription(subscribeData.BoxName, subscribeData.UserName);


            foreach (var email in emailList.Where(w => w.UserId != subscribeData.UserId))
            {
                m_MailManager.SendEmail(parameters, email.Email);
            }
            return true;
        }
    }
}
