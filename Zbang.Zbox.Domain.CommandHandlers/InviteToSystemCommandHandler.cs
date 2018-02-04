using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.Infrastructure.Url;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class InviteToSystemCommandHandler : ICommandHandlerAsync<InviteToSystemCommand>
    {
        private readonly IQueueProvider _queueProvider;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<InviteToSystem> m_InviteToCloudents;
        private readonly IEmailVerification m_EmailVerification;
        private readonly IGuidIdGenerator _idGenerator;

        public InviteToSystemCommandHandler(IQueueProvider queueProvider, IUserRepository userRepository,
            IRepository<InviteToSystem> inviteToCloudentsRepository,
            IGuidIdGenerator idGenerator, IEmailVerification emailVerification)
        {
            _queueProvider = queueProvider;
            _userRepository = userRepository;
            m_InviteToCloudents = inviteToCloudentsRepository;
            _idGenerator = idGenerator;
            m_EmailVerification = emailVerification;
        }

        public async Task HandleAsync(InviteToSystemCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            var sender = _userRepository.Load(command.SenderId);

            var tasks = new List<Task>();

            foreach (var recipientEmail in command.Recipients.Where(w => !string.IsNullOrWhiteSpace(w)).Distinct())
            {
                var recipientUser = _userRepository.GetUserByEmail(recipientEmail);
                if (recipientUser != null)
                {
                    continue;
                }
                var verifyEmail = await m_EmailVerification.VerifyEmailAsync(recipientEmail);
                if (!verifyEmail)
                {
                    continue;
                }
                var id = _idGenerator.GetId();
                var invite = new InviteToSystem(id, sender, null, recipientEmail, recipientEmail);
                m_InviteToCloudents.Save(invite);

                var invId = GuidEncoder.Encode(id);
                var url = UrlConst.BuildInviteCloudentsUrl(invId);

                tasks.Add(
                    _queueProvider.InsertMessageToMailNewAsync(new InviteToCloudentsData(sender.Name, sender.ImageLarge,
                        recipientEmail, sender.Culture, sender.Email, url)));
            }
            await Task.WhenAll(tasks);
        }
    }
}
