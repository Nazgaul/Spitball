using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddReputationCommandHandler : ICommandHandler<AddReputationCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Reputation> m_ReputationRepository;
        public AddReputationCommandHandler(IUserRepository userRepository, IRepository<Reputation> reputationRepository)
        {
            m_UserRepository = userRepository;
            m_ReputationRepository = reputationRepository;
        }
        public void Handle(AddReputationCommand message)
        {
            var user = m_UserRepository.Load(message.UserId);
            var reputation = user.AddReputation(Zbang.Zbox.Infrastructure.Enums.ReputationAction.ShareFacebook);

            m_ReputationRepository.Save(reputation);
            m_UserRepository.Save(user);
        }
    }
}
