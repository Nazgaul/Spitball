using System;
using System.Linq;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Mail.Parameters;
using Zbang.Zbox.Infrastructure.Routes;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries.Notification;

namespace Zbang.Zbox.WorkerRole.Mail
{
    internal class AddItem : IMail
    {
        readonly IZboxReadService m_ZboxReadService;
        readonly IMailManager m_MailManager;
        readonly IShortCodesCache m_ShortToLongCode;

        public AddItem(IZboxReadService zboxService, IMailManager mailManager, IShortCodesCache shortToLongCode)
        {
            m_ZboxReadService = zboxService;
            m_MailManager = mailManager;
            m_ShortToLongCode = shortToLongCode;
        }
        public bool Excecute(MailQueueData data)
        {
            var itemdata = data as AddItemMail;
            if (itemdata == null)
            {
                throw new ArgumentException("Add item mail did not receive proper stream");
            }            
            var query = new GetBoxDataForImmediateEmailQuery(itemdata.BoxId, itemdata.ItemId);
            var boxdata = m_ZboxReadService.GetBoxDataForItemAdd(query);

            var parametes = new Updates(boxdata.Name, boxdata.Owner,
                                        new Uri(
                                            RoutesCollectionZbox.ShareBoxUri(
                                                m_ShortToLongCode.LongToShortCode(itemdata.BoxId))),
                                        null,
                                        new List<FileDetails> {new FileDetails(boxdata.File.UploaderName,  DateTime.UtcNow - boxdata.File.CreationTime,
                                            boxdata.File.UploaderImage,
                                            boxdata.File.Name,
                                            boxdata.File.ThumbnailBlobUrl)}
                );
            var querySubscribers = new GetBoxSubscribers(itemdata.BoxId, Infrastructure.Enums.NotificationSettings.OnEveryChange);
            var emailList = m_ZboxReadService.GetBoxSubscribersToMail(querySubscribers);
            foreach (var email in emailList.Where(w => w.UserId != itemdata.UploaderId))
            {
                m_MailManager.SendEmail(parametes, email.Email);
            }
            return true;
        }
    }
}
