using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlersTests
{
    [TestClass]
    public class CreateBoxTabCommandHandlerTests
    {
        private IBoxTabRepository m_StubBoxTabRepository;
        private IUserRepository m_StubUserRepository;

        [TestInitialize]
        public void Setup()
        {
            m_StubBoxTabRepository = MockRepository.GenerateStub<IBoxTabRepository>();
            m_StubUserRepository = MockRepository.GenerateStub<IUserRepository>();
        }

        [TestMethod]
        public void Handle_CommandOk_Saves()
        {
            var guid = Guid.NewGuid();
            var command = new CreateBoxTabCommand(guid, "test", 1);
            var theUser = new User("some email", "some name", "some image", "some large image");

            m_StubUserRepository.Stub(x => x.Get(1L)).Return(theUser);

            var dummy = new BoxTab(guid, "test", theUser);

            var handler = new CreateBoxTabCommandHandler(m_StubBoxTabRepository, m_StubUserRepository);

            handler.Handle(command);

            m_StubBoxTabRepository.AssertWasCalled(x => x.Save(dummy));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_UserNull_ThrowException()
        {
            var guid = Guid.NewGuid();
            var command = new CreateBoxTabCommand(guid, "test", 1);
            m_StubUserRepository.Stub(x => x.Get(1L)).Return(null);

            var handler = new CreateBoxTabCommandHandler(m_StubBoxTabRepository, m_StubUserRepository);
            handler.Handle(command);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Handle_DuplicateTabName_ThrowException()
        {
            var guid = Guid.NewGuid();
            long userId = 1;
            var duplicateTabName = "Some name";
            var someUser = new User("some email", "some name", "some image", "some large image");
            someUser.GetType().GetProperty("Id").SetValue(someUser, userId);

            var boxTab = new BoxTab(Guid.NewGuid(), duplicateTabName, someUser);

            m_StubUserRepository.Stub(x => x.Get(userId)).Return(someUser);
            var command = new CreateBoxTabCommand(guid, duplicateTabName, userId);

            m_StubBoxTabRepository.Stub(x => x.GetTabWithTheSameName(command.Name, 1L)).Return(boxTab);

            var handler = new CreateBoxTabCommandHandler(m_StubBoxTabRepository, m_StubUserRepository);
            handler.Handle(command);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_TagNameEmpty_ThrowException()
        {
            var guid = Guid.NewGuid();
            long userId = 1;
            var duplicateTabName = string.Empty;
            var someUser = new User("some email", "some name", "some image", "some large image");

            m_StubUserRepository.Stub(x => x.Get(userId)).Return(someUser);
            var command = new CreateBoxTabCommand(guid, duplicateTabName, userId);

            var handler = new CreateBoxTabCommandHandler(m_StubBoxTabRepository, m_StubUserRepository);
            handler.Handle(command);
        }
    }
}
