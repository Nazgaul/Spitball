
using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CreateUniversityCommandHandler : ICommandHandler<CreateUniversityCommand>
    {
        private readonly IRepository<University> m_UniversityRepository;
        private readonly IRepository<User> m_UserRepository;

        public CreateUniversityCommandHandler(IRepository<University> universityRepository, IRepository<User> userRepository)
        {
            m_UniversityRepository = universityRepository;
            m_UserRepository = userRepository;
        }

        public void Handle(CreateUniversityCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");


            var user = m_UserRepository.Load(message.UserId);
            var university = new University(message.Id, message.Name, message.Country,
                message.SmallImage, message.LargeImage, user.Email
               
                );
            m_UniversityRepository.Save(university);


        }
    }
}
