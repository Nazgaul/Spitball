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
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.Services;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Cache;
using System.Security.Principal;
using System.Web;
using System.Security.Claims;
using System;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Cloudents.Jared.DataObjects;
using System.Collections.Generic;

namespace Zbang.Cloudents.Jared.Controllers.Tests
{
    [TestClass()]
    public class CourseControllerTests
    {
        private IZboxWriteService m_ZboxWriteService;
        private readonly ICommandBus m_CommandBus;
        private readonly IWithCache m_Cache;
        private CourseController controller;
        [TestInitialize]
        public void Setup()
        {
            var localStorageProvider = MockRepository.GenerateStub<ILocalStorageProvider>();
            var m_CommandBus = MockRepository.GenerateMock<ICommandBus>();
            var m_Cache = MockRepository.GenerateMock<IWithCache>();
            IocFactory.IocWrapper.RegisterInstance(localStorageProvider);

            m_ZboxWriteService = MockRepository.GenerateStub<IZboxWriteService>();
            controller = new CourseController(m_ZboxWriteService);
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());
        }

        [TestMethod()]
        public async Task FollowAsyncTest()
        {
            var x = await controller.FollowAsync(null);
        }

        [TestMethod()]
        public async Task CreateAcademicBoxAsyncTestTooLongName()
        {
            controller.ModelState.Clear();
            
            var model = new CreateAcademicCourseRequest()
            {
                Professor = "hello",
                CourseName = new string('*', 5000)
            };
            controller.Validate(model);
            var result = await controller.CreateAcademicBoxAsync(model);
            Assert.IsTrue(result.ReasonPhrase == "Bad Request");
        }
        [TestMethod()]
        public async Task CreateAcademicBoxAsyncTestNoName()
        {
            controller.ModelState.Clear();

            var model = new CreateAcademicCourseRequest()
            {
                Professor = "hello",
            };
            controller.Validate(model);
            var result = await controller.CreateAcademicBoxAsync(model);
            Assert.IsTrue(result.ReasonPhrase == "Bad Request");
        }
        [TestMethod()]
        public async Task CreateAcademicBoxAsyncTestNoUniversity()
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal();
            controller.ModelState.Clear();

            var model = new CreateAcademicCourseRequest()
            {
                Professor = "hello",
                CourseName="Name"          
            };
            controller.Validate(model);
            var result = await controller.CreateAcademicBoxAsync(model);
            Assert.IsTrue(result.ReasonPhrase == "Bad Request");
        }
    }
}