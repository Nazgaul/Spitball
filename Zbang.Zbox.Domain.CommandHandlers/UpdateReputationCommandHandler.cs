using System;
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
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (message.UserIds == null) return;
            foreach (var userId in message.UserIds)
            {
                var reputation = m_ReputationRepository.GetUserReputation(userId);
                m_UserRepository.UpdateUserReputation(reputation, userId);

                var score = m_ReputationRepository.CalculateReputation(userId);
                m_UserRepository.UpdateScore(score, userId);
            }
        }
    }
}
