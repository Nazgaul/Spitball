using System.Collections.Generic;

using ZboxDto = Zbang.Zbox.ViewModel.DTOs;
using ZboxQuery = Zbang.Zbox.ViewModel.Queries;

using Zbang.Zbox.ApiViewModel.DTOs;
using Zbang.Zbox.ApiViewModel.Queries;

namespace Zbang.Zbox.ReadServices
{
   public interface IZboxApiReadService : IBaseReadService
    {
       IList<ApiBoxDto> GetBoxes(GetBoxesQueryBase query);
      // IEnumerable<UserDto> GetUserFriends(ZboxQuery.GetUserFriendsQuery query);
       IEnumerable<ZboxDto.UserPublicSettingDto> GetBoxUsers(ZboxQuery.GetBoxUsersQuery query);
       IList<ItemDto> GetBoxItems(ZboxQuery.GetBoxItemsPagedQuery query);
       ItemDto GetBoxItem(ZboxQuery.GetItemQuery query);
       IEnumerable<ApiCommentDto> GetBoxComments(ZboxQuery.GetBoxCommentsQuery query);
       //IEnumerable<ApiCommentDto> GetItemComments(ZboxQuery.GetItemCommentsQuery query);
       IEnumerable<ApiTextDto> GetUserWall(GetWallQuery query);
      // ZboxDto.BoxWithDetailDto GetBox(GetBoxQuery query);
    }
}
