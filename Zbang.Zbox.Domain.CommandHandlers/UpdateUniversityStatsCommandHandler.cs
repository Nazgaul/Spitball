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
    public class UpdateUniversityStatsCommandHandler : ICommandHandler<UpdateUniversityStatsCommand>
    {
        private readonly IUniversityRepository m_UniversityRepository;

        public UpdateUniversityStatsCommandHandler(IUniversityRepository universityRepository)
        {
            m_UniversityRepository = universityRepository;
        }

        public void Handle(UpdateUniversityStatsCommand message)
        {
            foreach (var universityId in message.UniversitiesIds)
            {
                var university = m_UniversityRepository.Load(universityId);
                university.AdminScore = m_UniversityRepository.GetAdminScore(universityId);
                university.NoOfUsers = m_UniversityRepository.GetNumberOfUsers(universityId);
                university.NoOfQuizzes = m_UniversityRepository.GetNumberOfQuizzes(universityId);
                university.NoOfItems = m_UniversityRepository.GetNumberOfItems(universityId);

                m_UniversityRepository.Save(university);
            }
        }
    }
}
