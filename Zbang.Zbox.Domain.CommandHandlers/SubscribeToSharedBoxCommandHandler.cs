using System;
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
        private readonly IRepository<Box> _boxRepository;
        private readonly IUserRepository _userRepository;
        private readonly IInviteRepository _inviteRepository;
        private readonly IQueueProvider _queueRepository;

        public SubscribeToSharedBoxCommandHandler(IRepository<Box> boxRepository, IUserRepository userRepository,
            IInviteRepository inviteRepository,
            IQueueProvider queueRepository
            )
        {
            _userRepository = userRepository;
            _boxRepository = boxRepository;
            _inviteRepository = inviteRepository;
            //_reputationRepository = reputationRepository;
            _queueRepository = queueRepository;
        }

        public async Task HandleAsync(SubscribeToSharedBoxCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var userBoxRel = _userRepository.GetUserBoxRelationship(command.UserId, command.BoxId);
            var type = UserRelationshipType.None;
            if (userBoxRel != null)
            {
                type = userBoxRel.UserRelationshipType;
            }
            if (type == UserRelationshipType.Owner || type == UserRelationshipType.Subscribe)
            {
                return;
            }
            var box = _boxRepository.Load(command.BoxId);
            if (type == UserRelationshipType.Invite ||
                type == UserRelationshipType.None && box.PrivacySettings.PrivacySetting == BoxPrivacySetting.AnyoneWithUrl)
            {
                var user = _userRepository.Load(command.UserId);
                user.ChangeUserRelationShipToBoxType(box, UserRelationshipType.Subscribe);
                _userRepository.Save(user);
                if (userBoxRel != null) GiveReputation(user.Email, box.Id);
                box.CalculateMembers();
                _boxRepository.Save(box);
                var t5 = _queueRepository.InsertFileMessageAsync(new BoxProcessData(box.Id));

                var t1 =  _queueRepository.InsertMessageToTransactionAsync(new FollowClassBadgeData(user.Id));
                await Task.WhenAll(t1, t5).ConfigureAwait(true);
            }
        }

        private void GiveReputation(string email, long boxId)
        {
            var invites = _inviteRepository.GetUserInvitesToBox(email, boxId);
            foreach (var invite in invites)
            {
                if (invite == null)
                    return;
                if (invite.IsUsed)
                    return;
                invite.UsedInvite();
                _userRepository.Save(invite.Sender);
            }
        }
    }
}
