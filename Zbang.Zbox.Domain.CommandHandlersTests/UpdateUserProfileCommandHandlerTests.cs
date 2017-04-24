using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Zbox.Domain.CommandHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.DataAccess;
using Rhino.Mocks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.CommandHandlers.Tests
{
    [TestClass()]
    public class UpdateUserProfileCommandHandlerTests
    {
        private IUserRepository m_StubUserRepository;

        [TestInitialize]
        public void Setup()
        {
            m_StubUserRepository = MockRepository.GenerateStub<IUserRepository>();
        }
        [TestMethod()]
        public void UpdateUserProfileImageCommandHandlerTest()
        {
            var someUserId = 2L;
            var someUser = new User("some email", "some largeImage", "some first name", "some last name", "en-US", Sex.NotApplicable);
            m_StubUserRepository.Stub(x => x.Get(someUserId)).Return(someUser);
            var command = new UpdateUserProfileCommand(someUserId,"yifat","123");
            m_StubUserRepository.Stub(x => x.Get(someUserId)).Return(someUser);
            m_StubUserRepository.Stub(x => x.Load(someUserId)).Return(someUser);
            var commandHandler = new UpdateUserProfileCommandHandler(m_StubUserRepository);
            commandHandler.Handle(command);
        }

        [TestMethod()]
        public void HandleTest()
        {
            Assert.Fail();
        }
    }
}