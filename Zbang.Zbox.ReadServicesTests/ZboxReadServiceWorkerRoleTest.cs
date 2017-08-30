using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
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
            var m_Logger = MockRepository.GenerateStub<ILogger>();
            IocFactory.IocWrapper.RegisterInstance(m_LocalStorageProvider);
            m_ZboxReadService = new ZboxReadServiceWorkerRole(m_Logger);
        }

        [TestMethod]
        public void GetUsersByNotificationSettings_Query_ReturnResult()
        {
            var query = new GetUserByNotificationQuery(NotificationSetting.OnceADay,1,10,1);
            try
            {
                m_ZboxReadService.GetUsersByNotificationSettingsAsync(query, default(CancellationToken));
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }

        }

        [TestMethod]
        public void GetFlagItemUserDetail_Query_ReturnResult()
        {
            var query = new GetBadItemFlagQuery(1, 149242);
            try
            {
                m_ZboxReadService.GetFlagItemUserDetail(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetBoxesLastUpdates_Query_ReturnResult()
        {
            var query = new GetBoxesLastUpdateQuery(NotificationSetting.OnceADay, 1);
            try
            {
                m_ZboxReadService.GetBoxesLastUpdatesAsync(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }

        }

        [TestMethod]
        public async Task GetItemsLastUpdates_Query_ReturnResult()
        {
            var query = new GetBoxLastUpdateQuery(NotificationSetting.OnceADay, 4511);
            try
            {
                await m_ZboxReadService.GetBoxLastUpdatesAsync(query);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }

        }
    }
}
