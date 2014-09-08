
using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CreateUniversityCommandHandler : ICommandHandler<CreateUniversityCommand>
    {
        private readonly IRepository<University> m_UniversityRepository;
        private readonly IRepository<User> m_UserRepository;
        private readonly IQueueProvider m_QueueProvider;

        public CreateUniversityCommandHandler(IRepository<University> universityRepository, IRepository<User> userRepository, IQueueProvider queueProvider)
        {
            m_UniversityRepository = universityRepository;
            m_UserRepository = userRepository;
            m_QueueProvider = queueProvider;
        }

        public void Handle(CreateUniversityCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");


            var user = m_UserRepository.Load(message.UserId);
            var university = new University(message.Id, message.Name, message.Country,
                message.SmallImage, message.LargeImage, user.Email
               
                );
            m_UniversityRepository.Save(university);

            m_QueueProvider.InsertMessageToTranaction(new UniversityData(message.Name, message.Id, message.LargeImage));


        }
    }
}
