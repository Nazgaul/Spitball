using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries.Emails;

namespace Zbang.Zbox.ReadServicesTests
{
    [TestClass]
    public class ZboxReadServiceWorkerRoleTest
    {
        private IZboxReadServiceWorkerRole m_ZboxReadService;

        [TestInitialize]
        public void Setup()
        {
            var m_LocalStorageProvider = Rhino.Mocks.MockRepository.GenerateStub<ILocalStorageProvider>();
            Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.RegisterInstance<ILocalStorageProvider>(m_LocalStorageProvider);
            m_ZboxReadService = new ZboxReadServiceWorkerRole();
        }

        [TestMethod]
        public void GetUsersByNotificationSettings_Query_ReturnResult()
        {
            var query = new GetUserByNotificationQuery(Infrastructure.Enums.NotificationSettings.OnceADay);
            try
            {
                m_ZboxReadService.GetUsersByNotificationSettings(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }

        }

        [TestMethod]
        public void GetBoxesLastUpdates_Query_ReturnResult()
        {
            var query = new GetBoxesLastUpdateQuery(Infrastructure.Enums.NotificationSettings.OnceADay,1);
            try
            {
                m_ZboxReadService.GetBoxesLastUpdates(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }

        }

        [TestMethod]
        public void GetItemsLastUpdates_Query_ReturnResult()
        {
            var query = new GetItemsLastUpdateQuery(Infrastructure.Enums.NotificationSettings.OnceADay, 1);
            try
            {
                m_ZboxReadService.GetItemsLastUpdates(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }

        }
    }
}
