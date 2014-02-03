using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;

namespace Zbang.Zbox.Domain.CommandHandlersTests
{
    [TestClass]
    public class ChangeBoxTabNameCommandHandlerTests
    {
        private IBoxTabRepository m_StubBoxTabRepository;

        [TestInitialize]
        public void Setup()
        {
            m_StubBoxTabRepository = MockRepository.GenerateStub<IBoxTabRepository>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_NullInput_ThrowException()
        {
            var handler = new ChangeBoxTabNameCommandHandler(m_StubBoxTabRepository);
            handler.Handle(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_TabNotExists_ThrowException()
        {
            var tabId = Guid.NewGuid();
            var someCommand = new ChangeBoxTabNameCommand(tabId, "some name", 1);

            m_StubBoxTabRepository.Stub(x => x.Get(tabId)).Return(null);

            var handler = new ChangeBoxTabNameCommandHandler(m_StubBoxTabRepository);
            handler.Handle(someCommand);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void Handle_TabNotConnectedToUser_ThrowException()
        {
            var tabId = Guid.NewGuid();
            var someUser = new User("some email", "some user name", "some image", "some large img");
            var someCommand = new ChangeBoxTabNameCommand(tabId, "some name", 2);

            var someTab = new BoxTab(tabId, "some name", someUser);

            someUser.GetType().GetProperty("Id").SetValue(someUser, 1);

            m_StubBoxTabRepository.Stub(x => x.Get(tabId)).Return(someTab);

            var handler = new ChangeBoxTabNameCommandHandler(m_StubBoxTabRepository);
            handler.Handle(someCommand);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Handle_RenameToAlreadyBoxWithTheSameName_ThrowException()
        {
            var tabId = Guid.NewGuid();
            var someUserId = 2L;
            var someName = "some name";
            var someUser = new User("some email", "some user name", "some image", "some large img");
            someUser.GetType().GetProperty("Id").SetValue(someUser, someUserId);

            var someOtherTab = new BoxTab(Guid.NewGuid(), someName, someUser);
            var someTab = new BoxTab(tabId, someName, someUser);

            var someCommand = new ChangeBoxTabNameCommand(tabId, someName, someUserId);


            m_StubBoxTabRepository.Stub(x => x.GetTabWithTheSameName(someName, someUserId)).Return(someOtherTab);
            m_StubBoxTabRepository.Stub(x => x.Get(tabId)).Return(someTab);

            var handler = new ChangeBoxTabNameCommandHandler(m_StubBoxTabRepository);
            handler.Handle(someCommand);

        }

        [TestMethod]
        public void Handle_InputOk_Save()
        {
            var tabId = Guid.NewGuid();
            var someUserId = 1L;
            var someUser = new User("some email", "some user name", "some image", "some large img");
            var someCommand = new ChangeBoxTabNameCommand(tabId, "some name", someUserId);

            var someTab = new BoxTab(tabId, "some name", someUser);

            someUser.GetType().GetProperty("Id").SetValue(someUser, someUserId);

            m_StubBoxTabRepository.Stub(x => x.Get(tabId)).Return(someTab);

            var handler = new ChangeBoxTabNameCommandHandler(m_StubBoxTabRepository);
            handler.Handle(someCommand);

            m_StubBoxTabRepository.AssertWasCalled(x => x.Save(someTab));
        }
    }
}
