using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Profile;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class SendMessageCommandHandler : ICommandHandler<SendMessageCommand>
    {
        private readonly IQueueProvider m_QueueProvider;
        private readonly IUserRepository m_UserRepository;
        private readonly IProfilePictureProvider m_ProfilePictureProvider;
        private readonly IIdGenerator m_IdGenerator;
        private readonly IRepository<Message> m_MessageRepository;

        public SendMessageCommandHandler(IQueueProvider queueProvider,
            IUserRepository userRepository, IProfilePictureProvider profilePictureProvider,
            IIdGenerator idGenerator,
        IRepository<Message> messageRepository
           )
        {
            m_QueueProvider = queueProvider;
            m_UserRepository = userRepository;
            m_ProfilePictureProvider = profilePictureProvider;
            m_IdGenerator = idGenerator;
            m_MessageRepository = messageRepository;
        }

        public void Handle(SendMessageCommand command)
        {
            var sender = m_UserRepository.Load(command.Sender);

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
                    recepientUser = new User(recepient, recepient, images.Image.AbsoluteUri, images.LargeImage.AbsoluteUri);
                    m_UserRepository.Save(recepientUser);


                   // TriggerSendMail(command.PersonalNote, recepient, sender.Name, Zbang.Zbox.Infrastructure.Culture.Languages.GetDefaultSystemCulture().Culture);
                    //continue;
                }
                var message = new Message(m_IdGenerator.GetId(), sender, recepientUser, command.PersonalNote);
                m_MessageRepository.Save(message);
                TriggerSendMail(command.PersonalNote, recepientUser.Email, sender.Name, recepientUser.Culture);
            }
        }

        private void TriggerSendMail(string personalNote, string email, string senderUserName, string culture)
        {
            m_QueueProvider.InsertMessageToMailNew(new MessageMailData(personalNote, email, senderUserName, culture));
        }


        private User GetUser(string recepient)
        {
            long userid;
            if (long.TryParse(recepient, out userid))
            {
                return m_UserRepository.Get(userid);
            }
            //var recepientUserId = m_ShortCodesCache.ShortCodeToLong(recepient, ShortCodesType.User);
            // var recepientUser = m_UserRepository.Get(recepientUserId);

            //if (recepientUser != null)
            //{
            //    return recepientUser;
            //}
            var user = m_UserRepository.GetUserByEmail(recepient);
            if (user != null)
            {
                return user;
            }
            return null;
        }
    }
}
