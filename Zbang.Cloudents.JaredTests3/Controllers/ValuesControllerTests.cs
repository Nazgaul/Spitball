using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Cloudents.Jared.Controllers;
using Rhino.Mocks;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.ReadServices;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Cloudents.Jared.Controllers.Tests
{
    [TestClass]
    public class ValuesControllerTests
    {
        private IZboxReadService m_ZboxReadService;
        [TestInitialize]
        public void Setup()
        {
            var localStorageProvider = MockRepository.GenerateStub<ILocalStorageProvider>();

            IocFactory.IocWrapper.RegisterInstance(localStorageProvider);

            m_ZboxReadService = new ZboxReadService();
        }

        //[TestMethod]
        //public async Task getCategoriesTextTest()
        //{
        //    int numOfDiff = 0;
        //    var b=await new ValuesController(m_ZboxReadService).getCategoriesText();
        //    var c = await new ValuesController(m_ZboxReadService).getCategoriesText();
        //    foreach (var item in b.Keys)
        //    {
        //        if (!b[item].Equals(c[item])) numOfDiff++;
        //    }
        //    var a = 5;
        //    var k = 6;
        //}
    }
}