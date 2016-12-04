using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateBadgesCommandHandler : ICommandHandler<UpdateBadgesCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IGamificationRepository m_BadgeRepository;
        private readonly IGuidIdGenerator m_IdGenerator;

        public UpdateBadgesCommandHandler(IUserRepository userRepository, IGamificationRepository badgeRepository, IGuidIdGenerator idGenerator)
        {
            m_UserRepository = userRepository;
            m_BadgeRepository = badgeRepository;
            m_IdGenerator = idGenerator;
        }


        public void Handle(UpdateBadgesCommand message)
        {
            var user = m_UserRepository.Load(message.UserId);
            var badge = m_BadgeRepository.GetBadgeOfUser(message.UserId, message.BadgeType);
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
                    progress.Start = progress.To = 100;
                    break;
                case BadgeType.FollowClass:
                    progress.Start = user.UserBoxRel.Count;
                    progress.To = 3;
                    break;
                case BadgeType.CreateQuizzes:
                    progress.Start = user.Quizzes.Count(w => !w.IsDeleted);
                    progress.To = 5;
                    break;
                case BadgeType.UploadFiles:
                    progress.Start = user.Items.Count(w => !w.IsDeleted);
                    progress.To = 15;
                    break;
                case BadgeType.Likes:
                    progress.Start = m_UserRepository.LikesCount(user.Id);
                    progress.To = 50;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            CreateOrUpdateBadge(ref badge, message.BadgeType, user, progress);
            m_BadgeRepository.Save(badge);
        }

        private void CreateOrUpdateBadge(ref Badge badge, BadgeType type, User user, Progress currentNumber)
        {
            var progress = (int)Math.Min((float)currentNumber.Start / currentNumber.To * 100, 100);
            if (badge == null)
            {
                badge = new Badge(m_IdGenerator.GetId(), user, type, progress);
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
