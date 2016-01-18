﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.ViewModel.Dto;
using Zbang.Zbox.ViewModel.Dto.BoxDtos;
using Zbang.Zbox.ViewModel.Dto.Dashboard;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Dto.Qna;
using Zbang.Zbox.ViewModel.Dto.UserDtos;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;
using Zbang.Zbox.ViewModel.Queries.Dashboard;
using Zbang.Zbox.ViewModel.Queries.Library;
using Zbang.Zbox.ViewModel.Queries.QnA;
using UserDto = Zbang.Zbox.ViewModel.Dto.UserDtos.UserDto;

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

        [Obsolete]
        public Task<IEnumerable<BoxDto>> GetUserBoxesOld(GetBoxesQuery query)
        {
            return m_ReadService.GetUserBoxesOld(query);
        }

        public Task<IEnumerable<BoxDto>> GetUserBoxesAsync(GetBoxesQuery query)
        {
            return m_ReadService.GetUserBoxesAsync(query);
        }

        public Task<UniversityDashboardInfoDto> GetUniversityInfoAsync(UniversityQuery query)
        {
            return m_ReadService.GetUniversityInfoAsync(query);
        }

        public Task<NodeBoxesDto> GetLibraryNodeAsync(GetLibraryNodeQuery query)
        {
            return m_ReadService.GetLibraryNodeAsync(query);
        }

        public Task<UniversityInfoDto> GetUniversityDetailAsync(GetUniversityDetailQuery query)
        {
            return m_Cache.QueryAsync
                (m_ReadService.GetUniversityDetailAsync, query);
        }

        public Task<IEnumerable<InviteDto>> GetInvitesAsync(GetInvitesQuery query)
        {
            return m_ReadService.GetInvitesAsync(query);
        }

        public Task<IEnumerable<ViewModel.Dto.ItemDtos.ItemDto>> GetBoxItemsPagedAsync(GetBoxItemsPagedQuery query)
        {
            return m_ReadService.GetBoxItemsPagedAsync(query);
        }
        public Task<IEnumerable<ViewModel.Dto.ItemDtos.ItemDto>> GetWebServiceBoxItemsPagedAsync(GetBoxItemsPagedQuery query)
        {
            return m_ReadService.GetWebServiceBoxItemsPagedAsync(query);
        }
        public Task<IEnumerable<QuizDto>> GetBoxQuizesAsync(GetBoxQuizesPagedQuery query)
        {
            return m_ReadService.GetBoxQuizesAsync(query);
        }

       
        public Task<ItemDetailDto> GetItem2Async(GetItemQuery query)
        {
            return m_ReadService.GetItem2Async(query);
        }

        public Task<BoxDto2> GetBox2Async(GetBoxQuery query)
        {
            return m_ReadService.GetBox2Async(query);
        }
        public Task<BoxDtoWithMembers> GetBoxMetaWithMembersAsync(GetBoxQuery query, int numberOfMembers)
        {
            return m_ReadService.GetBoxMetaWithMembersAsync(query, numberOfMembers);
        }

        public Task<IEnumerable<TabDto>> GetBoxTabsAsync(GetBoxQuery query)
        {
            return m_ReadService.GetBoxTabsAsync(query);
        }

       

        public Task<IEnumerable<UserDto>> GetUserFriendsAsync(GetUserFriendsQuery query)
        {
            return m_ReadService.GetUserFriendsAsync(query);
        }

        public UserDetailDto GetUserData(GetUserDetailsQuery query)
        {
            return m_ReadService.GetUserData(query);
        }

        public Task<UserAccountDto> GetUserAccountDetailsAsync(GetUserDetailsQuery query)
        {
            return m_ReadService.GetUserAccountDetailsAsync(query);
        }

        public Task<IEnumerable<BoxNotificationDto>> GetUserBoxesNotificationAsync(GetUserDetailsQuery query)
        {
            return m_ReadService.GetUserBoxesNotificationAsync(query);
        }

        public Task<NotificationSettings> GetUserBoxNotificationSettingsAsync(GetBoxQuery query, long userId)
        {
            return m_ReadService.GetUserBoxNotificationSettingsAsync(query, userId);
        }


        public BoxSettingsDto GetBoxSetting(GetBoxQuery query, long userId)
        {
            return m_ReadService.GetBoxSetting(query, userId);
        }

        public Task<IEnumerable<UserMemberDto>> GetBoxMembersAsync(GetBoxQuery query)
        {
            return m_ReadService.GetBoxMembersAsync(query);
        }
        public Task<IEnumerable<long>> GetBoxUsersIdAsync(GetBoxWithUserQuery query)
        {
            return m_ReadService.GetBoxUsersIdAsync(query);
        }

        public Task<UserMinProfile> GetUserMinProfileAsync(GetUserMinProfileQuery query)
        {
            return m_Cache.QueryAsync
               (m_ReadService.GetUserMinProfileAsync, query);
            //return m_ReadService.GetUserMinProfile(query);
        }

        public Task<string> GetLocationByIpAsync(GetCountryByIpQuery query)
        {
            return m_Cache.QueryAsync
              (m_ReadService.GetLocationByIpAsync, query);
        }

        public Task<IEnumerable<QuestionDto>> GetQuestionsWithAnswersAsync(GetBoxQuestionsQuery query)
        {
            return m_ReadService.GetQuestionsWithAnswersAsync(query);
        }

        public Task<IEnumerable<QuestionDto>> GetQuestionsWithLastAnswerAsync(GetBoxQuestionsQuery query)
        {
            return m_ReadService.GetQuestionsWithLastAnswerAsync(query);
        }
        public Task<IEnumerable<AnswerDto>> GetRepliesAsync(GetCommentRepliesQuery query)
        {
            return m_ReadService.GetRepliesAsync(query);
        }

        public Task<QuestionDto> GetQuestionAsync(GetQuestionQuery query)
        {
            return m_ReadService.GetQuestionAsync(query);
        }

        public Task<bool> GetInviteAsync(GetInviteDetailQuery query)
        {
            return m_ReadService.GetInviteAsync(query);
        }



        public Task<LogInUserDto> GetUserDetailsByMembershipId(GetUserByMembershipQuery query)
        {
            return m_ReadService.GetUserDetailsByMembershipId(query);
        }

        public Task<LogInUserDto> GetUserDetailsByFacebookId(GetUserByFacebookQuery query)
        {
            return m_ReadService.GetUserDetailsByFacebookId(query);
        }

        public Task<LogInUserDto> GetUserDetailsByEmail(GetUserByEmailQuery query)
        {
            return m_ReadService.GetUserDetailsByEmail(query);
        }

        //public long GetItemIdByBlobId(string blobId)
        //{
        //    return m_ReadService.GetItemIdByBlobId(blobId);
        //}


        public Task<IEnumerable<BoxDto>> GetUserBoxesActivityAsync(GetUserWithFriendQuery query)
        {
            return m_ReadService.GetUserBoxesActivityAsync(query);
        }

      


        //public Task<IEnumerable<QuestionToFriendDto>> GetUserWithFriendQuestion(GetUserWithFriendQuery query)
        //{
        //    return m_ReadService.GetUserWithFriendQuestion(query);
        //}
        //public Task<IEnumerable<AnswerToFriendDto>> GetUserWithFriendAnswer(GetUserWithFriendQuery query)
        //{
        //    return m_ReadService.GetUserWithFriendAnswer(query);
        //}
        //public Task<IEnumerable<UserInviteDto>> GetUserPersonalInvites(GetInvitesQuery query)
        //{
        //    return m_ReadService.GetUserPersonalInvites(query);
        //}
        //public Task<UserToFriendActivity> GetUserWithFriendActivity(GetUserWithFriendQuery query)
        //{
        //    return m_ReadService.GetUserWithFriendActivity(query);
        //}




        //public Task<IEnumerable<AdminUserDto>> GetUniversityUsers(GetAdminUsersQuery query)
        //{
        //    return m_ReadService.GetUniversityUsers(query);
        //}


        public Task<IEnumerable<RussianDepartmentDto>> GetRussianDepartmentList(long universityId)
        {
            return m_ReadService.GetRussianDepartmentList(universityId);
        }


       

       


        public Task<IEnumerable<UpdatesDto>> GetUpdatesAsync(QueryBase query)
        {
            return m_ReadService.GetUpdatesAsync(query);
        }


        public Task<QuizWithDetailSolvedDto> GetQuizAsync(GetQuizQuery query)
        {
            return m_ReadService.GetQuizAsync(query);
        }

        public Task<QuizSeo> GetQuizSeoAsync(GetQuizSeoQuery query)
        {
            return m_ReadService.GetQuizSeoAsync(query);
        }


        public Task<QuizWithDetailDto> GetDraftQuizAsync(GetQuizDraftQuery query)
        {
            return m_ReadService.GetDraftQuizAsync(query);
        }


        public Task<IEnumerable<DiscussionDto>> GetDiscussionAsync(GetDisscussionQuery query)
        {
            return m_ReadService.GetDiscussionAsync(query);
        }


        public Task<UniversityWithCodeDto> GetUniversityNeedIdAsync(long universityId)
        {
            return m_ReadService.GetUniversityNeedIdAsync(universityId);
        }




        public Task<IEnumerable<UniversityByPrefixDto>> GetUniversityListByFriendsIdsAsync(IEnumerable<long> friendsIds)
        {
            return m_ReadService.GetUniversityListByFriendsIdsAsync(friendsIds);
        }


        public Task<IEnumerable<ActivityDto>> GetUserCommentActivityAsync(GetUserWithFriendQuery query)
        {
            return m_ReadService.GetUserCommentActivityAsync(query);
        }

        public Task<IEnumerable<string>> GetSeoItemsAsync(int page)
        {
            return m_ReadService.GetSeoItemsAsync(page);
        }

        public Task<int> GetSeoItemCountAsync()
        {
            return m_ReadService.GetSeoItemCountAsync();
        }

        public Task<IEnumerable<RecommendBoxDto>> GetRecommendedCoursesAsync(RecommendedCoursesQuery query)
        {
            return m_ReadService.GetRecommendedCoursesAsync(query);
        }


        public Task<int> GetNumberOfSolversAsync(long quizId)
        {
            return m_ReadService.GetNumberOfSolversAsync(quizId);
        }





        public Task<FileSeo> GetItemSeoAsync(GetFileSeoQuery query)
        {
            return m_ReadService.GetItemSeoAsync(query);
        }


        public Task<BoxSeoDto> GetBoxSeoAsync(GetBoxSeoQuery query)
        {
            return m_ReadService.GetBoxSeoAsync(query);
        }


        public Task<IEnumerable<LeaderBoardDto>> GetBoxLeaderBoardAsync(GetLeaderBoardQuery query)
        {
            return m_ReadService.GetBoxLeaderBoardAsync(query);
            //return m_Cache.QueryAsync(m_ReadService.GetBoxLeaderBoardAsync, query);
        }

        public Task<IEnumerable<RecommendBoxDto>> GetBoxRecommendedCoursesAsync(GetBoxSideBarQuery query)
        {
            return m_ReadService.GetBoxRecommendedCoursesAsync(query);
        }






      


        public Task<UserDetailDto> GetUserDataAsync(GetUserDetailsQuery query)
        {
            return m_ReadService.GetUserDataAsync(query);
        }


        public Task<ItemMobileDto> GetItemDetailApiAsync(GetItemQuery query)
        {
            return m_ReadService.GetItemDetailApiAsync(query);
        }


        public Task<LogInUserDto> GetUserDetailsById(GetUserByIdQuery query)
        {
            return m_ReadService.GetUserDetailsById(query);
        }


        public Task<IEnumerable<UniversityByPrefixDto>> GetUniversityByIpAddressAsync(ViewModel.Queries.Search.UniversityByIpQuery query)
        {
            return m_ReadService.GetUniversityByIpAddressAsync(query);
        }


        public Task<IEnumerable<ViewModel.Dto.ItemDtos.ItemDto>> GetUserItemsActivityAsync(GetUserWithFriendQuery query)
        {
            return m_ReadService.GetUserItemsActivityAsync(query);
        }
        public Task<IEnumerable<QuizDto>> GetUserQuizActivityAsync(GetUserWithFriendQuery query)
        {
            return m_ReadService.GetUserQuizActivityAsync(query);
        }


        public Task<IEnumerable<UserWithImageNameDto>> GetUsersByTermAsync(ViewModel.Queries.Search.UserSearchQuery query)
        {
            return m_ReadService.GetUsersByTermAsync(query);
        }


        public Task<QuizSolversWithCountDto> GetQuizSolversAsync(GetQuizBestSolvers query)
        {
            return m_ReadService.GetQuizSolversAsync(query);
        }


        public Task<QuizQuestionWithSolvedAnswersDto> GetQuizQuestionWithAnswersAsync(GetQuizQuery query)
        {
            return m_ReadService.GetQuizQuestionWithAnswersAsync(query);
        }


        public Task<IEnumerable<LeaderBoardDto>> GetDashboardLeaderBoardAsync(LeaderBoardQuery query)
        {
            return m_Cache.QueryAsync(m_ReadService.GetDashboardLeaderBoardAsync, query);
        }





        public Task<UserWithStats> GetUserProfileWithStatsAsync(GetUserWithFriendQuery query)
        {
            return m_ReadService.GetUserProfileWithStatsAsync(query);
        }


        //public Theme GetUserTheme(GetUserDetailsQuery query)
        //{
        //    return m_ReadService.GetUserTheme(query);
        //}
    }
}
