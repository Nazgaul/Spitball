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
        private readonly IProfilePictureProvider m_ProfilePictureProvider;
        private readonly IInviteLinkGenerator m_InviteLinkGenerator;
        private readonly IIdGenerator m_IdGenerator;
        private readonly IInviteRepository m_InviteRepository;


        public ShareBoxCommandHandler(IQueueProvider queueProvider, IUserRepository userRepository,
            IRepository<Box> boxRepository,
            IProfilePictureProvider profilePictureProvider,
            IInviteLinkGenerator inviteLinkGenerator,
            IIdGenerator idGenerator,
            IInviteRepository inviteRepository
            )
        {
            m_BoxRepository = boxRepository;
            m_QueueProvider = queueProvider;
            m_UserRepository = userRepository;
            m_ProfilePictureProvider = profilePictureProvider;
            m_InviteLinkGenerator = inviteLinkGenerator;
            m_IdGenerator = idGenerator;
            m_InviteRepository = inviteRepository;
        }

        public void Handle(ShareBoxCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            User sender;
            Box box;

            ValidateSenderInput(command, out sender, out box);

            foreach (var recepient in command.Recepients.Where(w => !string.IsNullOrWhiteSpace(w)).Distinct())
            {
                var recepientUser = GetUser(recepient);
                if (recepientUser == null)
                {
                    if (!Validation.IsEmailValid2(recepient))
                    {
                        continue;
                    }
                    var images = m_ProfilePictureProvider.GetDefaultProfileImage();
                    recepientUser = new User(recepient, images.Image.AbsoluteUri, images.LargeImage.AbsoluteUri);
                    m_UserRepository.Save(recepientUser, true);
                }

                var userType = m_UserRepository.GetUserToBoxRelationShipType(recepientUser.Id, box.Id);
                if (userType == UserRelationshipType.Subscribe || userType == UserRelationshipType.Owner)
                {
                    continue;
                }

                var currentInvite = m_InviteRepository.GetCurrentInvite(recepientUser, box);
                if (currentInvite == null)
                {
                    currentInvite = new Invite(m_IdGenerator.GetId(), sender, recepientUser, box);
                }
                //dont want to spam to email
                if (currentInvite.SendTime.HasValue && currentInvite.SendTime.Value.AddHours(1) > DateTime.UtcNow)
                {
                    continue;
                }
                currentInvite.UpdateSendTime();
                m_InviteRepository.Save(currentInvite);
                var hash = m_InviteLinkGenerator.GenerateInviteUrl(currentInvite.Id, box.Id, sender.Id, recepientUser.Email);
                var inviteUrl = string.Format(UrlConsts.BoxUrlInvite, hash, recepientUser.Email);
                //var inviteUrl = string.Format(UrlConsts.BoxUrl, box.Id);
                m_QueueProvider.InsertMessageToMailNew(new InviteMailData(sender.Name, box.Name,
                    inviteUrl,
                    recepientUser.Email, recepientUser.Culture, sender.Image, sender.Email));
            }
        }

        private void ValidateSenderInput(ShareBoxCommand command, out User sender, out Box box)
        {
            sender = m_UserRepository.Load(command.InviteeId);
            box = m_BoxRepository.Load(command.BoxId);
            var senderType = m_UserRepository.GetUserToBoxRelationShipType(command.InviteeId, command.BoxId);
            if (senderType == UserRelationshipType.None || senderType == UserRelationshipType.Invite)
            {
                throw new UnauthorizedAccessException("User is not connected to box");
            }
        }

        private User GetUser(string recepient)
        {
            long userid;
            if (long.TryParse(recepient, out userid))
            {
                return m_UserRepository.Get(userid);
            }
            var user = m_UserRepository.GetUserByEmail(recepient);
            if (user != null)
            {
                return user;
            }
            return null;
        }
    }
}