using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Cloudents.Jared.Controllers;
using Rhino.Mocks;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.ReadServices;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Ioc;
using System.Threading;
using System.Net.Http;
using System.Web.Http;

namespace Zbang.Cloudents.Jared.Controllers.Tests
{
    [TestClass]
    public class ValuesControllerTests
    {
        private IZboxReadService m_ZboxReadService;
        private ValuesController controller;
        [TestInitialize]
        public void Setup()
        {
            var localStorageProvider = MockRepository.GenerateStub<ILocalStorageProvider>();

            IocFactory.IocWrapper.RegisterInstance(localStorageProvider);

            m_ZboxReadService = new ZboxReadService();
            controller = new ValuesController(m_ZboxReadService);
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());
        }

        [TestMethod]
        public async Task getCategoriesTextTest()
        {
            int numOfDiff = 0;
            CancellationToken token = new CancellationToken();
            var b = await controller.Get(token);
            var c = await controller.Get(token);
           
            //foreach (var item in b.Content.)
            //{
            //    if (!b[item].Equals(c[item])) numOfDiff++;
            //}
            var a = 5;
            var k = 6;
        }
    }
}