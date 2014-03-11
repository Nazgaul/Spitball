using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var updates = m_UpdatesRepository.GetUserBoxUpdates(message.UserId, message.BoxId);

            foreach (var update in updates)
            {
                m_UpdatesRepository.Delete(update);
            }
        }
    }
}
