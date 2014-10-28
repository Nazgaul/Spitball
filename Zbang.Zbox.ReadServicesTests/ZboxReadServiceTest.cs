using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;
using Zbang.Zbox.ViewModel.Queries.Library;
using Zbang.Zbox.ViewModel.Queries.QnA;
using Zbang.Zbox.ViewModel.Queries.Search;
using Zbang.Zbox.ViewModel.Queries.User;

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

            IocFactory.Unity.RegisterInstance(localStorageProvider);

            m_ZboxReadService = new ZboxReadService();
        }


        [TestMethod]
        public void GetDashboard_Query_ReturnResult()
        {
            var query = new GetBoxesQuery(1);
            try
            {
                var x = m_ZboxReadService.GetDashboard(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetRecommendedCourses_Query_ReturnResult()
        {
            var query = new RecommendedCoursesQuery(920);
            try
            {
                var x = m_ZboxReadService.GetRecommendedCourses(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

      


        [TestMethod]
        public void Search_Query_ReturnResult()
        {
            var query = new GroupSearchQuery("1", 14, 1, false);
            try
            {
                var x = m_ZboxReadService.Search(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void OtherUniversities_Query_ReturnResult()
        {
            var query = new GroupSearchQuery("1", 14, 1, false);
            try
            {
                var x = m_ZboxReadService.OtherUniversities(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetMyData_Query_ReturnResult()
        {
            var query = new GetDashboardQuery(1);
            try
            {
                m_ZboxReadService.GetMyData(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetLibraryNode_Query_ReturnResult()
        {
            var query = new GetLibraryNodeQuery(1, null, 1);
            try
            {
                var x = m_ZboxReadService.GetLibraryNode(query).Result;
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
                var x = m_ZboxReadService.GetLibraryNode(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetUniversityDetail_Query_ReturnResult()
        {
            var query = new GetUniversityDetailQuery(1);
            try
            {
                var x = m_ZboxReadService.GetUniversityDetail(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetInvites_Query_ReturnResult()
        {
            var query = new GetInvitesQuery(1);
            try
            {
                m_ZboxReadService.GetInvites(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetBox_Query_ReturnResult()
        {
            var query = new GetBoxQuery(60130, 1);
            try
            {
                m_ZboxReadService.GetBox(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetBox2_Query_ReturnResult()
        {
            var query = new GetBoxQuery(4, 3);
            try
            {
                var x = m_ZboxReadService.GetBox2(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetBoxTabs_Query_ReturnResult()
        {
            var query = new GetBoxQuery(1, 1);
            try
            {
                var x = m_ZboxReadService.GetBoxTabs(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public async Task GetBoxQuizes_Query_ReturnResult()
        {
            var query = new GetBoxItemsPagedQuery(1, 1);
            try
            {
                var x = await m_ZboxReadService.GetBoxQuizes(query);
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
                var x = m_ZboxReadService.GetItem2(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetRussianDepartmentList_Query_ReturnResult()
        {
            try
            {
                var x = m_ZboxReadService.GetRussianDepartmentList(984).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetUniversityNeedId_Query_ReturnResult()
        {
            try
            {
                var x = m_ZboxReadService.GetUniversityNeedId(984).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetUniversityNeedCode_Query_ReturnResult()
        {
            try
            {
                var x = m_ZboxReadService.GetUniversityNeedCode(984).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetUniversityListByFriendsIds_Query_ReturnResult()
        {
            try
            {
                var x = m_ZboxReadService.GetUniversityListByFriendsIds(Enumerable.Range(0, 500).Select(s => (long)s)).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetInvite_Query_ReturnResult()
        {
            try
            {
                var query = new GetInviteDetailQuery(Guid.NewGuid());
                var x = m_ZboxReadService.GetInvite(query).Result;
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
                var x = m_ZboxReadService.GetSeoItems(1).Result;
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
                var x = m_ZboxReadService.GetSeoItemCount().Result;
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
                var x = m_ZboxReadService.GetQuizSeo(query).Result;
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
                var x = m_ZboxReadService.GetItemSeo(query).Result;
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
                var query = new GetBoxSeoQuery(4, 3);
                var x = await m_ZboxReadService.GetBoxSeo(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }


        [TestMethod]
        public void GetUserBoxNotificationSettings_Query_ReturnResult()
        {
            var query = new GetBoxQuery(1, 1);
            try
            {
                m_ZboxReadService.GetUserBoxNotificationSettings(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetQuiz_Query_ReturnResult()
        {
            var query = new GetQuizQuery(1, 1, 1);
            try
            {
                var x = m_ZboxReadService.GetQuiz(query);
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
                var x = m_ZboxReadService.GetDiscussion(query);
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
                var x = m_ZboxReadService.GetDraftQuiz(query).Result;
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
                var x = m_ZboxReadService.GetNumberOfSolvers(1).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetProducts_Query_ReturnResult()
        {
            var query = new GetStoreProductsByCategoryQuery(null, null, null);
            try
            {
                var x = m_ZboxReadService.GetProducts(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void SearchProducts_Query_ReturnResult()
        {
            var query = new SearchProductQuery("א", null);
            try
            {
                var x = m_ZboxReadService.SearchProducts(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetProduct_Query_ReturnResult()
        {
            var query = new GetStoreProductQuery(1);
            try
            {
                var x = m_ZboxReadService.GetProduct(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetProductCheckOut_Query_ReturnResult()
        {
            var query = new GetStoreProductQuery(1);
            try
            {
                var x = m_ZboxReadService.GetProductCheckOut(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetBanners_Query_ReturnResult()
        {
            try
            {
                var x = m_ZboxReadService.GetBanners(null).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void ValidateCoupon_Query_ReturnResult()
        {
            try
            {
                var x = m_ZboxReadService.ValidateCoupon(10).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void CloudentsUniversityToStoreUniversity_Query_ReturnResult()
        {
            try
            {
                var x = m_ZboxReadService.CloudentsUniversityToStoreUniversity(10).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetCategories_Query_ReturnResult()
        {
            try
            {
                var x = m_ZboxReadService.GetCategories();
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetBoxItemsPaged2_Query_ReturnResult()
        {
            var query = new GetBoxItemsPagedQuery(1, 1);
            try
            {
                m_ZboxReadService.GetBoxItemsPaged2(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetItem_Query_ReturnResult()
        {
            var query = new GetItemQuery(3, 17, 4);
            try
            {
                m_ZboxReadService.GetItem(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }


        //[TestMethod]
        //public void GetBoxComments_Query_ReturnResult()
        //{
        //    var query = new GetBoxCommentsQuery(1, 1);
        //    try
        //    {
        //        m_ZboxReadService.GetBoxComments(query);
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.Fail("Expected no exception, but got: " + ex.Message);
        //    }
        //}
        [TestMethod]
        public async Task GetQuestions_Query_ReturnResult()
        {
            var query = new GetBoxQuestionsQuery(60130, 1);
            try
            {
              var x =  await m_ZboxReadService.GetQuestions(query);
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
                m_ZboxReadService.GetUserFriends(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetLocationByIP_Query_ReturnResult()
        {
            try
            {
                m_ZboxReadService.GetLocationByIp(200);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        //[TestMethod]
        //public void GetUniversityListByPrefix_Query_ReturnResult()
        //{
        //    var query = new GetUniversityByPrefixQuery();
        //    try
        //    {
        //        var x = m_ZboxReadService.GetUniversityListByPrefix(query).Result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.Fail("Expected no exception, but got: " + ex.Message);
        //    }
        //}
        [TestMethod]
        public void GetUserData_Query_ReturnResult()
        {
            var query = new GetUserDetailsQuery(1);
            try
            {
                m_ZboxReadService.GetUserData(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetBoxSetting_Query_ReturnResult()
        {
            var query = new GetBoxQuery(1, 1);
            try
            {
                m_ZboxReadService.GetBoxSetting(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetUserAccountDetails_Query_ReturnResult()
        {
            var query = new GetUserDetailsQuery(1);
            try
            {
                m_ZboxReadService.GetUserAccountDetails(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetUserBoxesNotification_Query_ReturnResult()
        {
            var query = new GetUserDetailsQuery(1);
            try
            {
                m_ZboxReadService.GetUserBoxesNotification(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetBoxMembers_Query_ReturnResult()
        {
            var query = new GetBoxQuery(1, 1);
            try
            {
                m_ZboxReadService.GetBoxMembers(query);
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
                var x = m_ZboxReadService.GetUserMinProfile(query).Result;
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
                var x = m_ZboxReadService.GetUserWithFriendBoxes(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetUserWithFriendFiles_Query_ReturnResult()
        {
            var query = new GetUserWithFriendQuery(4, 1);
            try
            {
                var x = m_ZboxReadService.GetUserWithFriendFiles(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetUserWithFriendQuestion_Query_ReturnResult()
        {
            var query = new GetUserWithFriendQuery(4, 1);
            try
            {
                var x = m_ZboxReadService.GetUserWithFriendQuestion(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetUserWithFriendAnswer_Query_ReturnResult()
        {
            var query = new GetUserWithFriendQuery(4, 1);
            try
            {
                var x = m_ZboxReadService.GetUserWithFriendAnswer(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetUserWithFriendActivity_Query_ReturnResult()
        {
            var query = new GetUserWithFriendQuery(4, 1);
            try
            {
                var x = m_ZboxReadService.GetUserWithFriendActivity(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetUserPersonalInvites_Query_ReturnResult()
        {
            var query = new GetInvitesQuery(1);
            try
            {
                var x = m_ZboxReadService.GetUserPersonalInvites(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetUniversityUsers_Query_ReturnResult()
        {
            var query = new GetAdminUsersQuery(14);
            try
            {
                var x = m_ZboxReadService.GetUniversityUsers(query).Result;
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
                var x = m_ZboxReadService.GetUpdates(query).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public async Task GetBoxSideBar_Query_ReturnResult()
        {
            var query = new GetBoxSideBarQuery(60130);
            try
            {
                var x = await m_ZboxReadService.GetBoxSideBar(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
    }
}
