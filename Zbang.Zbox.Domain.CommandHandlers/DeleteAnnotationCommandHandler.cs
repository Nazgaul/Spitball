﻿using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteAnnotationCommandHandler : ICommandHandler<DeleteAnnotationCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<ItemComment> m_ItemCommentRepository;
        private readonly IRepository<Item> m_ItemRepository;

        public DeleteAnnotationCommandHandler(IUserRepository userRepository, 
            IRepository<ItemComment> itemCommentRepository,
            IRepository<Item> itemRepository)
        {
            m_UserRepository = userRepository;
            m_ItemCommentRepository = itemCommentRepository;
            m_ItemRepository = itemRepository;
        }
        public void Handle(DeleteAnnotationCommand message)
        {

            Throw.OnNull(message, "message");
            var user = m_UserRepository.Load(message.UserId);

            var itemComment = m_ItemCommentRepository.Load(message.AnnotationId);
            if (!Equals(itemComment.Author, user))
            {
                throw new UnauthorizedAccessException("User is unauthorized to delete annotation");
            }
            itemComment.Item.DecreaseNumberOfComments();
            m_ItemRepository.Save(itemComment.Item);

            m_ItemCommentRepository.Delete(itemComment);
        }
    }
}
