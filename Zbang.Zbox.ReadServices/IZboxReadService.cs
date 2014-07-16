using System.Collections.Generic;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.DTOs.Dashboard;
using Zbang.Zbox.ViewModel.DTOs.Library;
using Zbang.Zbox.ViewModel.DTOs.Qna;
using Zbang.Zbox.ViewModel.DTOs.Search;
using Zbang.Zbox.ViewModel.DTOs.Store;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;
using Zbang.Zbox.ViewModel.Queries.Library;
using Zbang.Zbox.ViewModel.Queries.QnA;
using Zbang.Zbox.ViewModel.Queries.Search;
using Zbang.Zbox.ViewModel.Queries.User;
using Activity = Zbang.Zbox.ViewModel.DTOs.ActivityDtos;
using Box = Zbang.Zbox.ViewModel.DTOs.BoxDtos;
using Item = Zbang.Zbox.ViewModel.DTOs.ItemDtos;
using User = Zbang.Zbox.ViewModel.DTOs.UserDtos;

namespace Zbang.Zbox.ReadServices
{
    public interface IZboxReadService : IBaseReadService
    {
        //PagedDto2<BoxDto> GetBoxes(GetBoxesQuery query);
        //Task<IEnumerable<BoxDto>> GetBoxes(GetBoxesQuery query);
        Task<DashboardDto> GetDashboard(GetBoxesQuery query);
       

        Task<UniversityDashboardInfoDto> GetMyData(GetDashboardQuery query);

        NodeBoxesDto GetLibraryNode(GetLibraryNodeQuery query);
        Task<UniversityInfoDto> GetUniversityDetail(GetUniversityDetailQuery query);
        Task<IEnumerable<InviteDto>> GetInvites(GetInvitesQuery query);

        //PagedDto<ItemDto> GetBoxItemsPaged(GetBoxItemsPagedQuery query);
        IEnumerable<Item.IItemDto> GetBoxItemsPaged2(GetBoxItemsPagedQuery query);
        Item.ItemWithDetailDto GetItem(GetItemQuery query);
        Task<ItemRateDto> GetRate(GetItemRateQuery query);

        IEnumerable<Activity.BaseActivityDto> GetBoxComments(GetBoxCommentsQuery query);
        Task<IEnumerable<Activity.AnnotationDto>> GetItemComments(GetItemCommentsQuery query);

        Box.BoxDto GetBox(GetBoxQuery query);

        Task<SearchDto> Search(GroupSearchQuery query);
        Task<IEnumerable<User.UserDto>> GetUserFriends(GetUserFriendsQuery query);

        User.UserDetailDto GetUserData(GetUserDetailsQuery query);
        User.UserAccountDto GetUserAccountDetails(GetUserDetailsQuery query);
        IEnumerable<Box.BoxNotificationDto> GetUserBoxesNotification(GetUserDetailsQuery query);
        NotificationSettings GetUserBoxNotificationSettings(GetBoxQuery query);

        //Task<IEnumerable<UniversityByPrefixDto>> GetUniversityListByPrefix(GetUniversityByPrefixQuery query);
        //IEnumerable<string> GetUniversityByPrefix(GetUniversityQuery query);
        

        BoxSettingsDto GetBoxSetting(GetBoxQuery query);
        IEnumerable<User.UserMemberDto> GetBoxMembers(GetBoxQuery query);

        Task<User.UserMinProfile> GetUserMinProfile(GetUserMinProfileQuery query);

        string GetLocationByIp(long ipNumber);
        Task<IEnumerable<UniversityByFriendDto>> GetUniversityListByFriendsIds(IEnumerable<long> friendsIds);


        IEnumerable<QuestionDto> GetQuestions(GetBoxQuestionsQuery query);
        Task<bool> GetInvite(GetInviteDetailQuery query);

        Box.BoxMetaDto GetBoxMeta(GetBoxQuery query);


        //user page
        Task<IEnumerable<Box.BoxToFriendDto>> GetUserWithFriendBoxes(GetUserWithFriendQuery query);
        Task<IEnumerable<Item.ItemToFriendDto>> GetUserWithFriendFiles(GetUserWithFriendQuery query);
        Task<IEnumerable<QuestionToFriendDto>> GetUserWithFriendQuestion(GetUserWithFriendQuery query);
        Task<IEnumerable<AnswerToFriendDto>> GetUserWithFriendAnswer(GetUserWithFriendQuery query);
        Task<IEnumerable<User.UserInviteDto>> GetUserPersonalInvites(GetInvitesQuery query);


        Task<User.UserToFriendActivity> GetUserWithFriendActivity(GetUserWithFriendQuery query);

        Task<IEnumerable<string>> GetSeoItems(int page);
        Task<int> GetSeoItemCount();

        Task<IEnumerable<ViewModel.DTOs.UserDtos.AdminUserDto>> GetUniversityUsers(GetAdminUsersQuery query);

        Task<IEnumerable<DepartmentDto>> GetDepartmentList(long universityId);
        Task<bool> GetUniversityNeedId(long universityId);
        Task<bool> GetUniversityNeedCode(long universityId);

        Task<IEnumerable<SearchItems>> OtherUniversities(GroupSearchQuery query);
        Task<IEnumerable<UpdatesDto>> GetUpdates(QueryBase query);

        //Quiz
        Task<Item.QuizWithDetailSolvedDto> GetQuiz(GetQuizQuery query);
        Task<Item.QuizWithDetailDto> GetDraftQuiz(GetQuizDraftQuery query);
        Task<IEnumerable<Item.DiscussionDto>> GetDiscussion(GetDisscussionQuery query);

        Task<IEnumerable<Product>> GetProducts();
    }
}
