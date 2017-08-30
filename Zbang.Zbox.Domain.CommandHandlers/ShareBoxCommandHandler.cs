using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Mail;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class ShareBoxCommandHandler : ICommandHandlerAsync<ShareBoxCommand>
    {
        private readonly IQueueProvider m_QueueProvider;
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Box> m_BoxRepository;
        private readonly IGuidIdGenerator m_IdGenerator;
        private readonly IEmailVerification m_EmailVerification;
        private readonly IRepository<InviteToBox> m_InviteRepository;

        public ShareBoxCommandHandler(IQueueProvider queueProvider, IUserRepository userRepository,
            IRepository<Box> boxRepository,
            IGuidIdGenerator idGenerator,
            IRepository<InviteToBox> inviteRepository, IEmailVerification emailVerification)
        {
            m_BoxRepository = boxRepository;
            m_QueueProvider = queueProvider;
            m_UserRepository = userRepository;
            m_IdGenerator = idGenerator;
            m_InviteRepository = inviteRepository;
            m_EmailVerification = emailVerification;
        }

        public Task HandleAsync(ShareBoxCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var sender = m_UserRepository.Load(command.InviteeId);
            var box = m_BoxRepository.Load(command.BoxId);

            if (box.Actual is AcademicBox)
            {
                return SendInvitesAsync(command, sender, box);
            }
            if (box.Owner.Id == sender.Id)
            {
                return SendInvitesAsync(command, sender, box);
            }
            throw new UnauthorizedAccessException();
        }

        private async Task SendInvitesAsync(ShareBoxCommand command, User sender, Box box)
        {
            var tasks = new List<Task>();
            foreach (var recipientEmail in command.Recipients.Where(w => !string.IsNullOrWhiteSpace(w)).Distinct())
            {
                var recipientUser = GetUser(recipientEmail);
                Guid id = m_IdGenerator.GetId();
                if (recipientUser == null)
                {
                    var verified = await m_EmailVerification.VerifyEmailAsync(recipientEmail);
                    if (!verified)
                    {
                        continue;
                    }

                    var inviteToBox = new InviteToBox(id, sender, box, recipientEmail, recipientEmail);
                    m_InviteRepository.Save(inviteToBox);
                    tasks.Add(SendInviteAsync(sender.Name, box.Name,
                        id, box.Id,
                        recipientEmail, sender.ImageLarge, sender.Email,
                        System.Globalization.CultureInfo.CurrentCulture.Name, box.Url, null));
                    continue;
                }

                var userType = m_UserRepository.GetUserToBoxRelationShipType(recipientUser.Id, box.Id);
                if (userType != UserRelationshipType.None)
                {
                    continue;
                }

                var newInvite = new UserBoxRel(recipientUser, box, UserRelationshipType.Subscribe);
                box.UserBoxRelationship.Add(newInvite);
                box.CalculateMembers();
                m_BoxRepository.Save(box);

                tasks.Add(SendInviteAsync(sender.Name, box.Name,
                    id, box.Id,
                    recipientUser.Email, sender.ImageLarge, sender.Email, recipientUser.Culture, box.Url, recipientUser.Id));
            }
            await Task.WhenAll(tasks);
        }

        private Task SendInviteAsync(string senderName,
            string boxName, Guid id, long boxId,
            string recipientEmail, string senderImage, string senderEmail,
            string culture, string boxUrl, long? recipientId)
        {
            var invId = GuidEncoder.Encode(id);
            var url = UrlConst.BuildInviteUrl(boxUrl, invId);
            return m_QueueProvider.InsertMessageToMailNewAsync(new InviteMailData(senderName, boxName,
                   url,
                   recipientEmail, culture, senderImage, senderEmail, recipientId, boxId));
        }

        private User GetUser(string recipient)
        {
            long userid;
            if (long.TryParse(recipient, out userid))
            {
                return m_UserRepository.Get(userid);
            }
            var user = m_UserRepository.GetUserByEmail(recipient);
            return user;
        }
    }
}