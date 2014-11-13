using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.Infrastructure.Url;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class InviteToSystemCommandHandler : ICommandHandler<InviteToSystemCommand>
    {
        private readonly IQueueProvider m_QueueProvider;
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<InviteToSystem> m_InviteToCloudents;

        private readonly IIdGenerator m_IdGenerator;

        public InviteToSystemCommandHandler(IQueueProvider queueProvider, IUserRepository userRepository,
            IRepository<InviteToSystem> inviteToCloudentsRepository,
            IIdGenerator idGenerator)
        {
            m_QueueProvider = queueProvider;
            m_UserRepository = userRepository;
            m_InviteToCloudents = inviteToCloudentsRepository;
            m_IdGenerator = idGenerator;
        }

        public void Handle(InviteToSystemCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            User sender = m_UserRepository.Load(command.SenderId);


            foreach (var recipientEmail in command.Recipients.Where(w => !string.IsNullOrWhiteSpace(w)).Distinct())
            {
                var recipientUser = m_UserRepository.GetUserByEmail(recipientEmail);
                if (recipientUser != null)
                {
                    continue;
                }

                if (!Validation.IsEmailValid2(recipientEmail))
                {
                    continue;
                }
                var id = m_IdGenerator.GetId();
                var invite = new InviteToSystem(id, sender, null, recipientEmail, recipientEmail);
                m_InviteToCloudents.Save(invite);

                var invId = GuidEncoder.Encode(id);
                var url = UrlConsts.BuildInviteCloudentsUrl(invId);

                m_QueueProvider.InsertMessageToMailNew(new InviteToCloudentsData(sender.Name, sender.Image, recipientEmail, sender.Culture, sender.Email, url));
            }
        }
    }
}
