
using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CreateUniversityCommandHandler : ICommandHandler<CreateUniversityCommand>
    {
        private readonly IRepository<University> m_UniversityRepository;
        private readonly IRepository<User> m_UserRepository;
        private readonly IIdGenerator m_IdGenerator;

        public CreateUniversityCommandHandler(IRepository<University> universityRepository, 
            IRepository<User> userRepository, IIdGenerator idGenerator)
        {
            m_UniversityRepository = universityRepository;
            m_UserRepository = userRepository;
            m_IdGenerator = idGenerator;
        }

        public void Handle(CreateUniversityCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));


            var user = m_UserRepository.Load(message.UserId);
            var university = m_UniversityRepository.GetQueryable()
                 .Where(w => w.UniversityName == message.Name)
                 .FirstOrDefault();
            if (university == null)
            {
                var id = m_IdGenerator.GetId(IdContainer.UniversityScope);
                university = new University(id, message.Name, message.Country,
                    user.Id);
                m_UniversityRepository.Save(university);

            }
            if (university.IsDeleted)
            {
                university.IsDeleted = false;
                m_UniversityRepository.Save(university);
            }
            user.UpdateUniversity(university, null);
            message.Id = university.Id;
            m_UserRepository.Save(user);

            //m_QueueProvider.InsertMessageToTranaction(new UniversityData(message.Name, message.Id, message.LargeImage));


        }
    }
}
