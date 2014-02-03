using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class SubscribeToSharedBoxCommandHandler : ICommandHandler<SubscribeToSharedBoxCommand>
    {
        private readonly IRepository<Box> m_BoxRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IQueueProvider m_QueueProvider;
        private readonly IInviteRepository m_InviteRepository;

        const int SubscribeToShareBoxReputation = 5;

        public SubscribeToSharedBoxCommandHandler(IRepository<Box> boxRepository, IUserRepository userRepository,
            IQueueProvider queueProvider,
            IInviteRepository inviteRepository)
        {

            m_UserRepository = userRepository;
            m_BoxRepository = boxRepository;
            m_QueueProvider = queueProvider;
            m_InviteRepository = inviteRepository;

        }

        public void Handle(SubscribeToSharedBoxCommand command)
        {
            bool isSubscribed = false;
            User user = m_UserRepository.Load(command.Id);
            Box box = m_BoxRepository.Load(command.BoxId);

            UserRelationshipType type = m_UserRepository.GetUserToBoxRelationShipTypeWithInvite(user.Id, box.Id);
            if (type == UserRelationshipType.Invite)
            {
                user.ChangeUserRelationShipToBoxType(box, UserRelationshipType.Subscribe);
                var invite = m_InviteRepository.GetCurrentInvite(user, box);
                invite.Sender.AddReputation(SubscribeToShareBoxReputation);

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
