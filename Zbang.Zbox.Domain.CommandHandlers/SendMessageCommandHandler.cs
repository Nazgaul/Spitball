using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class SendMessageCommandHandler : ICommandHandlerAsync<SendMessageCommand>
    {
        private readonly IQueueProvider m_QueueProvider;
        private readonly IUserRepository m_UserRepository;
        private readonly IIdGenerator m_IdGenerator;
        private readonly IRepository<Message> m_MessageRepository;

        public SendMessageCommandHandler(IQueueProvider queueProvider,
            IUserRepository userRepository,
            IIdGenerator idGenerator,
        IRepository<Message> messageRepository
           )
        {
            m_QueueProvider = queueProvider;
            m_UserRepository = userRepository;
            m_IdGenerator = idGenerator;
            m_MessageRepository = messageRepository;
        }

        public Task HandleAsync(SendMessageCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            var sender = m_UserRepository.Load(command.Sender);

            var tasks = new List<Task>();
            foreach (var recipient in command.Recipients.Where(w => !string.IsNullOrWhiteSpace(w)).Distinct())
            {
                var recipientUser = GetUser(recipient);
                if (recipientUser == null)
                {
                    if (!Validation.IsEmailValid2(recipient))
                    {
                        continue;
                    }
                    recipientUser = new User(recipient, null, null);
                    m_UserRepository.Save(recipientUser);


                    // TriggerSendMail(command.PersonalNote, recipient, sender.Name, Zbang.Zbox.Infrastructure.Culture.Languages.GetDefaultSystemCulture().Culture);
                    //continue;
                }
                var message = new Message(m_IdGenerator.GetId(), sender, recipientUser, command.PersonalNote);
                m_MessageRepository.Save(message);
                tasks.Add(TriggerSendMailAsync(command.PersonalNote, recipientUser.Email, sender.Name, recipientUser.Culture, sender.Image, sender.Email));
            }
            return Task.WhenAll(tasks);
        }

        private Task TriggerSendMailAsync(string personalNote, string email, string senderUserName, string culture, string senderImage, string senderEmail)
        {
            return m_QueueProvider.InsertMessageToMailNewAsync(new MessageMailData2(personalNote, email, senderUserName, senderImage, senderEmail, culture));
        }


        private User GetUser(string recipient)
        {
            long userid;
            if (long.TryParse(recipient, out userid))
            {
                return m_UserRepository.Get(userid);
            }
            
            var user = m_UserRepository.GetUserByEmail(recipient);
            if (user != null)
            {
                return user;
            }
            return null;
        }
    }
}
