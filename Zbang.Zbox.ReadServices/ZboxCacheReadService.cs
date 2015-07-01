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
using Zbang.Zbox.ViewModel.Dto.Store;
using Zbang.Zbox.ViewModel.Dto.UserDtos;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;
using Zbang.Zbox.ViewModel.Queries.Library;
using Zbang.Zbox.ViewModel.Queries.QnA;
using Zbang.Zbox.ViewModel.Queries.User;
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

        public Task<IEnumerable<BoxDto>> GetUserBoxes(GetBoxesQuery query)
        {
            return m_ReadService.GetUserBoxes(query);
        }

        public Task<UniversityDashboardInfoDto> GetMyData(GetDashboardQuery query)
        {
            return m_ReadService.GetMyData(query);
        }

        public Task<NodeBoxesDto> GetLibraryNode(GetLibraryNodeQuery query)
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

        public Task<IEnumerable<ViewModel.Dto.ItemDtos.ItemDto>> GetBoxItemsPagedAsync(GetBoxItemsPagedQuery query)
        {
            return m_ReadService.GetBoxItemsPagedAsync(query);
        }
        public Task<IEnumerable<QuizDto>> GetBoxQuizes(GetBoxQuizesPagedQuery query)
        {
            return m_ReadService.GetBoxQuizes(query);
        }

        public ItemWithDetailDto GetItem(GetItemQuery query)
        {
            return m_ReadService.GetItem(query);
        }
        public Task<ItemDetailDto> GetItem2(GetItemQuery query)
        {
            return m_ReadService.GetItem2(query);
        }

        public Task<BoxDto2> GetBox2(GetBoxQuery query)
        {
            return m_ReadService.GetBox2(query);
        }

        public Task<IEnumerable<TabDto>> GetBoxTabs(GetBoxQuery query)
        {
            return m_ReadService.GetBoxTabs(query);
        }

       

        public Task<IEnumerable<UserDto>> GetUserFriends(GetUserFriendsQuery query)
        {
            return m_ReadService.GetUserFriends(query);
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

        public NotificationSettings GetUserBoxNotificationSettings(GetBoxQuery query, long userId)
        {
            return m_ReadService.GetUserBoxNotificationSettings(query, userId);
        }


        public BoxSettingsDto GetBoxSetting(GetBoxQuery query, long userId)
        {
            return m_ReadService.GetBoxSetting(query, userId);
        }

        public Task<IEnumerable<UserMemberDto>> GetBoxMembers(GetBoxQuery query)
        {
            return m_ReadService.GetBoxMembers(query);
        }
        public Task<IEnumerable<long>> GetBoxUsersId(GetBoxWithUserQuery query)
        {
            return m_ReadService.GetBoxUsersId(query);
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

        public Task<IEnumerable<QuestionDto>> GetQuestionsWithAnswers(GetBoxQuestionsQuery query)
        {
            return m_ReadService.GetQuestionsWithAnswers(query);
        }

        public Task<IEnumerable<QuestionDto>> GetQuestionsWithLastAnswer(GetBoxQuestionsQuery query)
        {
            return m_ReadService.GetQuestionsWithLastAnswer(query);
        }
        public Task<IEnumerable<AnswerDto>> GetReplies(GetCommentRepliesQuery query)
        {
            return m_ReadService.GetReplies(query);
        }
        public Task<bool> GetInvite(GetInviteDetailQuery query)
        {
            return m_ReadService.GetInvite(query);
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

        public long GetItemIdByBlobId(string blobId)
        {
            return m_ReadService.GetItemIdByBlobId(blobId);
        }


        public Task<IEnumerable<BoxDto>> GetUserWithFriendBoxes(GetUserWithFriendQuery query)
        {
            return m_ReadService.GetUserWithFriendBoxes(query);
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


        public Task<IEnumerable<RussianDepartmentDto>> GetRussianDepartmentList(long universityId)
        {
            return m_ReadService.GetRussianDepartmentList(universityId);
        }


       

       


        public Task<IEnumerable<UpdatesDto>> GetUpdates(QueryBase query)
        {
            return m_ReadService.GetUpdates(query);
        }


        public Task<QuizWithDetailSolvedDto> GetQuiz(GetQuizQuery query)
        {
            return m_ReadService.GetQuiz(query);
        }

        public Task<QuizSeo> GetQuizSeo(GetQuizSeoQuery query)
        {
            return m_ReadService.GetQuizSeo(query);
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


        //public Task<IEnumerable<ProductDto>> GetProducts(GetStoreProductsByCategoryQuery query)
        //{
        //    return m_ReadService.GetProducts(query);
        //}


        //public IEnumerable<CategoryDto> GetCategories()
        //{
        //    return m_ReadService.GetCategories();
        //}


        //public Task<ProductWithDetailDto> GetProduct(GetStoreProductQuery query)
        //{
        //    return m_ReadService.GetProduct(query);
        //}
        //public Task<ProductCheckOutDto> GetProductCheckOut(GetStoreProductQuery query)
        //{
        //    return m_ReadService.GetProductCheckOut(query);
        //}


        //public Task<IEnumerable<ProductDto>> SearchProducts(SearchProductQuery query)
        //{
        //    return m_ReadService.SearchProducts(query);
        //}


        //public Task<IEnumerable<BannerDto>> GetBanners(int? universityId)
        //{
        //    return m_ReadService.GetBanners(universityId);
        //}


        //public Task<bool> ValidateCoupon(int coupon)
        //{
        //    return m_ReadService.ValidateCoupon(coupon);
        //}


        //public Task<int?> CloudentsUniversityToStoreUniversity(long universityId)
        //{
        //    return m_ReadService.CloudentsUniversityToStoreUniversity(universityId);
        //}

        public Task<IEnumerable<RecommendBoxDto>> GetRecommendedCourses(RecommendedCoursesQuery query)
        {
            return m_ReadService.GetRecommendedCourses(query);
        }


        public Task<int> GetNumberOfSolvers(long quizId)
        {
            return m_ReadService.GetNumberOfSolvers(quizId);
        }





        public Task<FileSeo> GetItemSeo(GetFileSeoQuery query)
        {
            return m_ReadService.GetItemSeo(query);
        }


        public Task<BoxSeoDto> GetBoxSeo(GetBoxSeoQuery query)
        {
            return m_ReadService.GetBoxSeo(query);
        }


        public Task<IEnumerable<LeaderBoardDto>> GetBoxLeaderBoard(GetLeaderBoardQuery query)
        {
            return m_ReadService.GetBoxLeaderBoard(query);
            //return m_Cache.QueryAsync(m_ReadService.GetBoxLeaderBoard, query);
        }

        public Task<IEnumerable<RecommendBoxDto>> GetBoxRecommendedCourses(GetBoxSideBarQuery query)
        {
            return m_ReadService.GetBoxRecommendedCourses(query);
        }






        public Task<DashboardDto> GetDashboardSideBar(GetDashboardQuery query)
        {
            return m_Cache.QueryAsync(m_ReadService.GetDashboardSideBar, query);
        }


        public Task<UserDetailDto> GetUserDataAsync(GetUserDetailsQuery query)
        {
            return m_ReadService.GetUserDataAsync(query);
        }


        public Task<ItemMobileDto> GetItemDetailApi(GetItemQuery query)
        {
            return m_ReadService.GetItemDetailApi(query);
        }


        public Task<LogInUserDto> GetUserDetailsById(GetUserByIdQuery query)
        {
            return m_ReadService.GetUserDetailsById(query);
        }


        public Task<IEnumerable<UniversityByPrefixDto>> GetUniversityByIpAddress(ViewModel.Queries.Search.UniversityByIpQuery query)
        {
            return m_ReadService.GetUniversityByIpAddress(query);
        }
    }
}
