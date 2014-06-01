using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Cache;

namespace Zbang.Zbox.ReadServicesTests
{
    [TestClass]
    public class BaseReadServiceTest
    {
        private IZboxReadService m_ZboxReadService;

        [TestInitialize]
        public void Setup()
        {
            var m_LocalStorageProvider = Rhino.Mocks.MockRepository.GenerateStub<ILocalStorageProvider>();
            var m_HttpCacheProvider = Rhino.Mocks.MockRepository.GenerateStub<IHttpContextCacheWrapper>();
            Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.RegisterInstance<ILocalStorageProvider>(m_LocalStorageProvider);

          //  var blobProvider = Rhino.Mocks.MockRepository.GenerateStub<IBlobProvider>();
            m_ZboxReadService = new ZboxReadService(m_HttpCacheProvider);
        }

        [TestMethod]
        public void GetUserDetailsByFacebookId_Query_ReturnResult()
        {
            var query = new ViewModel.Queries.GetUserByFacebookQuery(1);
            try
            {
                m_ZboxReadService.GetUserDetailsByFacebookId(query);
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
        public void GetUserDetailsByMembershipId_Query_RetrurnResult()
        {
            var query = new ViewModel.Queries.GetUserByMembershipQuery(Guid.NewGuid());
            try
            {
                m_ZboxReadService.GetUserDetailsByMembershipId(query);
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
