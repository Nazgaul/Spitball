using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Security;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class ShareBoxFacebookCommandHandler : ICommandHandler<ShareBoxFacebookCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Box> m_BoxRepository;
        private readonly IInviteRepository m_InviteRepository;
        private readonly IRepository<UserBoxRel> m_UserBoxRelRepository;
        private readonly IFacebookService m_FacebookPictureService;

        public ShareBoxFacebookCommandHandler(
            IUserRepository userRepository,
            IRepository<Box> boxRepository,
            IInviteRepository inviteRepository,
             IRepository<UserBoxRel> userBoxRelRepository, IFacebookService facebookPictureService)
        {
            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;
            m_InviteRepository = inviteRepository;
            m_UserBoxRelRepository = userBoxRelRepository;
            m_FacebookPictureService = facebookPictureService;
        }
        public void Handle(ShareBoxFacebookCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");

            var sender = m_UserRepository.Load(message.SenderId);
            var box = m_BoxRepository.Load(message.BoxId);

            var senderType = m_UserRepository.GetUserToBoxRelationShipType(message.SenderId, message.BoxId);
            if (senderType == UserRelationshipType.None || senderType == UserRelationshipType.Invite)
            {
                throw new UnauthorizedAccessException("User is not connected to box");
            }

            var recipient = m_UserRepository.GetUserByFacebookId(message.FacebookUserId);
            if (recipient == null)
            {
                var inviteToBox = new InviteToBox(message.Id, sender, box, null, m_FacebookPictureService.GetFacebookUserImage(message.FacebookUserId, FacebookPictureType.Normal), message.FacebookUserName);
                m_InviteRepository.Save(inviteToBox);
                return;
            }
            var userType = m_UserRepository.GetUserToBoxRelationShipType(recipient.Id, box.Id);
            if (userType != UserRelationshipType.None)
            {
                return;
            }

            var newInvite = new UserBoxRel(recipient, box, UserRelationshipType.Invite);
            var inviteToBoxExistingUser = new InviteToBox(message.Id, sender, box, newInvite, null, null);

            m_UserBoxRelRepository.Save(newInvite);
            m_InviteRepository.Save(inviteToBoxExistingUser);
        }
    }
}
