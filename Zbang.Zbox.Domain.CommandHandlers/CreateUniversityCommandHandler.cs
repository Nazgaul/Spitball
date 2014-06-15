
using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CreateUniversityCommandHandler : ICommandHandler<CreateUniversityCommand>
    {
        private readonly IRepository<University> m_UniversityRepository;
        public CreateUniversityCommandHandler(IRepository<University> universityRepository)
        {
            m_UniversityRepository = universityRepository;
        }
        public void Handle(CreateUniversityCommand message)
        {
            var windowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();
            if (windowsIdentity == null)
            {
                throw new NullReferenceException("windowsIdentity");
            }

            var university = new University(message.Email, message.Name,
                "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/Lib1.jpg",
                "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/Lib1.jpg",
                windowsIdentity.Name)
            {
                Country = message.Country
            };
            m_UniversityRepository.Save(university);


        }
    }
}
