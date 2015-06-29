using System.Collections.Generic;
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
        Task<IEnumerable<UserDigestDto>> GetUsersByNotificationSettings(GetUserByNotificationQuery query);

        Task<IEnumerable<BoxDigestDto>> GetBoxesLastUpdates(GetBoxesLastUpdateQuery query);
        Task<BoxUpdatesDigestDto> GetBoxLastUpdates(GetBoxLastUpdateQuery query);

        BadItemDto GetFlagItemUserDetail(GetBadItemFlagQuery query);

        Task<IEnumerable<dynamic>> GetMissingThumbnailBlobs(int index, long start);

        Task<PartnersDto> GetPartnersEmail(long userid);

        Task<IEnumerable<UniversitySearchDto>> GetUniversityDetail();

        Task<UniversityToUpdateSearchDto> GetUniversityDirtyUpdates(int index, int total);
        Task<BoxToUpdateSearchDto> GetBoxDirtyUpdates(int index, int total);

        Task<ItemToUpdateSearchDto> GetItemDirtyUpdatesAsync(int index, int total);
        Task<QuizToUpdateSearchDto> GetQuizzesDirtyUpdatesAsync(int index, int total);
    }
}
