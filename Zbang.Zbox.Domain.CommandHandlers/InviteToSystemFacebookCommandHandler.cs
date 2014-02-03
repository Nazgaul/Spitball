using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Profile;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class InviteToSystemFacebookCommandHandler : ICommandHandler<InviteToSystemFacebookCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<InviteToCloudents> m_InviteToCloudents;
        private readonly IIdGenerator m_IdGenerator;
        private readonly IFacebookAuthenticationService m_FacebookPictureService;

        public InviteToSystemFacebookCommandHandler(IUserRepository userRepository,

            IRepository<InviteToCloudents> inviteToCloudentsRepository,
            IIdGenerator idGenerator,
             IFacebookAuthenticationService facebookPrictureService
            )
        {
            m_UserRepository = userRepository;
            m_InviteToCloudents = inviteToCloudentsRepository;
            m_IdGenerator = idGenerator;
            m_FacebookPictureService = facebookPrictureService;
        }

        public void Handle(InviteToSystemFacebookCommand message)
        {
            User sender = m_UserRepository.Load(message.SenderId);


            var recepientUser = m_UserRepository.GetUserByFacebookId(message.FacebookUserId);
            if (recepientUser != null)
            {
                return;
            }


            recepientUser = new User(message.FacebookUserName + "@facebook.com",
                    message.FacebookName,
                    m_FacebookPictureService.GetFacebookUserImage(message.FacebookUserId, FacebookPictureType.square),
                    m_FacebookPictureService.GetFacebookUserImage(message.FacebookUserId, FacebookPictureType.normal));
            recepientUser.FacebookId = message.FacebookUserId;
            m_UserRepository.Save(recepientUser, true);


            var invite = new InviteToCloudents(m_IdGenerator.GetId(), sender, recepientUser);
            m_InviteToCloudents.Save(invite);
        }
    }
}
