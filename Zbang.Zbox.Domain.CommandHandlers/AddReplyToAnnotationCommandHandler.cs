
using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddReplyToAnnotationCommandHandler : ICommandHandler<AddReplyToAnnotationCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IRepository<Box> m_BoxRepository;
        private readonly IRepository<ItemComment> m_ItemCommentRepository;
        private readonly IRepository<ItemCommentReply> m_ItemCommentReplyRepository;
        private readonly IQueueProvider m_QueueRepository;


        public AddReplyToAnnotationCommandHandler(IUserRepository userRepository,
            IRepository<Item> itemRepository,
            IRepository<ItemComment> itemCommentRepository,
            IRepository<ItemCommentReply> itemCommentReplyRepository, IRepository<Box> boxRepository, IQueueProvider queueRepository)
        {
            m_UserRepository = userRepository;
            m_ItemRepository = itemRepository;
            m_ItemCommentRepository = itemCommentRepository;
            m_ItemCommentReplyRepository = itemCommentReplyRepository;
            m_BoxRepository = boxRepository;
            m_QueueRepository = queueRepository;
        }
        public void Handle(AddReplyToAnnotationCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var user = m_UserRepository.Load(message.UserId);
            var item = m_ItemRepository.Load(message.ItemId);

            var userType = m_UserRepository.GetUserToBoxRelationShipType(message.UserId, item.Box.Id); //user.GetUserType(box.Id);
            if (userType == UserRelationshipType.None || userType == UserRelationshipType.Invite)
            {
                user.ChangeUserRelationShipToBoxType(item.Box, UserRelationshipType.Subscribe);
                item.Box.CalculateMembers();
                m_UserRepository.Save(user);
                m_BoxRepository.Save(item.Box);
            }

            var itemComment = m_ItemCommentRepository.Load(message.ItemCommentId);
            var comment = new ItemCommentReply(user, item, message.Comment, itemComment);
            m_ItemCommentReplyRepository.Save(comment);

            m_QueueRepository.InsertMessageToTranaction(new ReputationData(user.Id));
            message.ReplyId = comment.Id;
            
        }
    }
}
