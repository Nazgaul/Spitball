using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.ViewModel.DTOs.Qna;
using Zbang.Zbox.ViewModel.DTOs.UserDtos;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;

namespace Zbang.Zbox.ReadServices
{
    public class ZboxCacheReadService : IZboxCacheReadService
    {
        private readonly IZboxReadService m_ReadService;
        private readonly IWithCache m_Cache;
        public ZboxCacheReadService(IZboxReadService readService, IWithCache cache)
        {
            m_ReadService = readService;
            m_Cache = cache;
        }


        public Task<ViewModel.DTOs.Dashboard.DashboardDto> GetDashboard(ViewModel.Queries.Boxes.GetBoxesQuery query)
        {
            return m_ReadService.GetDashboard(query);
        }

        public Task<ViewModel.DTOs.Dashboard.UniversityDashboardInfoDto> GetMyData(ViewModel.Queries.User.GetDashboardQuery query)
        {
            return m_ReadService.GetMyData(query);
        }

        public ViewModel.DTOs.Library.NodeBoxesDto GetLibraryNode(ViewModel.Queries.Library.GetLibraryNodeQuery query)
        {
            return m_ReadService.GetLibraryNode(query);
        }

        public Task<ViewModel.DTOs.Library.UniversityInfoDto> GetUniversityDetail(ViewModel.Queries.Library.GetUniversityDetailQuery query)
        {
            return m_Cache.QueryAsync<ViewModel.Queries.Library.GetUniversityDetailQuery, ViewModel.DTOs.Library.UniversityInfoDto>
                (m_ReadService.GetUniversityDetail, query);
        }

        public Task<IEnumerable<ViewModel.DTOs.InviteDto>> GetInvites(ViewModel.Queries.GetInvitesQuery query)
        {
            return m_ReadService.GetInvites(query);
        }

        public IEnumerable<ViewModel.DTOs.ItemDtos.IItemDto> GetBoxItemsPaged2(ViewModel.Queries.GetBoxItemsPagedQuery query)
        {
            return m_ReadService.GetBoxItemsPaged2(query);
        }

        public ViewModel.DTOs.ItemDtos.ItemWithDetailDto GetItem(ViewModel.Queries.GetItemQuery query)
        {
            return m_ReadService.GetItem(query);
        }

        public Task<ViewModel.DTOs.ItemRateDto> GetRate(ViewModel.Queries.GetItemRateQuery query)
        {
            return m_ReadService.GetRate(query);
        }

        public IEnumerable<ViewModel.DTOs.ActivityDtos.BaseActivityDto> GetBoxComments(ViewModel.Queries.GetBoxCommentsQuery query)
        {
            return m_ReadService.GetBoxComments(query);
        }

        public Task<IEnumerable<ViewModel.DTOs.ActivityDtos.AnnotationDto>> GetItemComments(ViewModel.Queries.GetItemCommentsQuery query)
        {
            return m_ReadService.GetItemComments(query);
        }

        public ViewModel.DTOs.BoxDtos.BoxDto GetBox2(ViewModel.Queries.GetBoxQuery query)
        {
            return m_ReadService.GetBox2(query);
        }

        public Task<ViewModel.DTOs.Search.SearchDto> Search(ViewModel.Queries.Search.GroupSearchQuery query)
        {
            return m_ReadService.Search(query);
        }

        public Task<IEnumerable<ViewModel.DTOs.UserDtos.UserDto>> GetUserFriends(ViewModel.Queries.GetUserFriendsQuery query)
        {
            return m_Cache.QueryAsync<GetUserFriendsQuery, IEnumerable<ViewModel.DTOs.UserDtos.UserDto>>
                (m_ReadService.GetUserFriends, query);
            //return m_ReadService.GetUserFriends(query);
        }

        public ViewModel.DTOs.UserDtos.UserDetailDto GetUserData(ViewModel.Queries.GetUserDetailsQuery query)
        {
            return m_ReadService.GetUserData(query);
        }

        public ViewModel.DTOs.UserDtos.UserAccountDto GetUserAccountDetails(ViewModel.Queries.GetUserDetailsQuery query)
        {
            return m_ReadService.GetUserAccountDetails(query);
        }

        public IEnumerable<ViewModel.DTOs.BoxDtos.BoxNotificationDto> GetUserBoxesNotification(ViewModel.Queries.GetUserDetailsQuery query)
        {
            return m_ReadService.GetUserBoxesNotification(query);
        }

        public Infrastructure.Enums.NotificationSettings GetUserBoxNotificationSettings(ViewModel.Queries.GetBoxQuery query)
        {
            return m_ReadService.GetUserBoxNotificationSettings(query);
        }

        public Task<IEnumerable<ViewModel.DTOs.Library.UniversityByPrefixDto>> GetUniversityListByPrefix(ViewModel.Queries.Library.GetUniversityByPrefixQuery query)
        {
            return m_Cache.QueryAsync<ViewModel.Queries.Library.GetUniversityByPrefixQuery, IEnumerable<ViewModel.DTOs.Library.UniversityByPrefixDto>>
                (m_ReadService.GetUniversityListByPrefix, query);
            //return m_ReadService.GetUniversityListByPrefix(query);
        }

        public ViewModel.DTOs.BoxSettingsDto GetBoxSetting(ViewModel.Queries.GetBoxQuery query)
        {
            return m_ReadService.GetBoxSetting(query);
        }

        public IEnumerable<ViewModel.DTOs.UserDtos.UserMemberDto> GetBoxMembers(ViewModel.Queries.GetBoxQuery query)
        {
            return m_ReadService.GetBoxMembers(query);
        }

        public Task<ViewModel.DTOs.UserDtos.UserMinProfile> GetUserMinProfile(ViewModel.Queries.GetUserMinProfileQuery query)
        {
            return m_Cache.QueryAsync<ViewModel.Queries.GetUserMinProfileQuery, ViewModel.DTOs.UserDtos.UserMinProfile>
               (m_ReadService.GetUserMinProfile, query);
            //return m_ReadService.GetUserMinProfile(query);
        }

        public string GetLocationByIP(long ipNumber)
        {
            return m_ReadService.GetLocationByIP(ipNumber);
        }

        public IEnumerable<ViewModel.DTOs.Qna.QuestionDto> GetQuestions(ViewModel.Queries.QnA.GetBoxQuestionsQuery query)
        {
            return m_ReadService.GetQuestions(query);
        }

        public Task<bool> GetInvite(ViewModel.Queries.GetInviteDetailQuery query)
        {
            return m_ReadService.GetInvite(query);
        }

        public ViewModel.DTOs.BoxDtos.BoxMetaDto GetBoxMeta(ViewModel.Queries.GetBoxQuery query)
        {
            return m_ReadService.GetBoxMeta(query);
        }

        public ViewModel.DTOs.UserDtos.LogInUserDto GetUserDetailsByMembershipId(ViewModel.Queries.GetUserByMembershipQuery query)
        {
            return m_ReadService.GetUserDetailsByMembershipId(query);
        }

        public ViewModel.DTOs.UserDtos.LogInUserDto GetUserDetailsByFacebookId(ViewModel.Queries.GetUserByFacebookQuery query)
        {
            return m_ReadService.GetUserDetailsByFacebookId(query);
        }

        public ViewModel.DTOs.UserDtos.LogInUserDto GetUserDetailsByEmail(ViewModel.Queries.GetUserByEmailQuery query)
        {
            return m_ReadService.GetUserDetailsByEmail(query);
        }

        public long GetItemIdByBlobId(string blobId)
        {
            return m_ReadService.GetItemIdByBlobId(blobId);
        }


        public Task<IEnumerable<ViewModel.DTOs.BoxDtos.BoxToFriendDto>> GetUserWithFriendBoxes(ViewModel.Queries.Boxes.GetUserWithFriendQuery query)
        {
            return m_ReadService.GetUserWithFriendBoxes(query);
        }

        public Task<IEnumerable<ViewModel.DTOs.ItemDtos.ItemToFriendDto>> GetUserWithFriendFiles(ViewModel.Queries.Boxes.GetUserWithFriendQuery query)
        {
            return m_ReadService.GetUserWithFriendFiles(query);
        }


        public Task<IEnumerable<QuestionToFriendDto>> GetUserWithFriendQuestion(GetUserWithFriendQuery query)
        {
            return m_ReadService.GetUserWithFriendQuestion(query);
        }
        public Task<IEnumerable<AnswerToFriendDto>> GetUserWithFriendAnswer(GetUserWithFriendQuery query)
        {
            return m_ReadService.GetUserWithFriendAnswer(query);
        }
        public Task<IEnumerable<UserInviteDto>> GetUserPersonalInvites(GetInvitesQuery query)
        {
            return m_ReadService.GetUserPersonalInvites(query);
        }
        public Task<UserToFriendActivity> GetUserWithFriendActivity(GetUserWithFriendQuery query)
        {
            return m_ReadService.GetUserWithFriendActivity(query);
        }


        public Task<ViewModel.DTOs.SeoDto> GetSeoBoxesAndItems()
        {
            return m_ReadService.GetSeoBoxesAndItems();
        }


        public Task<IEnumerable<AdminUserDto>> GetUniversityUsers(ViewModel.Queries.Library.GetAdminUsersQuery query)
        {
            return m_ReadService.GetUniversityUsers(query);
        }


        public Task<IEnumerable<ViewModel.DTOs.Library.DepartmentDto>> GetDepartmentList(long universityId)
        {
            return m_ReadService.GetDepartmentList(universityId);
        }


        public Task<IEnumerable<ViewModel.DTOs.Search.SearchItems>> OtherUniversities(ViewModel.Queries.Search.GroupSearchQuery query)
        {
            return m_ReadService.OtherUniversities(query);
        }


        public Task<IEnumerable<ViewModel.DTOs.UpdatesDto>> GetUpdates(QueryBase query)
        {
            return m_ReadService.GetUpdates(query);
        }


        public Task<ViewModel.DTOs.ItemDtos.QuizWithDetailSolvedDto> GetQuiz(GetQuizQuery query)
        {
            return m_ReadService.GetQuiz(query);
        }


        public Task<ViewModel.DTOs.ItemDtos.QuizWithDetailDto> GetDraftQuiz(GetQuizDraftQuery query)
        {
            return m_ReadService.GetDraftQuiz(query);
        }
    }
}
