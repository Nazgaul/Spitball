
using System;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CreateUniversityCommandHandler : ICommandHandlerAsync<CreateUniversityCommand>
    {
        private readonly IRepository<University> m_UniversityRepository;
        private readonly IRepository<User> m_UserRepository;
        private readonly IIdGenerator m_IdGenerator;
        private readonly IQueueProvider m_QueueProvider;

        public CreateUniversityCommandHandler(IRepository<University> universityRepository, 
            IRepository<User> userRepository, IIdGenerator idGenerator, IQueueProvider queueProvider)
        {
            m_UniversityRepository = universityRepository;
            m_UserRepository = userRepository;
            m_IdGenerator = idGenerator;
            m_QueueProvider = queueProvider;
        }

        public Task HandleAsync(CreateUniversityCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));


            var user = m_UserRepository.Load(message.UserId);
            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault Nhibernate doesnt support
            var university = m_UniversityRepository.Query()
                 .Where(w => w.UniversityName == message.Name)
                 .FirstOrDefault();
            var t1 = Task.CompletedTask;
            if (university == null)
            {
                var id = m_IdGenerator.GetId(IdContainer.UniversityScope);
                university = new University(id, message.Name, message.Country,
                    user.Id);
                t1 = m_QueueProvider.InsertFileMessageAsync(new UniversityProcessData(id));
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
            return t1;
            //m_QueueProvider.InsertMessageToTranaction(new UniversityData(message.Name, message.Id, message.LargeImage));


        }
    }
}
