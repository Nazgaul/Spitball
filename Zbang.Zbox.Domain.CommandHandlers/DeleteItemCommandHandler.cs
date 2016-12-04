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

        private readonly IRepository<Box> m_BoxRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IRepository<CommentReply> m_CommentReplyRepository;
        private readonly IQueueProvider m_QueueProvider;
        private readonly IItemTabRepository m_ItemTabRepository;
        private readonly IUpdatesRepository m_UpdatesRepository;

        public DeleteItemCommandHandler(

            IRepository<Box> boxRepository,
            IUserRepository userRepository,
            IRepository<Item> itemRepository,
            IRepository<CommentReply> commentReplyRepository,
            IQueueProvider queueProvider,
            IItemTabRepository itemTabRepository, IUpdatesRepository updatesRepository)
        {

            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;
            m_ItemRepository = itemRepository;
            m_CommentReplyRepository = commentReplyRepository;
            m_QueueProvider = queueProvider;
            m_ItemTabRepository = itemTabRepository;
            m_UpdatesRepository = updatesRepository;
        }

        public Task HandleAsync(DeleteItemCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            var item = m_ItemRepository.Load(command.ItemId);
            var user = m_UserRepository.Load(command.UserId);
            var userType = m_UserRepository.GetUserToBoxRelationShipType(user.Id, item.Box.Id);

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
                m_UpdatesRepository.DeleteReplyUpdates(item.CommentReply.Id);
                m_CommentReplyRepository.Delete(item.CommentReply);
            }
            if (item.Comment != null)
            {
                var shouldRemove = item.Comment.RemoveItem(item);
                if (shouldRemove)
                {
                    m_UpdatesRepository.DeleteCommentUpdates(item.Comment.Id);
                }
            }
            if (item.Tab != null)
            {
                item.Tab.DeleteItemFromTab(item);
                m_ItemTabRepository.Save(item.Tab);
            }
            var t1 = m_QueueProvider.InsertMessageToTranactionAsync(new ReputationData(item.UploaderId));
            var t2 = m_QueueProvider.InsertMessageToTranactionAsync(new QuotaData(item.UploaderId));
            var t4 = m_QueueProvider.InsertMessageToTranactionAsync(new UploadItemsBadgeData(item.UploaderId));
            m_ItemRepository.Delete(item);
            box.UpdateItemCount();
            command.BoxId = box.Id;
            m_BoxRepository.Save(box);

            return Task.WhenAll(t1, t2, t4);
        }

    }
}
