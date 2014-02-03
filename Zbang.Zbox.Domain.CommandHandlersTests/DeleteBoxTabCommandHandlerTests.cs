using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;

namespace Zbang.Zbox.Domain.CommandHandlersTests
{
    [TestClass]
    public class DeleteBoxTabCommandHandlerTests
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
            var handler = new DeleteBoxTabCommandHandler(m_StubBoxTabRepository);
            handler.Handle(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException),"cannot delete tab that doesnt exits")]
        public void Handle_BoxTabDeosntExists_ThrowException()
        {
            var someCommand = new DeleteBoxTabCommand(1, Guid.NewGuid());
            var handler = new DeleteBoxTabCommandHandler(m_StubBoxTabRepository);
            handler.Handle(someCommand);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException), "other user cannot delete tag")]
        public void Handle_BoxTabDoesntConnectToUser_ThrowException()
        {
            var someUserId = 1L;
            var someBoxTabId = Guid.NewGuid();
            var someUser = new User("some email", "some name", "some image", "some large image");
            someUser.GetType().GetProperty("Id").SetValue(someUser, 2L);

            var someBoxTab = new BoxTab(someBoxTabId, "some name", someUser);

            var someCommand = new DeleteBoxTabCommand(someUserId,someBoxTabId);

            m_StubBoxTabRepository.Stub(x => x.Get(someBoxTabId)).Return(someBoxTab);

            var handler = new DeleteBoxTabCommandHandler(m_StubBoxTabRepository);
            handler.Handle(someCommand);
        }

        [TestMethod]
        public void Handle_InputOk_Delete()
        {
            var someUserId = 0;
            var someBoxTabId = Guid.NewGuid();
            var someUser = new User("some email", "some name", "some image", "some large image");
            var someCommand = new DeleteBoxTabCommand(someUserId, someBoxTabId);

            var someBoxTab = new BoxTab(someBoxTabId, "some name", someUser);

            m_StubBoxTabRepository.Stub(x => x.Get(someBoxTabId)).Return(someBoxTab);

            var handler = new DeleteBoxTabCommandHandler(m_StubBoxTabRepository);
            handler.Handle(someCommand);

            m_StubBoxTabRepository.AssertWasCalled(x => x.Delete(someBoxTab));
        }
    }
}
