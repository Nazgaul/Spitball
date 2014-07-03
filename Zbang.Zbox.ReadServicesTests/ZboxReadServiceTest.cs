using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Culture;
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

        private string phrase = "s";
        [TestInitialize]
        public void Setup()
        {
            var m_LocalStorageProvider = Rhino.Mocks.MockRepository.GenerateStub<ILocalStorageProvider>();
            var m_EnglishStemmer = Rhino.Mocks.MockRepository.GenerateStub<IEnglishToHebrewChars>();
            var m_HebrewStemmer = Rhino.Mocks.MockRepository.GenerateStub<IHebrewStemmer>();
            var m_BlobProvider = Rhino.Mocks.MockRepository.GenerateStub<IBlobProvider>();
            var m_FilterWords = MockRepository.GenerateStub<IFilterWords>();
            m_FilterWords.Stub(x => x.RemoveWords(phrase)).Return(phrase);
            m_EnglishStemmer.Stub(x => x.TransferEnglishCharsToHebrew(phrase)).Return(phrase);
            m_HebrewStemmer.Stub(x => x.StemAHebrewWord(phrase)).Return(phrase);
            var m_HttpCacheProvider = Rhino.Mocks.MockRepository.GenerateStub<IHttpContextCacheWrapper>();

            Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.RegisterInstance<ILocalStorageProvider>(m_LocalStorageProvider);
            Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.RegisterInstance<IEnglishToHebrewChars>(m_EnglishStemmer);
            Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.RegisterInstance<IHebrewStemmer>(m_HebrewStemmer);
            Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.RegisterInstance<IFilterWords>(m_FilterWords);

            m_ZboxReadService = new ZboxReadService(m_HttpCacheProvider);
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
            var query = new GetLibraryNodeQuery(1, null, 1, 1, Infrastructure.Enums.OrderBy.LastModified);
            try
            {
                m_ZboxReadService.GetLibraryNode(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetLibraryNode_QueryWithNode_ReturnResult()
        {
            var query = new GetLibraryNodeQuery(14, Guid.Parse("3d49e348-33e2-4281-b763-d981b9bd0000"), 1, 0, Infrastructure.Enums.OrderBy.LastModified);
            try
            {
                m_ZboxReadService.GetLibraryNode(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetUniversityDetail_Query_ReturnResult()
        {
            var query = new GetUniversityDetailQuery(1, 1);
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
        public void GetBox2_Query_ReturnResult()
        {
            var query = new GetBoxQuery(1, 1);
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
            var query = new GetItemQuery(1, 25, 1);
            try
            {
                m_ZboxReadService.GetItem(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetRate_Query_ReturnResult()
        {
            var query = new GetItemRateQuery(1, 1);
            try
            {
                m_ZboxReadService.GetRate(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetBoxComments_Query_ReturnResult()
        {
            var query = new GetBoxCommentsQuery(1, 1);
            try
            {
                m_ZboxReadService.GetBoxComments(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetQuestions_Query_ReturnResult()
        {
            var query = new GetBoxQuestionsQuery(1, 1);
            try
            {
                m_ZboxReadService.GetQuestions(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        [TestMethod]
        public void GetItemComments_Query_ReturnResult()
        {
            var query = new GetItemCommentsQuery(1, 1);
            try
            {
                m_ZboxReadService.GetItemComments(query);
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
    }
}
