using System.Collections.Generic;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.DTOs.Notification;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Notification;

namespace Zbang.Zbox.Domain.Common
{
    //[ServiceContract(Namespace = Constants.ZboxServiceNamespace,
    //                 Name = Constants.ZboxServiceName)]
    public interface IZboxReadOnlyService
    {

        #region BoxBasedData
        BoxDto GetBox(GetBoxQuery query);
        IEnumerable<UserPublicDto> GetBoxUsersWithoutInvites(GetBoxUsersQuery query);
        IList<ItemDto> GetBoxItemsPaged(GetBoxItemsPagedQuery query);
        IEnumerable<UserPublicDto> GetBoxUsers(GetBoxUsersQuery query);         
        IEnumerable<CommentDto> GetBoxComments(GetBoxCommentsQuery query);
        IEnumerable<CommentDto> GetItemComments(GetItemCommentsQuery query);
        ItemDto GetBoxItem(GetItemQuery query);
        IEnumerable<ISearchable> Search(SearchQueryBase query);
        UserPermissionPerBoxDto GetUserPermission(GetBoxQuery query);
        BoxSettingsDto GetBoxSettingsData(GetBoxQuery query);
        bool GetBoxNameExists(GetBoxNameExistsQuery query);

        BoxUpdates GetBoxUpdate(GetBoxUpdatesQuery query);

        #endregion

        #region UserBasedData

        long GetUserDetailsByMembershipId(GetUserByMembershipQuery query);
        IEnumerable<TextDto> GetRecentActivities(GetRecentActivityQuery query);
        UserDto GetUserDetails(GetUserDetailsQuery query);
        IEnumerable<FriendDto> GetUserFriends(GetUserFriendsQuery query);
        IList<UserBoxesDto> GetUserBoxes(GetBoxesQuery query);

        #endregion


        #region Notification
        BoxDataDto GetBoxDataForDigestEmail(GetBoxDataForDigestEmailQuery query);
        BoxDataForImeediateEmail GetBoxDataForItemAdd(GetBoxDataForImmediateEmailQuery query);
        IEnumerable<UserMailDataDto> GetBoxSubscribersToMail(GetBoxSubscribers query);
        IEnumerable<UserDto> GetUserListForDigestEmail(GetUserDigestQuery query);
        IEnumerable<BoxIdDto> GetBoxIdList(GetBoxesDigestQuery query);
        InviteBoxDataDto GetBoxDateForInvite(GetBoxInviteDataQuery query);
        #endregion

        #region tag
        IEnumerable<TagDto> GetTags(GetTagQuery query);
        IEnumerable<TagCheckBoxDto> GetBoxTags(GetBoxTagsQuery query);
        #endregion


        long GetItemIdByBlobId(string blobId);
        
    }


}
