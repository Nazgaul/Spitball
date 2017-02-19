using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddItemCommentCommandHandler : ICommandHandlerAsync<AddItemCommentCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IRepository<ItemComment> m_ItemCommentRepository;
        private readonly IQueueProvider m_QueueRepository;
        private readonly IIdGenerator m_IdGenerator;

        public AddItemCommentCommandHandler(IUserRepository userRepository, IRepository<Item> itemRepository,
            IRepository<ItemComment> itemCommentRepository, IQueueProvider queueRepository, IIdGenerator idGenerator)
        {
            m_UserRepository = userRepository;
            m_ItemRepository = itemRepository;
            m_ItemCommentRepository = itemCommentRepository;
            m_QueueRepository = queueRepository;
            m_IdGenerator = idGenerator;
        }
        public Task HandleAsync(AddItemCommentCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var user = m_UserRepository.Load(message.UserId);
            var item = m_ItemRepository.Load(message.ItemId);

            if (message.BoxId != item.Box.Id)
            {
                throw new UnauthorizedAccessException("boxid is not equal to item in box id");
            }
            var comment = new ItemComment(user, item, message.Comment, m_IdGenerator.GetId(IdContainer.ItemAnnotationScope));
            m_ItemCommentRepository.Save(comment);
            m_ItemRepository.Save(item);
            message.CommentId = comment.Id;
            var t1 = m_QueueRepository.InsertMessageToTranactionAsync(new UpdateData(user.Id, item.Box.Id, itemDiscussionId: comment.Id));
            return Task.WhenAll(t1);
        }
    }
}
