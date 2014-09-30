using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Domain.Commands;

namespace Zbang.Zbox.Domain.CommandHandlersTests
{
    [TestClass]
    public class DeleteItemTabCommandHandlerTests
    {
        private IItemTabRepository m_StubItemTabRepository;
        private IUserRepository m_StubUserRepository;

        [TestInitialize]
        public void Setup()
        {
            m_StubItemTabRepository = MockRepository.GenerateStub<IItemTabRepository>();
            m_StubUserRepository = MockRepository.GenerateStub<IUserRepository>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_NullInput_ThrowException()
        {
            var handler = new DeleteItemTabCommandHandler(m_StubItemTabRepository, m_StubUserRepository);
            handler.Handle(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "cannot delete tab that doesnt exits")]
        public void Handle_ItemTabDeosntExists_ThrowException()
        {
            var someCommand = new DeleteItemTabCommand(1, Guid.NewGuid(), 1);
            var handler = new DeleteItemTabCommandHandler(m_StubItemTabRepository, m_StubUserRepository);
            m_StubUserRepository.Stub(x => x.GetUserToBoxRelationShipType(1, 1)).Return(Infrastructure.Enums.UserRelationshipType.Owner);

            handler.Handle(someCommand);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException), "other user cannot delete tag")]
        public void Handle_UserNotFollowBox_ThrowException()
        {
            long someUserId = 1L, someboxid = 2;
            var someBoxTabId = Guid.NewGuid();
            //var someUser = new User("some email", "some name", "some image", "some large image");

            //someUser.GetType().GetProperty("Id").SetValue(someUser, 2L);

            //var someBoxTab = new BoxTab(someBoxTabId, "some name", someUser);

            var someCommand = new DeleteItemTabCommand(someUserId, someBoxTabId, someboxid);

            //m_StubBoxTabRepository.Stub(x => x.Get(someBoxTabId)).Return(someBoxTab);

            var handler = new DeleteItemTabCommandHandler(m_StubItemTabRepository, m_StubUserRepository);
            handler.Handle(someCommand);
        }

        [TestMethod]
        public void Handle_InputOk_Delete()
        {
            long someUserId = 0, someBoxId = 1;
            var someItemTabId = Guid.NewGuid();
            var someUser = new User("some email", " some small image", "some largeImage", "some first name", "some middle name", "some last name", true, false, "en-US"); 
            var someBox = new Box("soem box name", someUser, Infrastructure.Enums.BoxPrivacySettings.MembersOnly);
            someBox.GetType().GetProperty("Id").SetValue(someBox, someBoxId);
            var someCommand = new DeleteItemTabCommand(someUserId, someItemTabId, someBoxId);

            var someItemTab = new ItemTab(someItemTabId, "some name", someBox);

            m_StubItemTabRepository.Stub(x => x.Get(someItemTabId)).Return(someItemTab);
            m_StubUserRepository.Stub(x => x.GetUserToBoxRelationShipType(someUserId, someBoxId)).Return(Infrastructure.Enums.UserRelationshipType.Owner);

            var handler = new DeleteItemTabCommandHandler(m_StubItemTabRepository, m_StubUserRepository);
            handler.Handle(someCommand);

            m_StubItemTabRepository.AssertWasCalled(x => x.Delete(someItemTab));
        }
    }
}
