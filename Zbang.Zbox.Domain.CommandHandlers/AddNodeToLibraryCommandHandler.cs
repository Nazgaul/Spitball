using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    class AddNodeToLibraryCommandHandler : ICommandHandler<AddNodeToLibraryCommand>
    {
        private readonly IRepository<University> m_UniversityRepository;
        private readonly IRepository<Library> m_LibraryRepository;
        private readonly IIdGenerator m_IdGenerator;

        public AddNodeToLibraryCommandHandler(IRepository<University> universityRepository, IRepository<Library> libraryRepository, IIdGenerator idGenerator)
        {
            m_LibraryRepository = libraryRepository;
            m_IdGenerator = idGenerator;
            m_UniversityRepository = universityRepository;
        }
        public void Handle(AddNodeToLibraryCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");

            var university = m_UniversityRepository.Load(message.UniversityId);

            var id = m_IdGenerator.GetId();

            if (message.ParentId.HasValue)
            {
                var libraryNode = m_LibraryRepository.Get(message.ParentId);
                var childNode = libraryNode.CreateSubLibrary(id, message.Name);
                m_LibraryRepository.Save(childNode);
                message.Url = childNode.Url;
            }
            //need to be in the root
            else
            {
                var lib = university.CreateNewLibraryRoot(id, message.Name);

                m_LibraryRepository.Save(lib);
                m_UniversityRepository.Save(university);
                message.Url = lib.Url;
            }

            message.Id = id;
            
        }
    }
}
