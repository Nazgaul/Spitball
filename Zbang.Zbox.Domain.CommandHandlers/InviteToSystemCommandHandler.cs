
using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Profile;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class InviteToSystemCommandHandler : ICommandHandler<InviteToSystemCommand>
    {
        private readonly IQueueProvider m_QueueProvider;
        private readonly IUserRepository m_UserRepository;
        private readonly IProfilePictureProvider m_ProfilePictureProvider;
        private readonly IRepository<InviteToCloudents> m_InviteToCloudents;
        private readonly IIdGenerator m_IdGenerator;

        public InviteToSystemCommandHandler(IQueueProvider queueProvider, IUserRepository userRepository,
            IProfilePictureProvider profilePictureProvider,
            IRepository<InviteToCloudents> inviteToCloudentsRepository,
            IIdGenerator idGenerator

            )
        {
            m_QueueProvider = queueProvider;
            m_UserRepository = userRepository;
            m_InviteToCloudents = inviteToCloudentsRepository;
            m_ProfilePictureProvider = profilePictureProvider;
            m_IdGenerator = idGenerator;
        }

        public void Handle(InviteToSystemCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            User sender = m_UserRepository.Load(command.SenderId);


            foreach (var recepient in command.Recipients.Where(w => !string.IsNullOrWhiteSpace(w)).Distinct())
            {
                var recepientUser = m_UserRepository.GetUserByEmail(recepient);
                if (recepientUser != null && recepientUser.IsRegisterUser)
                {
                    continue;
                }

                if (!Validation.IsEmailValid2(recepient))
                {
                    continue;
                }
                var images = m_ProfilePictureProvider.GetDefaultProfileImage();
                if (recepientUser == null)
                {
                    recepientUser = new User(recepient, images.Image.AbsoluteUri, images.LargeImage.AbsoluteUri);
                    m_UserRepository.Save(recepientUser, true);
                }


                var invite = new InviteToCloudents(m_IdGenerator.GetId(), sender, recepientUser);
                m_InviteToCloudents.Save(invite);

                m_QueueProvider.InsertMessageToMailNew(new InviteToCloudentsData(sender.Name, sender.Image, recepientUser.Email, sender.Culture, sender.Email));
            }
        }



    }
}
