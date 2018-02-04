using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto.BoxDtos;
using Zbang.Zbox.ViewModel.Dto.Emails;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Queries.Emails;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Zbox.ReadServices
{
    public interface IZboxReadServiceWorkerRole
    {
        Task<IEnumerable<UserDigestDto>> GetUsersByNotificationSettingsAsync(GetUserByNotificationQuery query, CancellationToken token);

        Task<IEnumerable<UserUpdatesDigestDto>> GetUserUpdatesAsync(GetBoxesLastUpdateQuery query,
            CancellationToken token);

        Task<BoxUpdatesDigestDto> GetUpdatesAsync(GetUpdatesQuery query, CancellationToken token);



        Task<UniversityToUpdateSearchDto> GetUniversitiesDirtyUpdatesAsync(int index, int total, int top);
        Task<UniversitySearchDto> GetUniversityDirtyUpdatesAsync(long id);

        Task<BoxSearchDto> GetBoxDirtyUpdatesAsync(long id,
            CancellationToken token);
        Task<BoxToUpdateSearchDto> GetBoxesDirtyUpdatesAsync(int index, int total, int top, CancellationToken token);

        Task<ItemToUpdateSearchDto> GetItemsDirtyUpdatesAsync(SearchItemDirtyQuery query, CancellationToken token);
        Task<QuizToUpdateSearchDto> GetQuizzesDirtyUpdatesAsync(int index, int total, int top);

        Task<FlashcardToUpdateSearchDto> GetFlashcardsDirtyUpdatesAsync(int index, int total, int top,
            CancellationToken token);

        Task<IEnumerable<MarketingDto>> GetUsersWithoutUniversityAsync(MarketingQuery query,
            CancellationToken token);

        Task<IEnumerable<MarketingDto>> GetUsersWithUniversityWithoutSubscribedBoxesAsync(MarketingQuery query,
            CancellationToken token);

        Task<IEnumerable<MarketingDto>> GetUsersWithLowActivityUniversitiesAsync(MarketingQuery query,
            CancellationToken token);

        Task<IEnumerable<MarketingDto>> GetUsersFollowingCoursesNoActivityAsync(MarketingQuery query,
            CancellationToken token);

        Task<IEnumerable<LikesDto>> GetLikesDataAsync(CancellationToken token);

        Task<IEnumerable<Tuple<long, string>>> GetDocumentsWithoutMd5Async(long id);

        Task<IEnumerable<Tuple<long, decimal>>> GetDuplicateDocumentsAsync();

        Task<IEnumerable<int>> SpamGunUniversityNumberAsync(CancellationToken token);

       // Task<IEnumerable<SpamGunDto>> GetSpamGunDataAsync(int universityId, CancellationToken token);
        Task<IEnumerable<SpamGunDto>> GetSpamGunDataAsync(int universityId, int limit, CancellationToken token);
    }
}
