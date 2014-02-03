using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Zbox.Domain.Common
{
    public class ZboxServiceAgent : ClientBase<IZboxService>, IZboxService
    {
        public ViewModel.DTOs.BoxUserDto GetBox(ViewModel.Queries.GetBoxQuery query)
        {
            return this.Channel.GetBox(query);
        }



        public IList<ViewModel.DTOs.UserBoxesDto> GetUserBoxes(ViewModel.Queries.GetBoxesQuery query)
        {
            return this.Channel.GetUserBoxes(query);
        }

        public ViewModel.DTOs.UserPermissionPerBoxDto GetUserPermission(ViewModel.Queries.GetBoxQuery query)
        {
            return this.Channel.GetUserPermission(query);
        }



        public IEnumerable<ViewModel.DTOs.FriendDto> GetUserFriends(ViewModel.Queries.GetUserFriendsQuery query)
        {
            return this.Channel.GetUserFriends(query);
        }

        public IEnumerable<ViewModel.DTOs.UserDto> GetBoxUsers(ViewModel.Queries.GetBoxUsersQuery query)
        {
            return this.Channel.GetBoxUsers(query);
        }

        public IEnumerable<TextDto> GetRecentActivities(GetRecentActivityQuery query)
        {
            return this.Channel.GetRecentActivities(query);
        }

        public IEnumerable<ViewModel.DTOs.CommentDto> GetBoxComments(ViewModel.Queries.GetBoxCommentsQuery query)
        {
            return this.Channel.GetBoxComments(query);
        }

        public IEnumerable<ViewModel.DTOs.CommentDto> GetItemComments(ViewModel.Queries.GetItemCommentsQuery query)
        {
            return this.Channel.GetItemComments(query);
        }

        public Commands.CreateBoxCommandResult CreateBox(Commands.CreateBoxCommand command)
        {
            return this.Channel.CreateBox(command);
        }

        public Commands.ChangeBoxNameCommandResult ChangeBoxName(Commands.ChangeBoxNameCommand command)
        {
            return this.Channel.ChangeBoxName(command);
        }

        public Commands.ChangeBoxPrivacySettingsCommandResult ChangeBoxPrivacySettings(Commands.ChangeBoxPrivacySettingsCommand command)
        {
            return this.Channel.ChangeBoxPrivacySettings(command);
        }

        public Commands.AddFileToBoxCommandResult AddFileToBox(Commands.AddFileToBoxCommand command)
        {
            return this.Channel.AddFileToBox(command);
        }

        public Commands.AddLinkToBoxCommandResult AddLinkToBox(Commands.AddLinkToBoxCommand command)
        {
            return this.Channel.AddLinkToBox(command);
        }

        public Commands.AddBoxCommentCommandResult AddBoxComment(Commands.AddBoxCommentCommand command)
        {
            return this.Channel.AddBoxComment(command);
        }

        public Commands.AddItemCommentCommandResult AddItemComment(Commands.AddItemCommentCommand command)
        {
            return this.Channel.AddItemComment(command);
        }

        public Commands.AddReplyCommandResult AddReply(Commands.AddReplyCommand command)
        {
            return this.Channel.AddReply(command);
        }

        public Commands.CreateUserCommandResult CreateUser(Commands.CreateUserCommand command)
        {
            return this.Channel.CreateUser(command);
        }

        public Commands.UpdateUserCommandResult UpdateUser(Commands.UpdateUserCommand command)
        {
            return this.Channel.UpdateUser(command);
        }

        public Commands.DeleteBoxCommandResult DeleteBox(Commands.DeleteBoxCommand command)
        {
            return this.Channel.DeleteBox(command);
        }

        public IList<ViewModel.DTOs.ItemDto> GetBoxItemsPaged(ViewModel.Queries.GetBoxItemsPagedQuery query)
        {
            return this.Channel.GetBoxItemsPaged(query);
        }

        public ViewModel.DTOs.ItemDto GetBoxItem(ViewModel.Queries.GetItemQuery query)
        {
            return this.Channel.GetBoxItem(query);
        }

       

        public List<ItemDto> SearchBoxItem(ViewModel.Queries.SearchBoxItemQuery query, params ViewModel.Queries.SearchBoxItemQuery[] moreQueries)
        {
            return this.Channel.SearchBoxItem(query, moreQueries);
        }

        public Commands.DeleteItemCommandResult DeleteItem(Commands.DeleteItemCommand command)
        {
            return this.Channel.DeleteItem(command);
        }

        public Commands.ShareBoxCommandResult ShareBox(Commands.ShareBoxCommand command)
        {
            return this.Channel.ShareBox(command);
        }

        public Commands.SubscribeToSharedBoxCommandResult SubscribeToSharedBox(Commands.SubscribeToSharedBoxCommand command)
        {
            return this.Channel.SubscribeToSharedBox(command);
        }

        public Commands.VerifyEmailCommandResult VerifyEmail(Commands.VerifyEmailCommand command)
        {
            return this.Channel.VerifyEmail(command);
        }

        public UserDto GetUserDetails(ViewModel.Queries.GetUserDetailsQuery query)
        {
            return this.Channel.GetUserDetails(query);
        }

        public UserDto GetUserDetailsByMembershipId(ViewModel.Queries.GetUserByMembershipQuery query)
        {
            return this.Channel.GetUserDetailsByMembershipId(query);
        }

        public Commands.RequestSubscriptionCommandResult RequestSubscription(Commands.RequestSubscriptionCommand command)
        {
            return this.Channel.RequestSubscription(command);
        }

        public Commands.BoxAuthenticationCommandResult BoxAuthentication(Commands.BoxAuthenticationCommand command)
        {
            return this.Channel.BoxAuthentication(command);
        }

        public Commands.RegenerateBoxAccessTokenCommandResult RegenerateBoxAccessToken(Commands.RegenerateBoxAccessTokenCommand command)
        {
            return this.Channel.RegenerateBoxAccessToken(command);
        }

        public Commands.DeleteCommentCommandResult DeleteComment(Commands.DeleteCommentCommand command)
        {
            return this.Channel.DeleteComment(command);
        }

        public Commands.DeleteUserFromBoxCommandResult DeleteUserFromBox(Commands.DeleteUserFromBoxCommand command)
        {
            return this.Channel.DeleteUserFromBox(command);
        }

        public Commands.ChangeBoxUserRightsCommandResult ChangeBoxUserRights(ChangeBoxUserRightsCommand command)
        {
            return this.Channel.ChangeBoxUserRights(command);
        }

        public Commands.ChangeNotificationSettingsCommandResult ChangeNotificationSettings(ChangeNotificationSettingsCommand command)
        {
            return this.Channel.ChangeNotificationSettings(command);
        }

        public Commands.ChangeBoxFavoriteCommandResult ChangeBoxFavorite(ChangeBoxFavoriteCommand command)
        {
            return this.Channel.ChangeBoxFavorite(command);
        }

        public Commands.ChangeFileNameCommandResult ChangeFileName(ChangeFileNameCommand command)
        {
            return this.Channel.ChangeFileName(command);
        }

        

    }
}
