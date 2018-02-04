
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
        private readonly IRepository<University> _universityRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IIdGenerator _idGenerator;
        private readonly IQueueProvider _queueProvider;

        public CreateUniversityCommandHandler(IRepository<University> universityRepository,
            IRepository<User> userRepository, IIdGenerator idGenerator, IQueueProvider queueProvider)
        {
            _universityRepository = universityRepository;
            _userRepository = userRepository;
            _idGenerator = idGenerator;
            _queueProvider = queueProvider;
        }

        public Task HandleAsync(CreateUniversityCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var user = _userRepository.Load(message.UserId);
            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault Nhibernate doesnt support
            var university = _universityRepository.Query()
                 .Where(w => w.UniversityName == message.Name)
                 .FirstOrDefault();
            var t1 = Task.CompletedTask;
            if (university == null)
            {
                var id = _idGenerator.GetId(IdContainer.UniversityScope);
                university = new University(id, message.Name, message.Country,
                    user.Id);
                t1 = _queueProvider.InsertFileMessageAsync(new UniversityProcessData(id));
                _universityRepository.Save(university);
            }
            if (university.IsDeleted)
            {
                university.IsDeleted = false;
                _universityRepository.Save(university);
            }
            user.UpdateUniversity(university, null);
            message.Id = university.Id;
            _userRepository.Save(user);
            return t1;
            //_queueProvider.InsertMessageToTranaction(new UniversityData(message.Name, message.Id, message.LargeImage));

        }
    }
}
