using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
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
        private readonly IFacebookAuthenticationService m_FacebookPictureService;

        public ShareBoxFacebookCommandHandler(
            IUserRepository userRepository,
            IRepository<Box> boxRepository,
            IIdGenerator idGenerator,
            IInviteRepository inviteRepository,
            IFacebookAuthenticationService facebookPrictureService)
        {
            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;
            m_IdGenerator = idGenerator;
            m_InviteRepository = inviteRepository;
            m_FacebookPictureService = facebookPrictureService;
        }
        public void Handle(ShareBoxFacebookCommand message)
        {
            Throw.OnNull(message, "message");

            var sender = m_UserRepository.Load(message.SenderId);
            var box = m_BoxRepository.Load(message.BoxId);

            var senderType = m_UserRepository.GetUserToBoxRelationShipType(message.SenderId, message.BoxId);
            if (senderType == UserRelationshipType.None || senderType == UserRelationshipType.Invite)
            {
                throw new UnauthorizedAccessException("User is not connected to box");
            }

            var recepient = m_UserRepository.GetUserByFacebookId(message.FacebookUserId);
            if (recepient == null)
            {
                recepient = new User(message.FacebookUserName + "@facebook.com", 
                    message.FacebookName,
                    m_FacebookPictureService.GetFacebookUserImage(message.FacebookUserId,FacebookPictureType.square),
                    m_FacebookPictureService.GetFacebookUserImage(message.FacebookUserId,FacebookPictureType.normal));
                recepient.FacebookId = message.FacebookUserId;
                m_UserRepository.Save(recepient, true);
            }

            var currentInvite = m_InviteRepository.GetCurrentInvite(recepient, box);
            if (currentInvite == null)
            {
                currentInvite = new Invite(m_IdGenerator.GetId(), sender, recepient, box);
            }
            currentInvite.UpdateSendTime();
            m_InviteRepository.Save(currentInvite);

            

        }
    }
}
