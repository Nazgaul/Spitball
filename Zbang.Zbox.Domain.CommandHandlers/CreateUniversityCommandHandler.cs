using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CreateUniversityCommandHandler : ICommandHandler<CreateUniversityCommand>
    {
        private readonly IUniversityRepository m_UniversityRepository;
        public CreateUniversityCommandHandler(IUniversityRepository universityRepository)
        {
            m_UniversityRepository = universityRepository;
        }
        public void Handle(CreateUniversityCommand message)
        {
            University university = new University(message.Email, message.Name,
                "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/Lib1.jpg",
                "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/Lib1.jpg",
                System.Security.Principal.WindowsIdentity.GetCurrent().Name);

            university.Country = message.Country;
            m_UniversityRepository.Save(university);
        }
    }
}
