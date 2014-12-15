﻿using System;

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
    public class AddAnnotationCommandHandler : ICommandHandler<AddAnnotationCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IRepository<Box> m_BoxRepository;
        private readonly IRepository<ItemComment> m_ItemCommentRepository;
        private readonly IQueueProvider m_QueueRepository;

        public AddAnnotationCommandHandler(IUserRepository userRepository, IRepository<Item> itemRepository,
            IRepository<ItemComment> itemCommentRepository, IRepository<Box> boxRepository, IQueueProvider queueRepository)
        {
            m_UserRepository = userRepository;
            m_ItemRepository = itemRepository;
            m_ItemCommentRepository = itemCommentRepository;
            m_BoxRepository = boxRepository;
            m_QueueRepository = queueRepository;
        }
        public void Handle(AddAnnotationCommand message)
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
            var text = TextManipulation.EncodeText(message.Comment);
            var comment = new ItemComment(user, item, text);
            item.IncreaseNumberOfComments();
            m_ItemCommentRepository.Save(comment);
            m_ItemRepository.Save(item);
            m_QueueRepository.InsertMessageToTranaction(new ReputationData(user.Id));
            message.AnnotationId = comment.Id;
        }
    }
}
