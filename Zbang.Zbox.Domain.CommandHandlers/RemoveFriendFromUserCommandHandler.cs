using System;

using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Domain.DataAccess;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class RemoveFriendFromUserCommandHandler : ICommandHandler<RemoveFriendFromUserCommand, RemoveFriendFromUserCommandResult>
    {
        //Fields                
        private readonly IFriendRepository m_FriendRepository;
        private readonly IInvitationRepository m_InvitationRepository;

        //Ctors
        public RemoveFriendFromUserCommandHandler(IFriendRepository friendRepository, IInvitationRepository invitationRepository)
        {
            m_FriendRepository = friendRepository;
            m_InvitationRepository = invitationRepository;
        }

        //Methods
        public RemoveFriendFromUserCommandResult Execute(RemoveFriendFromUserCommand command)
        {
            int friendId = command.FriendId;
            Guid userId = command.UserId;

            Friend friend = m_FriendRepository.Load(friendId);
            foreach (var invitation in m_InvitationRepository.GetFriendInvitation(friend))
            {
                var invitationBase = invitation as Invitation;
                if (invitationBase != null)
                    m_InvitationRepository.Delete(invitationBase);
            }


            if (friend == null)
                throw new ArgumentException("No such friend");

            friend.IsActive = false;

            m_FriendRepository.Save(friend);

            RemoveFriendFromUserCommandResult result = new RemoveFriendFromUserCommandResult();

            return result;
        }
    }
}
