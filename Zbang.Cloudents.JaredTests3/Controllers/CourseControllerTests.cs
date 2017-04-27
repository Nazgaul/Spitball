using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Zbang.Cloudents.Jared.Controllers;
using Zbang.Cloudents.Jared.DataObjects;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Cloudents.JaredTests3.Controllers
{
    [TestClass()]
    public class CourseControllerTests
    {
        private IZboxWriteService _mZboxWriteService;
        private CourseController _controller;
        [TestInitialize]
        public void Setup()
        {
            var localStorageProvider = MockRepository.GenerateStub<ILocalStorageProvider>();
            IocFactory.IocWrapper.RegisterInstance(localStorageProvider);

            _mZboxWriteService = MockRepository.GenerateStub<IZboxWriteService>();
            _controller = new CourseController(_mZboxWriteService) {Request = new HttpRequestMessage()};
            _controller.Request.SetConfiguration(new HttpConfiguration());
        }

        [TestMethod()]
        public async Task FollowAsyncTest()
        {
            var x = await _controller.FollowAsync(null);
        }

        [TestMethod()]
        public async Task CreateAcademicBoxAsyncTestTooLongName()
        {
            _controller.ModelState.Clear();

            var model = new CreateAcademicCourseRequest()
            {
                Professor = "hello",
                CourseName = new string('*', 5000)
            };
            _controller.Validate(model);
            var result = await _controller.CreateAcademicBoxAsync(model);
            Assert.IsTrue(result.ReasonPhrase == "Bad Request");
        }
        [TestMethod()]
        public async Task CreateAcademicBoxAsyncTestNoName()
        {
            _controller.ModelState.Clear();

            var model = new CreateAcademicCourseRequest()
            {
                Professor = "hello",
            };
            _controller.Validate(model);
            var result = await _controller.CreateAcademicBoxAsync(model);
            Assert.IsTrue(result.ReasonPhrase == "Bad Request");
        }
    }
}