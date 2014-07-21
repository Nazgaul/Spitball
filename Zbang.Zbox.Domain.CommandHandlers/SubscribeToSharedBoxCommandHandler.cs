using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class SubscribeToSharedBoxCommandHandler : ICommandHandler<SubscribeToSharedBoxCommand>
    {
        private readonly IRepository<Box> m_BoxRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IInviteRepository m_InviteRepository;
        private readonly IRepository<Reputation> m_ReputationRepository;


        public SubscribeToSharedBoxCommandHandler(IRepository<Box> boxRepository, IUserRepository userRepository,
            IInviteRepository inviteRepository,
            IRepository<Reputation> reputationRepository)
        {

            m_UserRepository = userRepository;
            m_BoxRepository = boxRepository;
            m_InviteRepository = inviteRepository;
            m_ReputationRepository = reputationRepository;

        }

        public void Handle(SubscribeToSharedBoxCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            bool isSubscribed = false;
            User user = m_UserRepository.Load(command.Id);
            Box box = m_BoxRepository.Load(command.BoxId);

            UserRelationshipType type = m_UserRepository.GetUserToBoxRelationShipTypeWithInvite(user.Id, box.Id);
            if (type == UserRelationshipType.Invite)
            {
                user.ChangeUserRelationShipToBoxType(box, UserRelationshipType.Subscribe);
                var invite = m_InviteRepository.GetCurrentInvite(user, box);
                m_ReputationRepository.Save(invite.Sender.AddReputation(ReputationAction.InviteToBox));

                isSubscribed = true;
                m_UserRepository.Save(user);
                m_UserRepository.Save(invite.Sender);
            }

            if (type == UserRelationshipType.None && box.PrivacySettings.PrivacySetting == BoxPrivacySettings.AnyoneWithUrl)
            {
                user.ChangeUserRelationShipToBoxType(box, UserRelationshipType.Subscribe);
                isSubscribed = true;
                m_UserRepository.Save(user);
            }

            if (isSubscribed)
            {
                box.CalculateMembers();
                box.UserTime.UpdateUserTime(user.Email);
                m_BoxRepository.Save(box);
            }
        }
    }
}
