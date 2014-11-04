using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Security;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class InviteToSystemFacebookCommandHandler : ICommandHandler<InviteToSystemFacebookCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<InviteToSystem> m_InviteToCloudents;
        private readonly IFacebookService m_FacebookPictureService;
        private readonly IIdGenerator m_IdGenerator;


        public InviteToSystemFacebookCommandHandler(
            IUserRepository userRepository,
            IRepository<InviteToSystem> inviteToCloudentsRepository, IFacebookService facebookPictureService, IIdGenerator idGenerator)
        {
            m_UserRepository = userRepository;
            m_InviteToCloudents = inviteToCloudentsRepository;
            m_FacebookPictureService = facebookPictureService;
            m_IdGenerator = idGenerator;
        }

        public void Handle(InviteToSystemFacebookCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var sender = m_UserRepository.Load(message.SenderId);

            
            var recipientUser = m_UserRepository.GetUserByFacebookId(message.FacebookUserId);
            if (recipientUser != null)
            {
                return;
            }
            var id = m_IdGenerator.GetId();
            message.Id = id;
            var invite = new InviteToSystem(id, sender, message.FacebookUserName,
                m_FacebookPictureService.GetFacebookUserImage(message.FacebookUserId, FacebookPictureType.Normal)
               );
            m_InviteToCloudents.Save(invite);
        }
    }
}
