﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.DTOs.ActivityDtos;
using Zbang.Zbox.ViewModel.DTOs.BoxDtos;
using Zbang.Zbox.ViewModel.DTOs.Dashboard;
using Zbang.Zbox.ViewModel.DTOs.ItemDtos;
using Zbang.Zbox.ViewModel.DTOs.Library;
using Zbang.Zbox.ViewModel.DTOs.Qna;
using Zbang.Zbox.ViewModel.DTOs.Search;
using Zbang.Zbox.ViewModel.DTOs.UserDtos;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;
using Zbang.Zbox.ViewModel.Queries.Library;
using Zbang.Zbox.ViewModel.Queries.QnA;
using Zbang.Zbox.ViewModel.Queries.Search;
using Zbang.Zbox.ViewModel.Queries.User;
using BoxDto = Zbang.Zbox.ViewModel.DTOs.BoxDtos.BoxDto;
using UserDto = Zbang.Zbox.ViewModel.DTOs.UserDtos.UserDto;

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


        public Task<DashboardDto> GetDashboard(GetBoxesQuery query)
        {
            return m_ReadService.GetDashboard(query);
        }

        public Task<UniversityDashboardInfoDto> GetMyData(GetDashboardQuery query)
        {
            return m_ReadService.GetMyData(query);
        }

        public NodeBoxesDto GetLibraryNode(GetLibraryNodeQuery query)
        {
            return m_ReadService.GetLibraryNode(query);
        }

        public Task<UniversityInfoDto> GetUniversityDetail(GetUniversityDetailQuery query)
        {
            return m_Cache.QueryAsync
                (m_ReadService.GetUniversityDetail, query);
        }

        public Task<IEnumerable<InviteDto>> GetInvites(GetInvitesQuery query)
        {
            return m_ReadService.GetInvites(query);
        }

        public IEnumerable<IItemDto> GetBoxItemsPaged2(GetBoxItemsPagedQuery query)
        {
            return m_ReadService.GetBoxItemsPaged2(query);
        }

        public ItemWithDetailDto GetItem(GetItemQuery query)
        {
            return m_ReadService.GetItem(query);
        }

        public Task<ItemRateDto> GetRate(GetItemRateQuery query)
        {
            return m_ReadService.GetRate(query);
        }

        public IEnumerable<BaseActivityDto> GetBoxComments(GetBoxCommentsQuery query)
        {
            return m_ReadService.GetBoxComments(query);
        }

        public Task<IEnumerable<AnnotationDto>> GetItemComments(GetItemCommentsQuery query)
        {
            return m_ReadService.GetItemComments(query);
        }

        public BoxDto GetBox2(GetBoxQuery query)
        {
            return m_ReadService.GetBox2(query);
        }

        public Task<SearchDto> Search(GroupSearchQuery query)
        {
            return m_ReadService.Search(query);
        }

        public Task<IEnumerable<UserDto>> GetUserFriends(GetUserFriendsQuery query)
        {
            return m_Cache.QueryAsync
                (m_ReadService.GetUserFriends, query);
            //return m_ReadService.GetUserFriends(query);
        }

        public UserDetailDto GetUserData(GetUserDetailsQuery query)
        {
            return m_ReadService.GetUserData(query);
        }

        public UserAccountDto GetUserAccountDetails(GetUserDetailsQuery query)
        {
            return m_ReadService.GetUserAccountDetails(query);
        }

        public IEnumerable<BoxNotificationDto> GetUserBoxesNotification(GetUserDetailsQuery query)
        {
            return m_ReadService.GetUserBoxesNotification(query);
        }

        public NotificationSettings GetUserBoxNotificationSettings(GetBoxQuery query)
        {
            return m_ReadService.GetUserBoxNotificationSettings(query);
        }

        //public Task<IEnumerable<ViewModel.DTOs.Library.UniversityByPrefixDto>> GetUniversityListByPrefix(ViewModel.Queries.Library.GetUniversityByPrefixQuery query)
        //{
        //    return m_Cache.QueryAsync<ViewModel.Queries.Library.GetUniversityByPrefixQuery, IEnumerable<ViewModel.DTOs.Library.UniversityByPrefixDto>>
        //        (m_ReadService.GetUniversityListByPrefix, query);
        //    //return m_ReadService.GetUniversityListByPrefix(query);
        //}

        public BoxSettingsDto GetBoxSetting(GetBoxQuery query)
        {
            return m_ReadService.GetBoxSetting(query);
        }

        public IEnumerable<UserMemberDto> GetBoxMembers(GetBoxQuery query)
        {
            return m_ReadService.GetBoxMembers(query);
        }

        public Task<UserMinProfile> GetUserMinProfile(GetUserMinProfileQuery query)
        {
            return m_Cache.QueryAsync
               (m_ReadService.GetUserMinProfile, query);
            //return m_ReadService.GetUserMinProfile(query);
        }

        public string GetLocationByIp(long ipNumber)
        {
            return m_ReadService.GetLocationByIp(ipNumber);
        }

        public IEnumerable<QuestionDto> GetQuestions(GetBoxQuestionsQuery query)
        {
            return m_ReadService.GetQuestions(query);
        }

        public Task<bool> GetInvite(GetInviteDetailQuery query)
        {
            return m_ReadService.GetInvite(query);
        }

        public BoxMetaDto GetBoxMeta(GetBoxQuery query)
        {
            return m_ReadService.GetBoxMeta(query);
        }

        public LogInUserDto GetUserDetailsByMembershipId(GetUserByMembershipQuery query)
        {
            return m_ReadService.GetUserDetailsByMembershipId(query);
        }

        public LogInUserDto GetUserDetailsByFacebookId(GetUserByFacebookQuery query)
        {
            return m_ReadService.GetUserDetailsByFacebookId(query);
        }

        public LogInUserDto GetUserDetailsByEmail(GetUserByEmailQuery query)
        {
            return m_ReadService.GetUserDetailsByEmail(query);
        }

        public long GetItemIdByBlobId(string blobId)
        {
            return m_ReadService.GetItemIdByBlobId(blobId);
        }


        public Task<IEnumerable<BoxToFriendDto>> GetUserWithFriendBoxes(GetUserWithFriendQuery query)
        {
            return m_ReadService.GetUserWithFriendBoxes(query);
        }

        public Task<IEnumerable<ItemToFriendDto>> GetUserWithFriendFiles(GetUserWithFriendQuery query)
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




        public Task<IEnumerable<AdminUserDto>> GetUniversityUsers(GetAdminUsersQuery query)
        {
            return m_ReadService.GetUniversityUsers(query);
        }


        public Task<IEnumerable<DepartmentDto>> GetDepartmentList(long universityId)
        {
            return m_ReadService.GetDepartmentList(universityId);
        }


        public Task<IEnumerable<SearchItems>> OtherUniversities(GroupSearchQuery query)
        {
            return m_ReadService.OtherUniversities(query);
        }


        public Task<IEnumerable<UpdatesDto>> GetUpdates(QueryBase query)
        {
            return m_ReadService.GetUpdates(query);
        }


        public Task<QuizWithDetailSolvedDto> GetQuiz(GetQuizQuery query)
        {
            return m_ReadService.GetQuiz(query);
        }


        public Task<QuizWithDetailDto> GetDraftQuiz(GetQuizDraftQuery query)
        {
            return m_ReadService.GetDraftQuiz(query);
        }


        public Task<IEnumerable<DiscussionDto>> GetDiscussion(GetDisscussionQuery query)
        {
            return m_ReadService.GetDiscussion(query);
        }


        public Task<bool> GetUniversityNeedId(long universityId)
        {
            return m_ReadService.GetUniversityNeedId(universityId);
        }

        public Task<bool> GetUniversityNeedCode(long universityId)
        {
            return m_ReadService.GetUniversityNeedCode(universityId);
        }


        public Task<IEnumerable<UniversityByFriendDto>> GetUniversityListByFriendsIds(IEnumerable<long> friendsIds)
        {
            return m_ReadService.GetUniversityListByFriendsIds(friendsIds);
        }


        public Task<IEnumerable<string>> GetSeoItems(int page)
        {
            return m_ReadService.GetSeoItems(page);
        }

        public Task<int> GetSeoItemCount()
        {
            return m_ReadService.GetSeoItemCount();
        }
    }
}
