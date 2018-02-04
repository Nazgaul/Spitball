using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.IdGenerator;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateBadgesCommandHandler : ICommandHandler<UpdateBadgesCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IGamificationRepository _badgeRepository;
        private readonly IGuidIdGenerator _idGenerator;

        public UpdateBadgesCommandHandler(IUserRepository userRepository,
            IGamificationRepository badgeRepository, IGuidIdGenerator idGenerator)
        {
            _userRepository = userRepository;
            _badgeRepository = badgeRepository;
            _idGenerator = idGenerator;
        }

        public void Handle(UpdateBadgesCommand message)
        {
            var user = _userRepository.Load(message.UserId);
            var badge = _badgeRepository.GetBadgeOfUser(message.UserId, message.BadgeType);
            if (badge?.Progress == 100)
            {
                return;
            }
            var progress = new Progress();
            switch (message.BadgeType)
            {
                case BadgeType.None:
                    throw new ArgumentOutOfRangeException();
                case BadgeType.Register:
                    return;
                    //progress.Start = progress.To = 100;
                case BadgeType.FollowClass:
                    progress.Start = user.UserBoxRel.Count;
                    progress.To = 3;
                    break;
                case BadgeType.CreateQuizzes:
                    progress.Start = _userRepository.QuizCount(user.Id);
                    progress.To = 5;
                    break;
                case BadgeType.UploadFiles:
                    progress.Start = _userRepository.ItemCount(user.Id);
                    progress.To = 15;
                    break;
                case BadgeType.Likes:
                    progress.Start = _userRepository.LikesCount(user.Id);
                    progress.To = 50;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            CreateOrUpdateBadge(ref badge, message.BadgeType, user, progress);
            message.Progress = badge.Progress;
            if (badge.Progress == 100)
            {
                user.BadgeCount = user.Badges.Count(w => w.Progress == 100) + 1; //register is virtual
                _userRepository.Save(user);
            }
            _badgeRepository.Save(badge);
        }

        private void CreateOrUpdateBadge(ref Badge badge, BadgeType type, User user, Progress currentNumber)
        {
            var progress = (int)Math.Min((float)currentNumber.Start / currentNumber.To * 100, 100);
            if (badge == null)
            {
                badge = new Badge(_idGenerator.GetId(), user, type, progress);
            }
            badge.Progress = progress;
        }

        private class Progress
        {
            public int Start { get; set; }
            public int To { get; set; }
        }

    }
}
