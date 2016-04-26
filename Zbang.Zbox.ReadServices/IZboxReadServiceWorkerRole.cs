using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto.BoxDtos;
using Zbang.Zbox.ViewModel.Dto.Emails;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Queries.Emails;

namespace Zbang.Zbox.ReadServices
{
    public interface IZboxReadServiceWorkerRole
    {
        Task<IEnumerable<UserDigestDto>> GetUsersByNotificationSettingsAsync(GetUserByNotificationQuery query, CancellationToken token);

        Task<IEnumerable<UserUpdatesDigestDto>> GetUserUpdatesAsync(GetBoxesLastUpdateQuery query,
            CancellationToken token);

        Task<BoxUpdatesDigestDto> GetUpdatesAsync(GetUpdatesQuery query, CancellationToken token);

        Task<IEnumerable<BoxDigestDto>> GetBoxesLastUpdatesAsync(GetBoxesLastUpdateQuery query);
        Task<BoxUpdatesDigestDto> GetBoxLastUpdatesAsync(GetBoxLastUpdateQuery query);

        BadItemDto GetFlagItemUserDetail(GetBadItemFlagQuery query);

        Task<IEnumerable<dynamic>> GetMissingThumbnailBlobsAsync(int index, long start);


        //Task<IEnumerable<UniversitySearchDto>> GetUniversityDetailAsync();

        Task<UniversityToUpdateSearchDto> GetUniversityDirtyUpdatesAsync(int index, int total, int top);
        Task<BoxToUpdateSearchDto> GetBoxDirtyUpdatesAsync(int index, int total, int top);

        Task<ItemToUpdateSearchDto> GetItemDirtyUpdatesAsync(int index, int total, int top);
        Task<QuizToUpdateSearchDto> GetQuizzesDirtyUpdatesAsync(int index, int total, int top);



        Task<IEnumerable<MarketingDto>> GetUsersWithoutUniversityAsync(MarketingQuery query,
            CancellationToken token);

        Task<IEnumerable<MarketingDto>> GetUsersWithUniversityWithoutSubscribedBoxesAsync(MarketingQuery query,
            CancellationToken token);

        Task<IEnumerable<MarketingDto>> GetUsersWithLowActivityUniversitiesAsync(MarketingQuery query,
            CancellationToken token);

        Task<IEnumerable<MarketingDto>> GetUsersFollowingCoursesNoActivityAsync(MarketingQuery query,
            CancellationToken token);

        Task<IEnumerable<LikesDto>> GetLikesDataAsync(CancellationToken token);
    }
}
