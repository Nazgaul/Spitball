using System;
using System.Linq;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Mail.Parameters;
using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries.Notification;

namespace Zbang.Zbox.WorkerRole.Mail
{
    internal class DeleteItem : IMail
    {
        readonly IZboxReadService m_ZboxReadService;
        readonly IMailManager m_MailManager;

        public DeleteItem(IZboxReadService zboxService, IMailManager mailManager)
        {
            m_ZboxReadService = zboxService;
            m_MailManager = mailManager;
        }

        public bool Excecute(MailQueueData data)
        {
            var itemdata = data as DeleteItemMail;
            if (itemdata == null)
            {
                throw new ArgumentException("delete item mail did not receive proper stream");
            }

            CreateMailBase parameters = new ItemDeleted(itemdata.BoxName, itemdata.BoxItemName, itemdata.UserNameThatDelete);
          

            var query = new GetBoxSubscribers(itemdata.BoxId, Infrastructure.Enums.NotificationSettings.OnEveryChange);
            var emailList = m_ZboxReadService.GetBoxSubscribersToMail(query);
            foreach (var email in emailList.Where(w => w.UserId != itemdata.UserThatDeleteId))
            {
                m_MailManager.SendEmail(parameters, email.Email);
            }
            return true;
        }
    }
}
