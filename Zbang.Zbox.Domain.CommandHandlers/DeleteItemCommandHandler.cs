using System;
using System.Collections.Generic;
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
        private readonly IRepository<CommentReplies> m_CommentRepliesRepository;
        private readonly IQueueProvider m_QueueProvider;
        private readonly IItemTabRepository m_ItemTabRepository;

        public DeleteItemCommandHandler(

            IRepository<Box> boxRepository,
            IUserRepository userRepository,
            IRepository<Item> itemRepository,
            IRepository<CommentReplies> commentRepliesRepository, IQueueProvider queueProvider, IItemTabRepository itemTabRepository)
        {

            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;
            m_ItemRepository = itemRepository;
            m_CommentRepliesRepository = commentRepliesRepository;
            m_QueueProvider = queueProvider;
            m_ItemTabRepository = itemTabRepository;
        }

        public Task HandleAsync(DeleteItemCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            var userType = m_UserRepository.GetUserToBoxRelationShipType(command.UserId, command.BoxId);


            Item item = m_ItemRepository.Load(command.ItemId);
            User user = m_UserRepository.Load(command.UserId);


            bool isAuthorize = userType == UserRelationshipType.Owner
                || Equals(item.Uploader, user)
                || user.IsAdmin();

            if (!isAuthorize)
            {
                throw new UnauthorizedAccessException("User is unauthorized to delete file");
            }

            item.DateTimeUser.UpdateUserTime(user.Email);
            Box box = m_BoxRepository.Load(command.BoxId);
            box.ShouldMakeDirty = () => false;
            var uploaderFileId = item.UploaderId;
            var usersAffectReputation = new List<long> { uploaderFileId };

            if (item.Answer != null && string.IsNullOrEmpty(item.Answer.Text))
            {
                m_CommentRepliesRepository.Delete(item.Answer);
            }
            if (item.Comment != null)
            {
                var shouldRemove = item.Comment.RemoveItem(item);
                if (shouldRemove)
                {
                    usersAffectReputation.AddRange(box.DeleteComment(item.Comment));
                }
            }
            if (item.Tab != null)
            {
                item.Tab.DeleteItemFromTab(item);
                m_ItemTabRepository.Save(item.Tab);
            }
            var t1 = m_QueueProvider.InsertMessageToTranactionAsync(new ReputationData(usersAffectReputation));
            var t2 = m_QueueProvider.InsertMessageToTranactionAsync(new QuotaData(uploaderFileId));
            m_ItemRepository.Delete(item);
            box.UpdateItemCount();
            m_BoxRepository.Save(box);

            return Task.WhenAll(t1, t2);
        }

    }
}
