using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.EventHandlers;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Domain.Events;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.ShortUrl;


namespace Zbang.Zbox.Domain.EventHandlers
{
    public class SendMailOnBoxItemCreatedEventHandler : IEventHandler<BoxItemCreatedEvent>
    {
        INotifier m_NotifierManager;
        private IRepository<Box> m_BoxRepository;
        private IRepository<User> m_UserRepository;

        public SendMailOnBoxItemCreatedEventHandler(INotifier notifierManager, IRepository<Box> boxRepository, IRepository<User> userRepository)
        {
            m_NotifierManager = notifierManager;
            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;

        }

        public void Handle(BoxItemCreatedEvent @event)
        {
            Box box = m_BoxRepository.Get(@event.BoxId);



            var boxFiles = box.Items.Where(w => w.GetType() == typeof(File) && !w.IsDeleted).Select(s => s as File);
            var LastThreeFiles = boxFiles.OrderByDescending(o => o.DateTimeUser.CreationTime).Take(FileDetails.NumberOfThumbnailInEmail);

            var LastComments = box.Comments.OrderByDescending(o => o.DateTimeUser.CreationTime).Take(CommentDetails.NumberOfCommets);



            List<FileDetails> FileDataForEmail = LastThreeFiles.Select(c => new FileDetails(ShortCodesCache.LongToShortCode(c.Id, ShortCodesType.item), c.Name, c.ThumbnailBlobName)).ToList();
            List<CommentDetails> CommentDataForEmail = LastComments.Select(s => new CommentDetails() { Comment = s.CommentText, Time = (DateTime.UtcNow - s.DateTimeUser.CreationTime), UserName = s.Author.Email }).ToList();
            //box.Storage.UserId
            CreateMailParams parametes = new CreateMailParams()
            {

                BoxUid = ShortCodesCache.LongToShortCode(box.Id),
                BoxName = box.BoxName,
                Files = FileDataForEmail,

                Comments = CommentDataForEmail,
                BoxOwner = box.GetUserOwner().Email
            };

            //string eventMessage = string.Format("Added a {0}  - <a href='{1}'>{2}</a>", @event.boxItem, @event.boxItemUrl, @event.BoxItemName);
            m_NotifierManager.Notify(@event.BoxId, @event.EmailId, NotificationEventType.Updates, Zbang.Zbox.Infrastructure.Mail.CreateMailParams.Updates, parametes);
        }
    }
}
