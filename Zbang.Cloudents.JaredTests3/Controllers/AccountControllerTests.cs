using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Cloudents.Jared.Controllers;
using Rhino.Mocks;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.ReadServices;
using System.Threading.Tasks;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel;
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
    public class AccountControllerTests
    {
        private AccountController controller;
        private IZboxWriteService m_ZboxWriteService;
        private IZboxReadService m_read;
        [TestInitialize]
        public void Setup()
        {
            var localStorageProvider = MockRepository.GenerateStub<ILocalStorageProvider>();
            IocFactory.IocWrapper.RegisterInstance(localStorageProvider);
            m_read = MockRepository.GenerateMock<IZboxReadService>();
            var m_CommandBus = MockRepository.GenerateMock<ICommandBus>();
            var m_Cache = MockRepository.GenerateMock<IWithCache>();
            m_ZboxWriteService = new ZboxWriteService(m_CommandBus, m_Cache);
            controller = new AccountController(m_ZboxWriteService,m_read);
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());
        }
        [TestMethod()]
        public void UpdateUniversityAsyncTest()
        {
            controller.ModelState.Clear();
            var model = new UpdateUniversityRequest() { UniversityId=long.Parse(18.ToString())};
            controller.Validate(model);
            var result = controller.UpdateUniversityAsync(null);
            Assert.IsTrue(result.ReasonPhrase == "Bad Request");
        }
    }
}