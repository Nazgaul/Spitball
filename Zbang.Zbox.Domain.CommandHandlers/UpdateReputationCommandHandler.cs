﻿using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateReputationCommandHandler : ICommandHandler<UpdateReputationCommand>
    {
        private readonly IGamificationRepository m_ReputationRepository;
        private readonly IUserRepository m_UserRepository;

        public UpdateReputationCommandHandler(IGamificationRepository reputationRepository, IUserRepository userRepository)
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
                message.Token.ThrowIfCancellationRequested();
                var reputation = m_ReputationRepository.GetUserReputation(userId);
                m_UserRepository.UpdateUserReputation(reputation, userId);



                var user = m_UserRepository.Load(userId);
                var score = m_ReputationRepository.CalculateReputation(userId);
                if (user.UserType == Infrastructure.Enums.UserType.TooHighScore)
                {
                    m_UserRepository.UpdateScore(score/20, userId);
                }
                else
                {
                    m_UserRepository.UpdateScore(score, userId);
                }
                
            }
        }
    }
}
