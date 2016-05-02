using System;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteItemCommentCommandHandler : ICommandHandlerAsync<DeleteItemCommentCommand>
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
        public Task HandleAsync(DeleteItemCommentCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var user = m_UserRepository.Load(message.UserId);

            var itemComment = m_ItemCommentRepository.Load(message.AnnotationId);
            if (!Equals(itemComment.Author, user))
            {
                throw new UnauthorizedAccessException("User is unauthorized to delete annotation");
            }
            itemComment.Item.DecreaseNumberOfComments();
            itemComment.Item.ShouldMakeDirty = () => false;
            m_ItemRepository.Save(itemComment.Item);
            m_ItemCommentRepository.Delete(itemComment);
            return m_QueueProvider.InsertMessageToTranactionAsync(
                new ReputationData(itemComment.GetUserIdReplies().Union(new[] {user.Id})));
           
        }
    }
}
