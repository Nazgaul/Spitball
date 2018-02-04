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
    public class DeleteItemCommentCommandHandler : ICommandHandler<DeleteItemCommentCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRepository<ItemComment> m_ItemCommentRepository;
        private readonly IRepository<Item> _itemRepository;
        private readonly IQueueProvider _queueProvider;

        public DeleteItemCommentCommandHandler(IUserRepository userRepository,
            IRepository<ItemComment> itemCommentRepository,
            IRepository<Item> itemRepository, IQueueProvider queueProvider)
        {
            _userRepository = userRepository;
            m_ItemCommentRepository = itemCommentRepository;
            _itemRepository = itemRepository;
            _queueProvider = queueProvider;
        }

        public void Handle(DeleteItemCommentCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var user = _userRepository.Load(message.UserId);

            var itemComment = m_ItemCommentRepository.Load(message.ItemCommentId);
            if (!Equals(itemComment.Author, user))
            {
                throw new UnauthorizedAccessException("User is unauthorized to delete annotation");
            }
            _itemRepository.Save(itemComment.Item);
            m_ItemCommentRepository.Delete(itemComment);
        }
    }
}
