using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateReputationCommandHandler : ICommandHandler<UpdateReputationCommand>
    {
        private readonly IGamificationRepository _reputationRepository;
        private readonly IUserRepository _userRepository;

        public UpdateReputationCommandHandler(IGamificationRepository reputationRepository, IUserRepository userRepository)
        {
            _reputationRepository = reputationRepository;
            _userRepository = userRepository;
        }

        public void Handle(UpdateReputationCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var userId = message.UserId;

            var user = _userRepository.Load(userId);
            var score = _reputationRepository.CalculateReputation(userId);
            if (user.UserType == Infrastructure.Enums.UserType.TooHighScore)
            {
                score = score / 20;
            }
            message.Score = score;
            _userRepository.UpdateScore(score, userId);
        }
    }
}
