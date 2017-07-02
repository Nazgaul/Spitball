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
using System.Collections.Generic;
using Zbang.Cloudents.Jared.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Profile;

namespace Zbang.Cloudents.Jared.Controllers.Tests
{
    [TestClass]
    public class AccountControllerTests
    {
        private AccountController m_Controller;
        private IZboxWriteService m_ZboxWriteService;
        private IZboxReadService m_Read;
        [TestInitialize]
        public void Setup()
        {
            var localStorageProvider = MockRepository.GenerateStub<ILocalStorageProvider>();
            IocFactory.IocWrapper.RegisterInstance(localStorageProvider);
            m_Read = MockRepository.GenerateMock<IZboxReadService>();
            var m_CommandBus = MockRepository.GenerateMock<ICommandBus>();
            var m_Cache = MockRepository.GenerateMock<IWithCache>();
            var profile = MockRepository.GenerateMock<IProfilePictureProvider>();
            m_ZboxWriteService = new ZboxWriteService(m_CommandBus, m_Cache);
            m_Controller = new AccountController(m_ZboxWriteService,m_Read, profile);
            m_Controller.Request = new HttpRequestMessage();
            m_Controller.Request.SetConfiguration(new HttpConfiguration());
        }
        [TestMethod()]
        public void UpdateUniversityAsyncTest()
        {
            m_Controller.ModelState.Clear();
            var model = new UpdateUniversityRequest { UniversityId=long.Parse(18.ToString())};
            m_Controller.Validate(model);
            var command = new UpdateUserUniversityCommand(model.UniversityId, 4, null);
            var c = command.StudentId ?? "678";
            var result = m_Controller.UpdateUniversityAsync(null);
            Assert.IsTrue(result.ReasonPhrase == "Bad Request");
        }
    }
}