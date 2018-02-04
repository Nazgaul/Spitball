using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateUniversityStatsCommandHandler : ICommandHandler<UpdateUniversityStatsCommand>
    {
        private readonly IUniversityRepository _universityRepository;

        public UpdateUniversityStatsCommandHandler(IUniversityRepository universityRepository)
        {
            _universityRepository = universityRepository;
        }

        public void Handle(UpdateUniversityStatsCommand message)
        {
            foreach (var universityId in message.UniversitiesIds)
            {
                var university = _universityRepository.Load(universityId);
                university.AdminScore = _universityRepository.GetAdminScore(universityId);
                var universityStats = _universityRepository.GetStats(universityId);

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

                _universityRepository.Save(university);
            }
        }
    }
}
