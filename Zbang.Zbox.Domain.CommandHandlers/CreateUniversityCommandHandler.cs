
using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.IdGenerator;
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
        private readonly IIdGenerator m_IdGenerator;

        public CreateUniversityCommandHandler(IRepository<University> universityRepository, IRepository<User> userRepository, IQueueProvider queueProvider, IIdGenerator idGenerator)
        {
            m_UniversityRepository = universityRepository;
            m_UserRepository = userRepository;
            m_QueueProvider = queueProvider;
            m_IdGenerator = idGenerator;
        }

        public void Handle(CreateUniversityCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");


            var user = m_UserRepository.Load(message.UserId);
            var university = m_UniversityRepository.GetQuerable()
                 .Where(w => w.UniversityName == message.Name)
                 .FirstOrDefault();
            if (university == null)
            {
                var id = m_IdGenerator.GetId(IdGenerator.UniversityScope);
                university = new University(id, message.Name, message.Country,
                    message.LargeImage, user.Email);
                m_UniversityRepository.Save(university);
                

            }
            user.UpdateUniversity(university, null, null, null, null);
            message.Id = university.Id;
            m_UserRepository.Save(user);

            m_QueueProvider.InsertMessageToTranaction(new UniversityData(message.Name, message.Id, message.LargeImage));


        }
    }
}
