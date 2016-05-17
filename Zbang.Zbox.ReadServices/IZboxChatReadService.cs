using System.Collections.Generic;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Zbox.ReadServices
{
    public interface IZboxChatReadService
    {
        Task<IEnumerable<ViewModel.Dto.UserDtos.UserWithStatusDto>> GetUserWithConversationAsync(QueryBase query);
    }
}