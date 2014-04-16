using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.DTOs.Emails;
using Zbang.Zbox.ViewModel.Queries.Emails;

namespace Zbang.Zbox.ReadServices
{
    public interface IZboxReadServiceWorkerRole
    {
        IEnumerable<UserDigestDto> GetUsersByNotificationSettings(GetUserByNotificationQuery query);
        IEnumerable<BoxDigestDto> GetBoxesLastUpdates(GetBoxesLastUpdateQuery query);
        IEnumerable<ItemDigestDto> GetItemsLastUpdates(GetItemsLastUpdateQuery query);
        IEnumerable<QnADigestDto> GetQuestionsLastUpdates(GetCommentsLastUpdateQuery query);
        IEnumerable<QnADigestDto> GetAnswersLastUpdates(GetCommentsLastUpdateQuery query);
        IEnumerable<MembersDigestDto> GetNewMembersLastUpdates(GetMembersLastUpdateQuery query);
        IEnumerable<QuizDigestDto> GetQuizLastpdates(GetItemsLastUpdateQuery query);

        BadItemDto GetFlagItemUserDetail(GetBadItemFlagQuery query);

        Task<IEnumerable<string>> GetMissingThumbnailBlobs();


        Task<PartnersDto> GetPartnersEmail(long userid);
    }
}
