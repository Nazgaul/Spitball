﻿using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteUpdatesCommandHandler : ICommandHandler<DeleteUpdatesCommand>
    {
        private readonly IUpdatesRepository m_UpdatesRepository;
        public DeleteUpdatesCommandHandler(IUpdatesRepository updatesRepository)
        {
            m_UpdatesRepository = updatesRepository;
        }
        public void Handle(DeleteUpdatesCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            //var feedDelete = message as DeleteUpdatesFeedCommand;
            //if (feedDelete != null)
            //{
            //    m_UpdatesRepository.DeleteCommentUpdates(message.UserId, message.BoxId, feedDelete.CommentId);
            //    return;
            //}
            var itemDelete = message as DeleteUpdatesItemCommand;
            if (itemDelete != null)
            {
                m_UpdatesRepository.DeleteUserItemUpdateByBoxId(message.UserId, message.BoxId);
                return;
            }
            m_UpdatesRepository.DeleteUserUpdateByBoxId(message.UserId, message.BoxId);
        }
    }
}
