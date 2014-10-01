
using System;
using System.Linq;
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
        private readonly IRepository<InviteToCloudents> m_InviteToCloudents;
        private readonly IIdGenerator m_IdGenerator;
        private readonly IFacebookService m_FacebookPictureService;

        public InviteToSystemFacebookCommandHandler(IUserRepository userRepository,

            IRepository<InviteToCloudents> inviteToCloudentsRepository,
            IIdGenerator idGenerator,
             IFacebookService facebookPictureService
            )
        {
            m_UserRepository = userRepository;
            m_InviteToCloudents = inviteToCloudentsRepository;
            m_IdGenerator = idGenerator;
            m_FacebookPictureService = facebookPictureService;
        }

        public void Handle(InviteToSystemFacebookCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            User sender = m_UserRepository.Load(message.SenderId);


            var recipientUser = m_UserRepository.GetUserByFacebookId(message.FacebookUserId);
            if (recipientUser != null)
            {
                return;
            }


            recipientUser = new User(message.FacebookUserName + "@facebook.com",
                    m_FacebookPictureService.GetFacebookUserImage(message.FacebookUserId, FacebookPictureType.Square),
                    m_FacebookPictureService.GetFacebookUserImage(message.FacebookUserId, FacebookPictureType.Normal),
                    message.FirstName,
                    message.MiddleName,
                    message.LastName,
                    message.Sex,
                    false, System.Globalization.CultureInfo.CurrentCulture.Name
                    ) { FacebookId = message.FacebookUserId };
            m_UserRepository.Save(recipientUser, true);

// ReSharper disable once ReplaceWithSingleCallToFirstOrDefault - nHibernate
            var invite = m_InviteToCloudents.GetQuerable().Where(w => w.Sender == sender && w.Recepient == recipientUser).FirstOrDefault();
            if (invite != null) return;
            invite = new InviteToCloudents(m_IdGenerator.GetId(), sender, recipientUser);
            m_InviteToCloudents.Save(invite);
        }
    }
}
