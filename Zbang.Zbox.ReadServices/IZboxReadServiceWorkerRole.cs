using System.Collections.Generic;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto.Emails;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Queries.Emails;

namespace Zbang.Zbox.ReadServices
{
    public interface IZboxReadServiceWorkerRole
    {
        Task<IEnumerable<UserDigestDto>> GetUsersByNotificationSettings(GetUserByNotificationQuery query);

        Task<IEnumerable<BoxDigestDto>> GetBoxesLastUpdates(GetBoxesLastUpdateQuery query);
        //Task<IEnumerable<ItemDigestDto>> GetItemsLastUpdates(GetItemsLastUpdateQuery query);
        //Task<IEnumerable<QnADigestDto>> GetQuestionsLastUpdates(GetCommentsLastUpdateQuery query);
        //Task<IEnumerable<QnADigestDto>> GetAnswersLastUpdates(GetCommentsLastUpdateQuery query);
        //Task<IEnumerable<QuizDigestDto>> GetQuizLastUpdates(GetItemsLastUpdateQuery query);

        //Task<IEnumerable<QuizDiscussionDigestDto>> GetQuizDiscussion(GetCommentsLastUpdateQuery query);
        Task<BoxUpdatesDigestDto> GetBoxLastUpdates(GetBoxLastUpdateQuery query);

        BadItemDto GetFlagItemUserDetail(GetBadItemFlagQuery query);

        Task<IEnumerable<string>> GetMissingThumbnailBlobs();


        Task<PartnersDto> GetPartnersEmail(long userid);

        Task<IEnumerable<UniversitySearchDto>> GetUniversityDetail();
    }
}
