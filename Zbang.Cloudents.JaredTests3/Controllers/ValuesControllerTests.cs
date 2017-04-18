using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Cloudents.Jared.Controllers;
using Rhino.Mocks;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.ReadServices;
using System.Threading.Tasks;

namespace Zbang.Cloudents.Jared.Controllers.Tests
{
    [TestClass]
    public class ValuesControllerTests
    {
        private IZboxReadService m_ZboxReadService;
        [TestInitialize]
        public void Setup()
        {
            m_ZboxReadService = MockRepository.GenerateStub<IZboxReadService>();
        }
        [TestMethod()]
        public void getCategoriesTextTest()
        {
            var b=new ValuesController(m_ZboxReadService).getCategoriesText();
            Task.WaitAll(b);
            var c=5;
            c = 7;

        }
    }
}