using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using System.Threading;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;
using Zbang.Zbox.ViewModel.Queries.Library;
using Zbang.Zbox.ViewModel.Queries.QnA;

namespace Zbang.Zbox.ReadServicesTests
{
    [TestClass]
    public class ZboxReadServiceTest
    {
        private IZboxReadService m_ZboxReadService;

        [TestInitialize]
        public void Setup()
        {
            var localStorageProvider = MockRepository.GenerateStub<ILocalStorageProvider>();

            IocFactory.IocWrapper.RegisterInstance(localStorageProvider);

            m_ZboxReadService = new ZboxReadService();
        }


        [TestMethod]
        public async Task GetUserBoxes_Query_ReturnResult()
        {
            var query = new GetBoxesQuery(1);
            try
            {
                var x = await m_ZboxReadService.GetUserBoxesAsync(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        //[TestMethod]
        //public async Task GetDashboardSideBar_Query_ReturnResult()
        //{
        //    var query = new GetDashboardQuery(920);
        //    try
        //    {
        //        var x = await m_ZboxReadService.GetDashboardSideBarAsync(query);
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.Fail("Expected no exception, but got: " + ex.Message);
        //    }
        //}



        //[TestMethod]
        //public async Task GetRecommendedCourses_Query_ReturnResult()
        //{
        //    var query = new RecommendedCoursesQuery(920, 1);
        //    try
        //    {
        //        var x = await m_ZboxReadService.GetRecommendedCoursesAsync(query);
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.Fail("Expected no exception, but got: " + ex.Message);
        //    }
        //}




        // [TestMethod]
        //public void GetMyData_Query_ReturnResult()
        //{
        //    var query = new GetDashboardQuery(1);
        //    try
        //    {
        //        m_ZboxReadService.GetMyData(query);
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.Fail("Expected no exception, but got: " + ex.Message);
        //    }
        //}

        [TestMethod]
        public void GetLibraryNode_Query_ReturnResult()
        {
            var query = new GetLibraryNodeQuery(1, null, 1);
            try
            {
                var x = m_ZboxReadService.GetLibraryNodeAsync(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetLibraryNode_QueryWithNode_ReturnResult()
        {
            var query = new GetLibraryNodeQuery(14, Guid.Parse("E6DBF958-8246-4669-915C-0041093B7FC1"), 1);
            try
            {
                var x = m_ZboxReadService.GetLibraryNodeAsync(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        //[TestMethod]
        //public void GetUniversityDetail_Query_ReturnResult()
        //{
        //    var query = new GetUniversityDetailQuery(1);
        //    try
        //    {
        //        var x = m_ZboxReadService.GetUniversityDetailAsync(query).Result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.Fail("Expected no exception, but got: " + ex.Message);
        //    }
        //}
        //[TestMethod]
        //public void GetInvites_Query_ReturnResult()
        //{
        //    var query = new GetInvitesQuery(1);
        //    try
        //    {
        //        m_ZboxReadService.GetInvitesAsync(query);
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.Fail("Expected no exception, but got: " + ex.Message);
        //    }
        //}

        [TestMethod]
        public async Task GetBox2_Query_ReturnResult()
        {
            var query = new GetBoxQuery(4);
            try
            {
                var x = await m_ZboxReadService.GetBox2Async(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public async Task GetBoxQuizes_Query_ReturnResult()
        {
            var query = new GetBoxQuizesPagedQuery(3732);
            try
            {
                var x = await m_ZboxReadService.GetBoxQuizesAsync(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetItem2_Query_ReturnResult()
        {
            var query = new GetItemQuery(3, 17, 4);
            try
            {
                var x = m_ZboxReadService.GetItem2Async(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        //[TestMethod]
        //public void GetRussianDepartmentList_Query_ReturnResult()
        //{
        //    try
        //    {
        //        var x = m_ZboxReadService.GetRussianDepartmentList(984).Result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.Fail("Expected no exception, but got: " + ex.Message);
        //    }
        //}

        [TestMethod]
        public void GetUniversityNeedId_Query_ReturnResult()
        {
            try
            {
                var x = m_ZboxReadService.GetUniversityNeedIdAsync(984).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }



        //[TestMethod]
        //public void GetUniversityListByFriendsIds_Query_ReturnResult()
        //{
        //    try
        //    {
        //        var x = m_ZboxReadService.GetUniversityListByFriendsIdsAsync(Enumerable.Range(0, 500).Select(s => (long)s)).Result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.Fail("Expected no exception, but got: " + ex.Message);
        //    }
        //}

        [TestMethod]
        public void GetInvite_Query_ReturnResult()
        {
            try
            {
                var query = new GetInviteDetailQuery(Guid.NewGuid());
                var x = m_ZboxReadService.GetInviteAsync(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetSeoItems_Query_ReturnResult()
        {
            try
            {
                var x = m_ZboxReadService.GetSeoItemsAsync(SeoType.Course, 1, default(CancellationToken)).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetSeoItemCount_Query_ReturnResult()
        {
            try
            {
                var x = m_ZboxReadService.GetSeoItemCountAsync().Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetQuizSeo_Query_ReturnResult()
        {
            try
            {
                var query = new GetQuizSeoQuery(1);
                var x = m_ZboxReadService.GetQuizSeoAsync(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetItemSeo_Query_ReturnResult()
        {
            try
            {
                var query = new GetFileSeoQuery(1);
                var x = m_ZboxReadService.GetItemSeoAsync(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public async Task GetBoxSeo_Query_ReturnResult()
        {
            try
            {
                var query = new GetBoxSeoQuery(4);
                var x = await m_ZboxReadService.GetBoxSeoAsync(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }


        [TestMethod]
        public async Task GetUserBoxNotificationSettings_Query_ReturnResult()
        {
            var query = new GetBoxQuery(1);
            try
            {
                await m_ZboxReadService.GetUserBoxNotificationSettingsAsync(query, 1);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetQuiz_Query_ReturnResult()
        {
            var query = new GetQuizQuery(1, 1);
            try
            {
                var x = m_ZboxReadService.GetQuizAsync(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetDiscussion_Query_ReturnResult()
        {
            var query = new GetDisscussionQuery(1);
            try
            {
                var x = m_ZboxReadService.GetDiscussionAsync(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetDraftQuiz_Query_ReturnResult()
        {
            var query = new GetQuizDraftQuery(1);
            try
            {
                var x = m_ZboxReadService.GetDraftQuizAsync(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetNumberOfSolvers_Query_ReturnResult()
        {
            var query = new GetQuizDraftQuery(1);
            try
            {
                var x = m_ZboxReadService.GetNumberOfSolversAsync(1).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }


        [TestMethod]
        public async Task GetBoxItemsPaged2_Query_ReturnResult()
        {
            var query = new GetBoxItemsPagedQuery(3732, null);
            try
            {
                var x = await m_ZboxReadService.GetBoxItemsPagedAsync(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public async Task GetBoxItemsPaged2WithTab_Query_ReturnResult()
        {
            var query = new GetBoxItemsPagedQuery(3732, null);
            try
            {
                var x = await m_ZboxReadService.GetBoxItemsPagedAsync(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetUserFriends_Query_ReturnResult()
        {
            var query = new GetUserFriendsQuery(1);
            try
            {
                m_ZboxReadService.GetUserFriendsAsync(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        //[TestMethod]
        //public async Task GetLocationByIP_Query_ReturnResult()
        //{
        //    try
        //    {
        //        await m_ZboxReadService.GetLocationByIpAsync(new GetCountryByIpQuery(200));
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.Fail("Expected no exception, but got: " + ex.Message);
        //    }
        //}

        [TestMethod]
        public async Task GetUserAccountDetails_Query_ReturnResult()
        {
            var query = new QueryBaseUserId(1);
            try
            {
                await m_ZboxReadService.GetUserAccountDetailsAsync(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public async Task GetUserBoxesNotification_Query_ReturnResult()
        {
            var query = new QueryBaseUserId(1);
            try
            {
                await m_ZboxReadService.GetUserBoxesNotificationAsync(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetBoxMembers_Query_ReturnResult()
        {
            var query = new GetBoxQuery(1);
            try
            {
                m_ZboxReadService.GetBoxMembersAsync(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetUserMinProfile_Query_ReturnResult()
        {
            var query = new GetUserMinProfileQuery(1);
            try
            {
                var x = m_ZboxReadService.GetUserMinProfileAsync(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetUserWithFriendBoxes_Query_ReturnResult()
        {
            var query = new GetUserWithFriendQuery(1, 2);
            try
            {
                var x = m_ZboxReadService.GetUserBoxesActivityAsync(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetUpdates_Query_ReturnResult()
        {
            var query = new QueryBase(18);
            try
            {
                var x = m_ZboxReadService.GetUpdatesAsync(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        //[TestMethod]
        //public async Task GetBoxLeaderBoard_Query_ReturnResult()
        //{
        //    var query = new GetLeaderBoardQuery(60130);
        //    try
        //    {
        //        var x = await m_ZboxReadService.GetBoxLeaderBoardAsync(query);
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.Fail("Expected no exception, but got: " + ex.Message);
        //    }
        //}

        //[TestMethod]
        //public async Task GetBoxRecommendedCourses_Query_ReturnResult()
        //{
        //    var query = new GetBoxSideBarQuery(60130,1);
        //    try
        //    {
        //        var x = await m_ZboxReadService.GetBoxRecommendedCoursesAsync(query,default(CancellationToken));
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.Fail("Expected no exception, but got: " + ex.Message);
        //    }
        //}
    }
}
