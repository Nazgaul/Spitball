using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddAnnotationCommandHandler : ICommandHandlerAsync<AddAnnotationCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IRepository<ItemComment> m_ItemCommentRepository;
        private readonly IQueueProvider m_QueueRepository;

        public AddAnnotationCommandHandler(IUserRepository userRepository, IRepository<Item> itemRepository,
            IRepository<ItemComment> itemCommentRepository, IQueueProvider queueRepository)
        {
            m_UserRepository = userRepository;
            m_ItemRepository = itemRepository;
            m_ItemCommentRepository = itemCommentRepository;
            m_QueueRepository = queueRepository;
        }
        public Task HandleAsync(AddAnnotationCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");

            var user = m_UserRepository.Load(message.UserId);
            var item = m_ItemRepository.Load(message.ItemId);

            if (message.BoxId != item.Box.Id)
            {
                throw new UnauthorizedAccessException("boxid is not equal to item in box id");
            }
           
            var text = TextManipulation.EncodeText(message.Comment);
            var comment = new ItemComment(user, item, text);
            item.IncreaseNumberOfComments();
            m_ItemCommentRepository.Save(comment);
            m_ItemRepository.Save(item);
            message.AnnotationId = comment.Id;
            return m_QueueRepository.InsertMessageToTranactionAsync(new ReputationData(user.Id));
            
        }
    }
}
