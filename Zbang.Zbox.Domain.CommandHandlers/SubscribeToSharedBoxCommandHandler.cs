﻿using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class SubscribeToSharedBoxCommandHandler : ICommandHandlerAsync<SubscribeToSharedBoxCommand>
    {
        private readonly IRepository<Box> m_BoxRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IInviteRepository m_InviteRepository;
        private readonly IRepository<Reputation> m_ReputationRepository;
        private readonly IQueueProvider m_QueueRepository;


        public SubscribeToSharedBoxCommandHandler(IRepository<Box> boxRepository, IUserRepository userRepository,
            IInviteRepository inviteRepository,
            IRepository<Reputation> reputationRepository, IQueueProvider queueRepository)
        {

            m_UserRepository = userRepository;
            m_BoxRepository = boxRepository;
            m_InviteRepository = inviteRepository;
            m_ReputationRepository = reputationRepository;
            m_QueueRepository = queueRepository;
        }

        public async Task HandleAsync(SubscribeToSharedBoxCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
           // var user = m_UserRepository.Load(command.UserId);
            //var box = m_BoxRepository.Load(command.BoxId);

            var userBoxRel = m_UserRepository.GetUserBoxRelationship(command.UserId, command.BoxId);
            var type = UserRelationshipType.None;
            if (userBoxRel != null)
            {
                type = userBoxRel.UserRelationshipType;
            }
            if (type == UserRelationshipType.Owner || type == UserRelationshipType.Subscribe)
            {
                return;
            }
            var box = m_BoxRepository.Load(command.BoxId);
            if (type == UserRelationshipType.Invite ||
                type == UserRelationshipType.None && box.PrivacySettings.PrivacySetting == BoxPrivacySettings.AnyoneWithUrl)
            {
                var user = m_UserRepository.Load(command.UserId);
                user.ChangeUserRelationShipToBoxType(box, UserRelationshipType.Subscribe);
                m_UserRepository.Save(user);
                if (userBoxRel != null) await GiveReputation(userBoxRel.Id);
                box.CalculateMembers();
                m_BoxRepository.Save(box);
            }
        }

        

        private async Task GiveReputation(long userBoxRelId)
        {
            var invite = m_InviteRepository.GetUserInvite(userBoxRelId);
            if (invite == null)
                return;
            if (invite.IsUsed)
                return;
            m_ReputationRepository.Save(invite.Sender.AddReputation(invite.GiveAction()));
            invite.UsedInvite();
            m_UserRepository.Save(invite.Sender);
            await m_QueueRepository.InsertMessageToTranactionAsync(new ReputationData(invite.Sender.Id));
        }
    }
}
