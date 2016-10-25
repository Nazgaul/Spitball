using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    class AddNodeToLibraryCommandHandler : ICommandHandler<AddNodeToLibraryCommand>
    {
        private readonly IRepository<University> m_UniversityRepository;
        private readonly IRepository<Library> m_LibraryRepository;
        private readonly IGuidIdGenerator m_IdGenerator;
        private readonly IUserRepository m_UserRepository;

        public AddNodeToLibraryCommandHandler(IRepository<University> universityRepository, IRepository<Library> libraryRepository, IGuidIdGenerator idGenerator, IUserRepository userRepository)
        {
            m_LibraryRepository = libraryRepository;
            m_IdGenerator = idGenerator;
            m_UserRepository = userRepository;
            m_UniversityRepository = universityRepository;
        }
        public void Handle(AddNodeToLibraryCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var user = m_UserRepository.Load(message.UserId);
            if (message.UniversityId != user.University.Id)
            {
                throw new UnauthorizedAccessException();
            }


            var university = m_UniversityRepository.Load(message.UniversityId);

            var id = m_IdGenerator.GetId();

            if (message.ParentId.HasValue)
            {
                var libraryNode = m_LibraryRepository.Load(message.ParentId);
                var childNode = libraryNode.CreateSubLibrary(id, message.Name, user);
                m_LibraryRepository.Save(childNode);
            }
            //need to be in the root
            else
            {
                var lib = university.CreateNewLibraryRoot(id, message.Name, user);

                m_LibraryRepository.Save(lib);
                university.ShouldMakeDirty = () => false;
                m_UniversityRepository.Save(university);
            }

            message.Id = id;

        }
    }
}
