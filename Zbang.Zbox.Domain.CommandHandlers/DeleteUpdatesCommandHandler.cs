using System;
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
            if (message == null) throw new ArgumentNullException("message");
            m_UpdatesRepository.DeleteUserUpdate(message.UserId, message.BoxId);
        }
    }
}
