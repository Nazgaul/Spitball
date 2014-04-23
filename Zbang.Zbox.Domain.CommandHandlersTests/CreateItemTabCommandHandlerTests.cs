using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Zbox.Domain.DataAccess;
using Rhino.Mocks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlersTests
{
    [TestClass]
    public class CreateItemTabCommandHandlerTests
    {
        private IItemTabRepository m_StubItemTabRepository;
        private IBoxRepository m_StubBoxRepository;
        private IUserRepository m_UserRepository;

        [TestInitialize]
        public void Setup()
        {
            m_StubItemTabRepository = MockRepository.GenerateStub<IItemTabRepository>();
            m_StubBoxRepository = MockRepository.GenerateStub<IBoxRepository>();
            m_UserRepository = MockRepository.GenerateStub<IUserRepository>();
        }

        [TestMethod]
        public void Handle_CommandOk_Saves()
        {
            var guid = Guid.NewGuid();
            long someUserId = 1, someBoxId = 1;
            var command = new CreateItemTabCommand(guid, "test", someUserId, someBoxId);
            var theUser = new User("some email", " some small image", "some largeImage", "some first name", "some middle name", "some last name", true, false);
            var theBox = new Box("some name", theUser, Infrastructure.Enums.BoxPrivacySettings.MembersOnly);

            m_UserRepository.Stub(x => x.GetUserToBoxRelationShipType(someUserId, someBoxId)).Return(Infrastructure.Enums.UserRelationshipType.Owner);
            m_StubBoxRepository.Stub(x => x.Get(someBoxId)).Return(theBox);

            var dummy = new ItemTab(guid, "test", theBox);

            var handler = new CreateItemTabCommandHandler(m_StubItemTabRepository, m_StubBoxRepository, m_UserRepository);

            handler.Handle(command);

            m_StubItemTabRepository.AssertWasCalled(x => x.Save(dummy));
        }



        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void Handle_UserNotFollow_ThrowException()
        {
            var guid = Guid.NewGuid();
            long someUserId = 1, someBoxId = 1;
            var command = new CreateItemTabCommand(guid, "test", someBoxId, someUserId);
            m_UserRepository.Stub(x => x.GetUserToBoxRelationShipType(someUserId, someBoxId)).Return(Infrastructure.Enums.UserRelationshipType.None);


            var handler = new CreateItemTabCommandHandler(m_StubItemTabRepository, m_StubBoxRepository, m_UserRepository);
            handler.Handle(command);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_BoxNull_ThrowException()
        {
            var guid = Guid.NewGuid();
            long someBoxId = 1, someUserId = 1;
            var command = new CreateItemTabCommand(guid, "test", someBoxId, someUserId);
            m_StubBoxRepository.Stub(x => x.Get(someBoxId)).Return(null);
            m_UserRepository.Stub(x => x.GetUserToBoxRelationShipType(someUserId, someBoxId)).Return(Infrastructure.Enums.UserRelationshipType.Owner);


            var handler = new CreateItemTabCommandHandler(m_StubItemTabRepository, m_StubBoxRepository, m_UserRepository);
            handler.Handle(command);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Handle_DuplicateTabName_ThrowException()
        {
            var guid = Guid.NewGuid();
            long boxId = 1, userId = 1;
            var duplicateTabName = "Some name";
            var someUser = new User("some email", " some small image", "some largeImage", "some first name", "some middle name", "some last name", true, false);
            var someBox = new Box("some name", someUser, Infrastructure.Enums.BoxPrivacySettings.MembersOnly);
            someBox.GetType().GetProperty("Id").SetValue(someBox, boxId);

            var boxTab = new ItemTab(Guid.NewGuid(), duplicateTabName, someBox);

            m_StubBoxRepository.Stub(x => x.Get(boxId)).Return(someBox);
            var command = new CreateItemTabCommand(guid, duplicateTabName, boxId, userId);

            m_StubItemTabRepository.Stub(x => x.GetTabWithTheSameName(command.Name, 1L)).Return(boxTab);
            m_UserRepository.Stub(x => x.GetUserToBoxRelationShipType(userId, boxId)).Return(Infrastructure.Enums.UserRelationshipType.Owner);


            var handler = new CreateItemTabCommandHandler(m_StubItemTabRepository, m_StubBoxRepository, m_UserRepository);
            handler.Handle(command);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_TabNameEmpty_ThrowException()
        {
            var guid = Guid.NewGuid();
            long boxId = 1;
            var duplicateTabName = string.Empty;
            var someUser = new User("some email", " some small image", "some largeImage", "some first name", "some middle name", "some last name", true, false);
            var someBox = new Box("some name", someUser, Infrastructure.Enums.BoxPrivacySettings.MembersOnly);


            m_StubBoxRepository.Stub(x => x.Get(boxId)).Return(someBox);
            var command = new CreateItemTabCommand(guid, duplicateTabName, boxId, 1);

            var handler = new CreateItemTabCommandHandler(m_StubItemTabRepository, m_StubBoxRepository, m_UserRepository);
            handler.Handle(command);
        }
    }
}
