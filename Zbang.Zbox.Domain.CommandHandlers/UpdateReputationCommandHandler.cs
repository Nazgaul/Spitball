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
    public class UpdateReputationCommandHandler : ICommandHandler<UpdateReputationCommand>
    {
        private readonly IReputationRepository m_ReputationRepository;
        private readonly IUserRepository m_UserRepository;

        public UpdateReputationCommandHandler(IReputationRepository reputationRepository, IUserRepository userRepository)
        {
            m_ReputationRepository = reputationRepository;
            m_UserRepository = userRepository;
        }

        public void Handle(UpdateReputationCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (message.UserIds == null) return;
            foreach (var userId in message.UserIds)
            {
                var user = m_UserRepository.Load(userId);
                var reputation = m_ReputationRepository.GetUserReputation(user.Id);

                user.Reputation = reputation;

                m_UserRepository.Save(user);
            }
        }
    }
}
