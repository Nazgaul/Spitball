using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto.BoxDtos;
using Zbang.Zbox.ViewModel.Dto.Emails;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Emails;

namespace Zbang.Zbox.ReadServices
{
    public interface IZboxReadServiceWorkerRole
    {
        Task<IEnumerable<UserDigestDto>> GetUsersByNotificationSettings(GetUserByNotificationQuery query);

        Task<IEnumerable<BoxDigestDto>> GetBoxesLastUpdates(GetBoxesLastUpdateQuery query);
        Task<BoxUpdatesDigestDto> GetBoxLastUpdates(GetBoxLastUpdateQuery query);

        BadItemDto GetFlagItemUserDetail(GetBadItemFlagQuery query);

        Task<IEnumerable<dynamic>> GetMissingThumbnailBlobs(int index, long start);


        Task<IEnumerable<UniversitySearchDto>> GetUniversityDetail();

        Task<UniversityToUpdateSearchDto> GetUniversityDirtyUpdates(int index, int total, int top);
        Task<BoxToUpdateSearchDto> GetBoxDirtyUpdates(int index, int total, int top);

        Task<ItemToUpdateSearchDto> GetItemDirtyUpdatesAsync(int index, int total, int top);
        Task<QuizToUpdateSearchDto> GetQuizzesDirtyUpdatesAsync(int index, int total, int top);



        Task<IEnumerable<MarketingDto>> GetUsersWithoutUniversityAsync(MarketingQuery query,
            CancellationToken token);

        Task<IEnumerable<MarketingDto>> GetUsersWithUniversityWithoutSubscribedBoxesAsync(MarketingQuery query,
            CancellationToken token);
    }
}
