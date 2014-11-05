using System.Collections.Generic;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.ViewModel.Dto;
using Zbang.Zbox.ViewModel.Dto.BoxDtos;
using Zbang.Zbox.ViewModel.Dto.Dashboard;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Dto.Qna;
using Zbang.Zbox.ViewModel.Dto.Search;
using Zbang.Zbox.ViewModel.Dto.Store;
using Zbang.Zbox.ViewModel.Dto.UserDtos;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;
using Zbang.Zbox.ViewModel.Queries.Library;
using Zbang.Zbox.ViewModel.Queries.QnA;
using Zbang.Zbox.ViewModel.Queries.Search;
using Zbang.Zbox.ViewModel.Queries.User;
using BoxDto = Zbang.Zbox.ViewModel.Dto.BoxDtos.BoxDto;

namespace Zbang.Zbox.ReadServices
{
    public interface IZboxReadService : IBaseReadService
    {
        //PagedDto2<BoxDto> GetBoxes(GetBoxesQuery query);
        //Task<IEnumerable<BoxDto>> GetBoxes(GetBoxesQuery query);
        Task<DashboardDto> GetDashboard(GetBoxesQuery query);
       

        Task<UniversityDashboardInfoDto> GetMyData(GetDashboardQuery query);

        Task<NodeBoxesDto> GetLibraryNode(GetLibraryNodeQuery query);
        Task<UniversityInfoDto> GetUniversityDetail(GetUniversityDetailQuery query);
        Task<IEnumerable<InviteDto>> GetInvites(GetInvitesQuery query);

        //PagedDto<ItemDto> GetBoxItemsPaged(GetBoxItemsPagedQuery query);
        IEnumerable<IItemDto> GetBoxItemsPaged2(GetBoxItemsPagedQuery query);
        Task<IEnumerable<QuizDto>> GetBoxQuizes(GetBoxItemsPagedQuery query);

        Task<SideDto> GetBoxSideBar(GetBoxSideBarQuery query);
        
        ItemWithDetailDto GetItem(GetItemQuery query);

        Task<ItemDetailDto> GetItem2(GetItemQuery query);
        Task<FileSeo> GetItemSeo(GetFileSeoQuery query);
        Task<BoxSeoDto> GetBoxSeo(GetBoxSeoQuery query);

       // IEnumerable<BaseActivityDto> GetBoxComments(GetBoxCommentsQuery query);

        BoxDto GetBox(GetBoxQuery query);
        Task<BoxDto2> GetBox2(GetBoxQuery query);
        Task<IEnumerable<TabDto>> GetBoxTabs(GetBoxQuery query);

        Task<SearchDto> Search(GroupSearchQuery query);
        Task<IEnumerable<UserDto>> GetUserFriends(GetUserFriendsQuery query);

        UserDetailDto GetUserData(GetUserDetailsQuery query);
        UserAccountDto GetUserAccountDetails(GetUserDetailsQuery query);
        IEnumerable<BoxNotificationDto> GetUserBoxesNotification(GetUserDetailsQuery query);
        NotificationSettings GetUserBoxNotificationSettings(GetBoxQuery query);

        //Task<IEnumerable<UniversityByPrefixDto>> GetUniversityListByPrefix(GetUniversityByPrefixQuery query);
        //IEnumerable<string> GetUniversityByPrefix(GetUniversityQuery query);
        

        BoxSettingsDto GetBoxSetting(GetBoxQuery query);

        Task<IEnumerable<UserMemberDto>> GetBoxMembers(GetBoxQuery query);

        Task<UserMinProfile> GetUserMinProfile(GetUserMinProfileQuery query);

        string GetLocationByIp(long ipNumber);
        Task<IEnumerable<UniversityByFriendDto>> GetUniversityListByFriendsIds(IEnumerable<long> friendsIds);


        Task<IEnumerable<QuestionDto>> GetQuestions(GetBoxQuestionsQuery query);
        Task<bool> GetInvite(GetInviteDetailQuery query);



        //user page
        Task<IEnumerable<BoxToFriendDto>> GetUserWithFriendBoxes(GetUserWithFriendQuery query);
        Task<IEnumerable<ItemToFriendDto>> GetUserWithFriendFiles(GetUserWithFriendQuery query);
        Task<IEnumerable<QuestionToFriendDto>> GetUserWithFriendQuestion(GetUserWithFriendQuery query);
        Task<IEnumerable<AnswerToFriendDto>> GetUserWithFriendAnswer(GetUserWithFriendQuery query);
        Task<IEnumerable<UserInviteDto>> GetUserPersonalInvites(GetInvitesQuery query);


        Task<UserToFriendActivity> GetUserWithFriendActivity(GetUserWithFriendQuery query);

        Task<IEnumerable<string>> GetSeoItems(int page);
        Task<int> GetSeoItemCount();

        Task<IEnumerable<AdminUserDto>> GetUniversityUsers(GetAdminUsersQuery query);

        Task<IEnumerable<RussianDepartmentDto>> GetRussianDepartmentList(long universityId);



        Task<bool> GetUniversityNeedId(long universityId);
        Task<bool> GetUniversityNeedCode(long universityId);

        //Task<NodeDto> GetDepartmentByUser(QueryBase query);

        Task<IEnumerable<SearchItems>> OtherUniversities(GroupSearchQuery query);
        Task<IEnumerable<UpdatesDto>> GetUpdates(QueryBase query);

        //Quiz
        Task<QuizWithDetailSolvedDto> GetQuiz(GetQuizQuery query);
        Task<QuizSeo> GetQuizSeo(GetQuizSeoQuery query);
        Task<QuizWithDetailDto> GetDraftQuiz(GetQuizDraftQuery query);
        Task<IEnumerable<DiscussionDto>> GetDiscussion(GetDisscussionQuery query);
        Task<int> GetNumberOfSolvers(long quizId);

        Task<IEnumerable<ProductDto>> GetProducts(GetStoreProductsByCategoryQuery query);
        IEnumerable<CategoryDto> GetCategories();
        Task<ProductWithDetailDto> GetProduct(GetStoreProductQuery query);
        Task<ProductCheckOutDto> GetProductCheckOut(GetStoreProductQuery query);
        Task<IEnumerable<ProductDto>> SearchProducts(SearchProductQuery query);
        Task<IEnumerable<BannerDto>> GetBanners(int? universityId);
        Task<bool> ValidateCoupon(int coupon);
        Task<int?> CloudentsUniversityToStoreUniversity(long universityId);


        Task<IEnumerable<RecommendBoxDto>> GetRecommendedCourses(RecommendedCoursesQuery query);

       
    }
}
