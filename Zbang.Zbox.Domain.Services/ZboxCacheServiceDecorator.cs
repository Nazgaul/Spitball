using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.ViewModel.Queries.Notification;
using Zbang.Zbox.ViewModel.DTOs.Notification;

namespace Zbang.Zbox.Domain.Services
{
    public class ZboxCacheServiceDecorator : IZboxCacheService
    {
        private readonly IWithCache m_WithCache;
        private readonly IZboxService m_ZboxService;

        public ZboxCacheServiceDecorator(IWithCache withCache, IZboxService zboxService)
        {
            m_WithCache = withCache;
            m_ZboxService = zboxService;
        }
        public BoxDto GetBox(GetBoxQuery query)
        {
            return m_ZboxService.GetBox(query);
        }

        public IEnumerable<UserPublicDto> GetBoxUsersWithoutInvites(GetBoxUsersQuery query)
        {
            return m_ZboxService.GetBoxUsersWithoutInvites(query);
        }

        public IList<ItemDto> GetBoxItemsPaged(GetBoxItemsPagedQuery query)
        {
            //var result = m_WithCache.Query<GetBoxItemsPagedQuery, IList<ItemDto>>(m_ZboxService.GetBoxItemsPaged, query);
            //if (result.Any(w => string.IsNullOrWhiteSpace(w.Uid)))
            //{
            //    m_WithCache.ClearCacheByKey(query.GetCacheKey());
            return m_ZboxService.GetBoxItemsPaged(query);
            //}
            //return result;
        }

        public IEnumerable<UserPublicDto> GetBoxUsers(GetBoxUsersQuery query)
        {
            return m_ZboxService.GetBoxUsers(query);
        }

        public IEnumerable<CommentDto> GetBoxComments(GetBoxCommentsQuery query)
        {
            var result = m_WithCache.Query(m_ZboxService.GetBoxComments, query);
            if (result.Any(w => w.Id == 0))
            {
                m_WithCache.ClearCacheByKey(query.GetCacheKey());
                result = m_ZboxService.GetBoxComments(query);
            }
            return result;

        }

        public IEnumerable<CommentDto> GetItemComments(GetItemCommentsQuery query)
        {
            return m_ZboxService.GetItemComments(query);
        }

        public ItemDto GetBoxItem(GetItemQuery query)
        {
            return m_ZboxService.GetBoxItem(query);
        }

        public IEnumerable<ISearchable> Search(SearchQueryBase query)
        {
            return m_ZboxService.Search(query);
        }

        public UserPermissionPerBoxDto GetUserPermission(GetBoxQuery query)
        {
            return m_ZboxService.GetUserPermission(query);
        }

        public BoxSettingsDto GetBoxSettingsData(GetBoxQuery query)
        {
            return m_ZboxService.GetBoxSettingsData(query);
        }

        public bool GetBoxNameExists(GetBoxNameExistsQuery query)
        {
            return m_ZboxService.GetBoxNameExists(query);
        }

        public long GetUserDetailsByMembershipId(GetUserByMembershipQuery query)
        {
            return m_ZboxService.GetUserDetailsByMembershipId(query);
        }

        public IEnumerable<TextDto> GetRecentActivities(GetRecentActivityQuery query)
        {
            return m_ZboxService.GetRecentActivities(query);
        }

        public UserDto GetUserDetails(GetUserDetailsQuery query)
        {
            return m_ZboxService.GetUserDetails(query);
        }

        public IEnumerable<FriendDto> GetUserFriends(GetUserFriendsQuery query)
        {
            return m_ZboxService.GetUserFriends(query);
        }

        public IList<UserBoxesDto> GetUserBoxes(GetBoxesQuery query)
        {
            return m_ZboxService.GetUserBoxes(query);
        }

        public BoxDataDto GetBoxDataForDigestEmail(GetBoxDataForDigestEmailQuery query)
        {
            return m_ZboxService.GetBoxDataForDigestEmail(query);
        }

        public IEnumerable<UserMailDataDto> GetBoxSubscribersToMail(GetBoxSubscribers query)
        {
            return m_ZboxService.GetBoxSubscribersToMail(query);
        }

        public IEnumerable<UserDto> GetUserListForDigestEmail(GetUserDigestQuery query)
        {
            return m_ZboxService.GetUserListForDigestEmail(query);
        }

        public IEnumerable<BoxIdDto> GetBoxIdList(GetBoxesDigestQuery query)
        {
            return m_ZboxService.GetBoxIdList(query);
        }

        public IEnumerable<TagDto> GetTags(GetTagQuery query)
        {
            return m_ZboxService.GetTags(query);
        }

        public IEnumerable<TagCheckBoxDto> GetBoxTags(GetBoxTagsQuery query)
        {
            return m_ZboxService.GetBoxTags(query);
        }

        public BoxUpdates GetBoxUpdate(GetBoxUpdatesQuery query)
        {
            return m_ZboxService.GetBoxUpdate(query);
        }

        public CreateUserCommandResult CreateUser(CreateUserCommand command)
        {
            return m_ZboxService.CreateUser(command);
        }

        public UpdateUserCommandResult UpdateUserPassword(UpdateUserPasswordCommand command)
        {
            return m_ZboxService.UpdateUserPassword(command);
        }

        public UpdateUserCommandResult UpdateUserEmail(UpdateUserEmailCommand command)
        {
            return m_ZboxService.UpdateUserEmail(command);
        }

        public UpdateUserCommandResult UpdateUserProfile(UpdateUserProfileCommand command)
        {
            return m_ZboxService.UpdateUserProfile(command);
        }

        public CreateBoxCommandResult CreateBox(CreateBoxCommand command)
        {
            return m_ZboxService.CreateBox(command);
        }

        public ChangeBoxNameCommandResult ChangeBoxName(ChangeBoxNameCommand command)
        {
            return m_ZboxService.ChangeBoxName(command);
        }

        public ChangeBoxPrivacySettingsCommandResult ChangeBoxPrivacySettings(ChangeBoxPrivacySettingsCommand command)
        {
            return m_ZboxService.ChangeBoxPrivacySettings(command);
        }

        public DeleteBoxCommandResult DeleteBox(DeleteBoxCommand command)
        {
            return m_ZboxService.DeleteBox(command);
        }

        public AddFileToBoxCommandResult AddFileToBox(AddFileToBoxCommand command)
        {
            return m_WithCache.Command(m_ZboxService.AddFileToBox, command);
        }

        public AddLinkToBoxCommandResult AddLinkToBox(AddLinkToBoxCommand command)
        {
            return m_WithCache.Command(m_ZboxService.AddLinkToBox, command);
        }

        public AddBoxCommentCommandResult AddBoxComment(AddBoxCommentCommand command)
        {
            return m_WithCache.Command(m_ZboxService.AddBoxComment, command);
        }

        public AddReplyToCommentCommandResult AddReplyToComment(AddReplyToCommentCommand command)
        {
            return m_WithCache.Command(m_ZboxService.AddReplyToComment, command);
        }

        public DeleteItemCommandResult DeleteItem(DeleteItemCommand command)
        {
            return m_WithCache.Command(m_ZboxService.DeleteItem, command);
        }

        public DeleteUserFromBoxCommandResult DeleteUserFromBox(DeleteUserFromBoxCommand command)
        {
            return m_ZboxService.DeleteUserFromBox(command);
        }

        public ChangeNotificationSettingsCommandResult ChangeNotificationSettings(ChangeNotificationSettingsCommand command)
        {
            return m_ZboxService.ChangeNotificationSettings(command);
        }

        public ShareBoxCommandResult ShareBox(ShareBoxCommand command)
        {
            return m_ZboxService.ShareBox(command);
        }

        public SubscribeToSharedBoxCommandResult SubscribeToSharedBox(SubscribeToSharedBoxCommand command)
        {
            return m_ZboxService.SubscribeToSharedBox(command);
        }

        public VerifyEmailCommandResult VerifyEmail(VerifyEmailCommand command)
        {
            return m_ZboxService.VerifyEmail(command);
        }

        public DeleteCommentCommandResult DeleteComment(DeleteCommentCommand command)
        {
            return m_WithCache.Command(m_ZboxService.DeleteComment, command);
        }

        public AddItemCommentCommandResult AddItemComment(AddItemCommentCommand command)
        {
            return m_WithCache.Command(m_ZboxService.AddItemComment, command);

        }

        public ChangeBoxFavoriteCommandResult ChangeBoxFavorite(ChangeBoxFavoriteCommand command)
        {
            return m_ZboxService.ChangeBoxFavorite(command);
        }

        public ChangeFileNameCommandResult ChangeFileName(ChangeFileNameCommand command)
        {
            return m_WithCache.Command(m_ZboxService.ChangeFileName, command);

        }

        public AddTagCommandResult AddTag(AddTagCommand command)
        {
            return m_ZboxService.AddTag(command);
        }

        public AssignTagsToBoxCommandResult AssignTagsToBox(AssignTagsToBoxCommand command)
        {
            return m_ZboxService.AssignTagsToBox(command);
        }

        public DeleteTagCommandResult DeleteTag(DeleteTagCommand command)
        {
            return m_ZboxService.DeleteTag(command);
        }

        public ChangeTagNameCommandResult ChangeTagName(ChangeTagNameCommand command)
        {
            return m_ZboxService.ChangeTagName(command);
        }

        public UpdateThumbnailCommandResult UpdateThumbnailPicture(UpdateThumbnailCommand command)
        {
            return m_WithCache.Command(m_ZboxService.UpdateThumbnailPicture, command);
        }





        public long GetItemIdByBlobId(string blobId)
        {
            return m_ZboxService.GetItemIdByBlobId(blobId);
        }


        public SendMessageToEmailCommandResult SendMessage(SendMessageToEmailCommand command)
        {
            return m_ZboxService.SendMessage(command);
        }


        public InviteBoxDataDto GetBoxDateForInvite(GetBoxInviteDataQuery query)
        {
            return m_ZboxService.GetBoxDateForInvite(query);
        }


        public BoxDataForImeediateEmail GetBoxDataForItemAdd(GetBoxDataForImmediateEmailQuery query)
        {
            return m_ZboxService.GetBoxDataForItemAdd(query);
            
        }
    }
}
