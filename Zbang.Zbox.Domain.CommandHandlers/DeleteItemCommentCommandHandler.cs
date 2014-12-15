using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteItemCommentCommandHandler : ICommandHandler<DeleteItemCommentCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<ItemComment> m_ItemCommentRepository;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IQueueProvider m_QueueProvider;

        public DeleteItemCommentCommandHandler(IUserRepository userRepository, 
            IRepository<ItemComment> itemCommentRepository,
            IRepository<Item> itemRepository, IQueueProvider queueProvider)
        {
            m_UserRepository = userRepository;
            m_ItemCommentRepository = itemCommentRepository;
            m_ItemRepository = itemRepository;
            m_QueueProvider = queueProvider;
        }
        public void Handle(DeleteItemCommentCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");

            var user = m_UserRepository.Load(message.UserId);

            var itemComment = m_ItemCommentRepository.Load(message.AnnotationId);
            if (!Equals(itemComment.Author, user))
            {
                throw new UnauthorizedAccessException("User is unauthorized to delete annotation");
            }
            itemComment.Item.DecreaseNumberOfComments();
            m_ItemRepository.Save(itemComment.Item);
            m_QueueProvider.InsertMessageToTranaction(
                new ReputationData(itemComment.GetUserIdReplies().Union(new[] {user.Id})));
            m_ItemCommentRepository.Delete(itemComment);
        }
    }
}
