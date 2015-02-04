﻿
using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddReplyToAnnotationCommandHandler : ICommandHandlerAsync<AddReplyToAnnotationCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IRepository<ItemComment> m_ItemCommentRepository;
        private readonly IRepository<ItemCommentReply> m_ItemCommentReplyRepository;
        private readonly IQueueProvider m_QueueRepository;
        private readonly IIdGenerator m_IdGenerator;


        public AddReplyToAnnotationCommandHandler(IUserRepository userRepository,
            IRepository<Item> itemRepository,
            IRepository<ItemComment> itemCommentRepository,
            IRepository<ItemCommentReply> itemCommentReplyRepository, IQueueProvider queueRepository, IIdGenerator idGenerator)
        {
            m_UserRepository = userRepository;
            m_ItemRepository = itemRepository;
            m_ItemCommentRepository = itemCommentRepository;
            m_ItemCommentReplyRepository = itemCommentReplyRepository;
            m_QueueRepository = queueRepository;
            m_IdGenerator = idGenerator;
        }
        public Task HandleAsync(AddReplyToAnnotationCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var user = m_UserRepository.Load(message.UserId);
            var item = m_ItemRepository.Load(message.ItemId);

            if (message.BoxId != item.Box.Id)
            {
                throw new UnauthorizedAccessException("boxid is not equal to item in box id");
            }


            var itemComment = m_ItemCommentRepository.Load(message.ItemCommentId);
            var id = m_IdGenerator.GetId(IdContainer.ItemAnnotationReplyScope);
            var comment = new ItemCommentReply(user, item, message.Comment, itemComment, id);
            m_ItemCommentReplyRepository.Save(comment);
            message.ReplyId = comment.Id;
            return m_QueueRepository.InsertMessageToTranactionAsync(new ReputationData(user.Id));


        }
    }
}
