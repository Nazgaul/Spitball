using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Zbox.ReadServicesTests
{
    [TestClass]
    public class BaseReadServiceTest
    {
        private IZboxReadService m_ZboxReadService;

        [TestInitialize]
        public void Setup()
        {
            var m_LocalStorageProvider = MockRepository.GenerateStub<ILocalStorageProvider>();
            var m_HttpCacheProvider = MockRepository.GenerateStub<IHttpContextCacheWrapper>();
            IocFactory.Unity.RegisterInstance(m_LocalStorageProvider);

            //  var blobProvider = Rhino.Mocks.MockRepository.GenerateStub<IBlobProvider>();
            m_ZboxReadService = new ZboxReadService(m_HttpCacheProvider);
        }

        [TestMethod]
        public async Task GetUserDetailsByFacebookId_Query_ReturnResult()
        {
            var query = new GetUserByFacebookQuery(1);
            try
            {
                var x = await m_ZboxReadService.GetUserDetailsByFacebookId(query);
            }
            catch (UserNotFoundException)
            {

            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public async Task GetUserDetailsByMembershipId_Query_ReturnResult()
        {
            var query = new GetUserByMembershipQuery(Guid.NewGuid());
            try
            {
                var x = await m_ZboxReadService.GetUserDetailsByMembershipId(query);
            }
            catch (UserNotFoundException)
            {

            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]
        public void GetUserDetailsByEmail_Query_ReturnResult()
        {
            var query = new GetUserByEmailQuery("yaari.ram@gmail.com");
            try
            {
                var x = m_ZboxReadService.GetUserDetailsByEmail(query).Result;
            }
            catch (UserNotFoundException)
            {

            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
    }
}
