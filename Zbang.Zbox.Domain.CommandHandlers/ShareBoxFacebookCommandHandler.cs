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
        private readonly IIdGenerator m_IdGenerator;
        private readonly IInviteRepository m_InviteRepository;
        private readonly IFacebookService m_FacebookPictureService;

        public ShareBoxFacebookCommandHandler(
            IUserRepository userRepository,
            IRepository<Box> boxRepository,
            IIdGenerator idGenerator,
            IInviteRepository inviteRepository,
            IFacebookService facebookPictureService)
        {
            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;
            m_IdGenerator = idGenerator;
            m_InviteRepository = inviteRepository;
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
                recipient = new User(message.FacebookUserName + "@facebook.com",
                    m_FacebookPictureService.GetFacebookUserImage(message.FacebookUserId, FacebookPictureType.Square),
                    m_FacebookPictureService.GetFacebookUserImage(message.FacebookUserId, FacebookPictureType.Normal),
                    message.FirstName,
                    message.MiddleName,
                    message.LastName,
                    message.Sex,
                    false, System.Globalization.CultureInfo.CurrentCulture.Name) { FacebookId = message.FacebookUserId };
                m_UserRepository.Save(recipient, true);
            }

            var currentInvite = m_InviteRepository.GetCurrentInvite(recipient, box) ??
                                new Invite(m_IdGenerator.GetId(), sender, recipient, box);
            currentInvite.UpdateSendTime();
            m_InviteRepository.Save(currentInvite);



        }
    }
}
