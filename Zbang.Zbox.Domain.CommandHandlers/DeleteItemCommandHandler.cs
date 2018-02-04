using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteItemCommandHandler : ICommandHandlerAsync<DeleteItemCommand>
    {
        private readonly IRepository<Box> _boxRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Item> _itemRepository;
        private readonly IRepository<CommentReply> m_CommentReplyRepository;
        private readonly IQueueProvider _queueProvider;
        private readonly IItemTabRepository m_ItemTabRepository;
        private readonly IUpdatesRepository _updatesRepository;

        public DeleteItemCommandHandler(

            IRepository<Box> boxRepository,
            IUserRepository userRepository,
            IRepository<Item> itemRepository,
            IRepository<CommentReply> commentReplyRepository,
            IQueueProvider queueProvider,
            IItemTabRepository itemTabRepository, IUpdatesRepository updatesRepository)
        {
            _boxRepository = boxRepository;
            _userRepository = userRepository;
            _itemRepository = itemRepository;
            m_CommentReplyRepository = commentReplyRepository;
            _queueProvider = queueProvider;
            m_ItemTabRepository = itemTabRepository;
            _updatesRepository = updatesRepository;
        }

        public Task HandleAsync(DeleteItemCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            var item = _itemRepository.Load(command.ItemId);
            var user = _userRepository.Load(command.UserId);
            var userType = _userRepository.GetUserToBoxRelationShipType(user.Id, item.Box.Id);

            bool isAuthorize = userType == UserRelationshipType.Owner
                || Equals(item.User, user)
                || user.IsAdmin();

            if (!isAuthorize)
            {
                throw new UnauthorizedAccessException("User is unauthorized to delete file");
            }

            item.DateTimeUser.UpdateUserTime(user.Id);
            var box = item.Box;
            box.ShouldMakeDirty = () => false;

            if (item.CommentReply != null && string.IsNullOrEmpty(item.CommentReply.Text) && item.CommentReply.Items.Count == 1) // only one answer
            {
                _updatesRepository.DeleteReplyUpdates(item.CommentReply.Id);
                m_CommentReplyRepository.Delete(item.CommentReply);
            }
            if (item.Comment != null)
            {
                var shouldRemove = item.Comment.RemoveItem(item);
                if (shouldRemove)
                {
                    _updatesRepository.DeleteCommentUpdates(item.Comment.Id);
                }
            }
            if (item.Tab != null)
            {
                item.Tab.DeleteItemFromTab(item);
                m_ItemTabRepository.Save(item.Tab);
            }
            var t1 = _queueProvider.InsertMessageToTransactionAsync(new ReputationData(item.UploaderId));
            var t2 = _queueProvider.InsertMessageToTransactionAsync(new QuotaData(item.UploaderId));
            var t4 = _queueProvider.InsertMessageToTransactionAsync(new UploadItemsBadgeData(item.UploaderId));
            var t5 = _queueProvider.InsertFileMessageAsync(new BoxProcessData(box.Id));

            _itemRepository.Delete(item);
            box.UpdateItemCount();
            command.BoxId = box.Id;
            _boxRepository.Save(box);

            return Task.WhenAll(t1, t2, t4, t5);
        }
    }
}
