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
                var universityStats = m_UniversityRepository.GetStats(universityId);

                if (university.NoOfUsers != universityStats.UsersCount)
                {
                    university.ShouldMakeDirty = () => true;
                }
                else
                {
                    university.ShouldMakeDirty = () => false;
                }

                university.NoOfUsers = universityStats.UsersCount;
                university.NoOfQuizzes = universityStats.QuizzesCount;
                university.NoOfItems = universityStats.ItemsCount;
                university.UpdateNumberOfBoxes( universityStats.BoxesCount);
                university.NoOfFlashcards = universityStats.FlashcardCount;

                m_UniversityRepository.Save(university);
            }
        }
    }
}
