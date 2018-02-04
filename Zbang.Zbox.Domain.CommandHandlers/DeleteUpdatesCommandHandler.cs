using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteUpdatesCommandHandler : ICommandHandler<DeleteUpdatesCommand>
    {
        private readonly IUpdatesRepository _updatesRepository;
        public DeleteUpdatesCommandHandler(IUpdatesRepository updatesRepository)
        {
            _updatesRepository = updatesRepository;
        }

        public void Handle(DeleteUpdatesCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            //var feedDelete = message as DeleteUpdatesFeedCommand;
            //if (feedDelete != null)
            //{
            //    _updatesRepository.DeleteCommentUpdates(message.UserId, message.BoxId, feedDelete.CommentId);
            //    return;
            //}
            var itemDelete = message as DeleteUpdatesItemCommand;
            if (itemDelete != null)
            {
                _updatesRepository.DeleteUserItemUpdateByBoxId(message.UserId, message.BoxId);
                return;
            }
            _updatesRepository.DeleteUserUpdateByBoxId(message.UserId, message.BoxId);
        }
    }
}
