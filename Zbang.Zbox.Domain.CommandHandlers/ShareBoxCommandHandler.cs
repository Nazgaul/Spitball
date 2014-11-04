using System;
using System.Linq;
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
    public class ShareBoxCommandHandler : ICommandHandler<ShareBoxCommand>
    {
        private readonly IQueueProvider m_QueueProvider;
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Box> m_BoxRepository;
        private readonly IRepository<UserBoxRel> m_UserBoxRelRepository;
        private readonly IIdGenerator m_IdGenerator;
        private readonly IRepository<InviteToBox> m_InviteRepository;
        private readonly IProfilePictureProvider m_ProfilePictureProvider;


        public ShareBoxCommandHandler(IQueueProvider queueProvider, IUserRepository userRepository,
            IRepository<Box> boxRepository,
            IIdGenerator idGenerator,
            IRepository<InviteToBox> inviteRepository, IRepository<UserBoxRel> userBoxRelRepository, IProfilePictureProvider profilePictureProvider)
        {
            m_BoxRepository = boxRepository;
            m_QueueProvider = queueProvider;
            m_UserRepository = userRepository;
            m_IdGenerator = idGenerator;
            m_InviteRepository = inviteRepository;
            m_UserBoxRelRepository = userBoxRelRepository;
            m_ProfilePictureProvider = profilePictureProvider;
        }

        public void Handle(ShareBoxCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            var senderType = m_UserRepository.GetUserToBoxRelationShipType(command.InviteeId, command.BoxId);
            if (senderType == UserRelationshipType.None || senderType == UserRelationshipType.Invite)
            {
                throw new UnauthorizedAccessException("User is not connected to box");
            }

            var sender = m_UserRepository.Load(command.InviteeId);
            var box = m_BoxRepository.Load(command.BoxId);

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

                    var inviteToBox = new InviteToBox(id, sender, box, null, m_ProfilePictureProvider.GetDefaultProfileImage().LargeImage.AbsoluteUri, recipientEmail);
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
                var inviteToBoxExistingUser = new InviteToBox(id, sender, box, newInvite, null, null);

                m_UserBoxRelRepository.Save(newInvite);
                m_InviteRepository.Save(inviteToBoxExistingUser);
                SendInvite(sender.Name, box.Name,
                        id,
                        recipientEmail, sender.Image, sender.Email, recipientUser.Culture, box.Url);
            }
        }

        private void SendInvite(string senderName, string boxName, Guid id, string recipientEmail, string senderImage, string senderEmail, string culture, string boxUrl)
        {
            var invId = GuidEncoder.Encode(id);
            var url = UrlConsts.BuildInviteUrl(boxUrl, invId);
            m_QueueProvider.InsertMessageToMailNew(new InviteMailData(senderName, boxName,
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