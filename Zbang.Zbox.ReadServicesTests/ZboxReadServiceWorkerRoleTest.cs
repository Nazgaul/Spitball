using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Ioc;
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
            var m_LocalStorageProvider = MockRepository.GenerateStub<ILocalStorageProvider>();
            var m_BlobProvider = MockRepository.GenerateStub<IBlobProvider>();
            IocFactory.Unity.RegisterInstance(m_LocalStorageProvider);
            m_ZboxReadService = new ZboxReadServiceWorkerRole(m_BlobProvider);
        }

        [TestMethod]
        public void GetUsersByNotificationSettings_Query_ReturnResult()
        {
            var query = new GetUserByNotificationQuery(NotificationSettings.OnceADay);
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
            var query = new GetBoxesLastUpdateQuery(NotificationSettings.OnceADay,1);
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
            var query = new GetItemsLastUpdateQuery(NotificationSettings.OnceADay, 1);
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
