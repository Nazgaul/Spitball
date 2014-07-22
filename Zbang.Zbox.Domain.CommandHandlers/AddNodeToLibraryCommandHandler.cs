using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    class AddNodeToLibraryCommandHandler : ICommandHandler<AddNodeToLibraryCommand>
    {
        private readonly IRepository<University> m_UniversityRepository;
        private readonly IRepository<Library> m_LibraryRepository;

        public AddNodeToLibraryCommandHandler(IRepository<University> universityRepository, IRepository<Library> libraryRepository)
        {
            m_LibraryRepository = libraryRepository;
            m_UniversityRepository = universityRepository;
        }
        public void Handle(AddNodeToLibraryCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");

            var university = m_UniversityRepository.Load(message.UniversityId);



            if (message.ParentId.HasValue)
            {
                var libraryNode = m_LibraryRepository.Get(message.ParentId);
                var childNode = libraryNode.CreateSubLibrary(message.NewId, message.Name);
                m_LibraryRepository.Save(childNode);
            }
            //need to be in the root
            else
            {
                var lib = university.CreateNewLibraryRoot(message.NewId, message.Name);

                m_LibraryRepository.Save(lib);
                m_UniversityRepository.Save(university);
            }






        }
    }
}
