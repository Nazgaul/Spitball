using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Profile;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.Infrastructure.IdGenerator;


namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class ShareBoxCommandHandler : ICommandHandlerAsync<ShareBoxCommand>
    {
        private readonly IQueueProvider m_QueueProvider;
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Box> m_BoxRepository;
        private readonly IRepository<UserBoxRel> m_UserBoxRelRepository;
        private readonly IIdGenerator m_IdGenerator;
        private readonly IRepository<InviteToBox> m_InviteRepository;


        public ShareBoxCommandHandler(IQueueProvider queueProvider, IUserRepository userRepository,
            IRepository<Box> boxRepository,
            IIdGenerator idGenerator,
            IRepository<InviteToBox> inviteRepository, IRepository<UserBoxRel> userBoxRelRepository)
        {
            m_BoxRepository = boxRepository;
            m_QueueProvider = queueProvider;
            m_UserRepository = userRepository;
            m_IdGenerator = idGenerator;
            m_InviteRepository = inviteRepository;
            m_UserBoxRelRepository = userBoxRelRepository;
        }

        public Task HandleAsync(ShareBoxCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            var senderType = m_UserRepository.GetUserToBoxRelationShipType(command.InviteeId, command.BoxId);
            if (senderType == UserRelationshipType.None || senderType == UserRelationshipType.Invite)
            {
                throw new UnauthorizedAccessException("User is not connected to box");
            }

            var sender = m_UserRepository.Load(command.InviteeId);
            var box = m_BoxRepository.Load(command.BoxId);

            var tasks = new List<Task>();
            foreach (var recipientEmail in command.Recipients.Where(w => !string.IsNullOrWhiteSpace(w)).Distinct())
            {
                var recipientUser = GetUser(recipientEmail);
                Guid id = m_IdGenerator.GetId();
                if (recipientUser == null)
                {
                    if (!Validation.IsEmailValid2(recipientEmail))
                    {
                        continue;
                    }

                    var inviteToBox = new InviteToBox(id, sender, box, null, null, recipientEmail, recipientEmail);
                    m_InviteRepository.Save(inviteToBox);
                    SendInvite(sender.Name, box.Name,
                        id,
                        recipientEmail, sender.Image, sender.Email,
                        System.Globalization.CultureInfo.CurrentCulture.Name, box.Url);
                    continue;


                }

                var userType = m_UserRepository.GetUserToBoxRelationShipType(recipientUser.Id, box.Id);
                if (userType != UserRelationshipType.None)
                {
                    continue;
                }

                var newInvite = new UserBoxRel(recipientUser, box, UserRelationshipType.Invite);
                var inviteToBoxExistingUser = new InviteToBox(id, sender, box, newInvite, null, null, recipientEmail);

                m_UserBoxRelRepository.Save(newInvite);
                m_InviteRepository.Save(inviteToBoxExistingUser);
                tasks.Add(SendInvite(sender.Name, box.Name,
                    id,
                    recipientEmail, sender.Image, sender.Email, recipientUser.Culture, box.Url));
            }
            return Task.WhenAll(tasks);
        }

        private Task SendInvite(string senderName, string boxName, Guid id, string recipientEmail, string senderImage, string senderEmail, string culture, string boxUrl)
        {
            var invId = GuidEncoder.Encode(id);
            var url = UrlConsts.BuildInviteUrl(boxUrl, invId);
           return m_QueueProvider.InsertMessageToMailNewAsync(new InviteMailData(senderName, boxName,
                  url,
                  recipientEmail, culture, senderImage, senderEmail));
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