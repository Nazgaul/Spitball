using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Domain.Commands;

namespace Zbang.Zbox.Domain.CommandHandlersTests
{
    [TestClass]
    public class ChangeItemTabNameCommandHandlerTests
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
            var handler = new ChangeItemTabNameCommandHandler(m_StubItemTabRepository, m_StubUserRepository);
            handler.Handle(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_TabNotExists_ThrowException()
        {
            var tabId = Guid.NewGuid();
            var someCommand = new ChangeItemTabNameCommand(tabId, "some name", 1, 1);

            m_StubUserRepository.Stub(x => x.GetUserToBoxRelationShipType(1, 1)).Return(Infrastructure.Enums.UserRelationshipType.Owner);

            var handler = new ChangeItemTabNameCommandHandler(m_StubItemTabRepository, m_StubUserRepository);
            handler.Handle(someCommand);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void Handle_UserNotFollowBox_ThrowException()
        {
            var tabId = Guid.NewGuid();
            var someUser = new User("some email", " some small image", "some largeImage", "some first name", "some middle name", "some last name", true, false); 
            var someCommand = new ChangeItemTabNameCommand(tabId, "some name", 2,1);

            //var someTab = new BoxTab(tabId, "some name", someUser);

            //someUser.GetType().GetProperty("Id").SetValue(someUser, 1);

            //m_StubUserRepository.Stub(x => x.GetUserToBoxRelationShipType(1, 1)).Return(Infrastructure.Enums.UserRelationshipType.None);


            var handler = new ChangeItemTabNameCommandHandler(m_StubItemTabRepository, m_StubUserRepository);
            handler.Handle(someCommand);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Handle_RenameToAlreadyBoxWithTheSameName_ThrowException()
        {
            var tabId = Guid.NewGuid();
            long someUserId = 2L, someBoxId = 3L;
            var someName = "some name";
            var someUser = new User("some email", " some small image", "some largeImage", "some first name", "some middle name", "some last name", true, false); 
            var someBox = new Box("some box name", someUser, Infrastructure.Enums.BoxPrivacySettings.MembersOnly);
            someUser.GetType().GetProperty("Id").SetValue(someUser, someUserId);
            someBox.GetType().GetProperty("Id").SetValue(someBox, someBoxId);

            var someOtherTab = new ItemTab(Guid.NewGuid(), someName, someBox);
            var someTab = new ItemTab(tabId, someName, someBox);

            var someCommand = new ChangeItemTabNameCommand(tabId, someName, someUserId, someBoxId);

            m_StubUserRepository.Stub(x => x.GetUserToBoxRelationShipType(someUserId, someBoxId)).Return(Infrastructure.Enums.UserRelationshipType.Owner);
            
            m_StubItemTabRepository.Stub(x => x.GetTabWithTheSameName(someName, someUserId)).Return(someOtherTab);
            m_StubItemTabRepository.Stub(x => x.Get(tabId)).Return(someTab);

            var handler = new ChangeItemTabNameCommandHandler(m_StubItemTabRepository, m_StubUserRepository);
            handler.Handle(someCommand);

        }

        [TestMethod]
        public void Handle_InputOk_Save()
        {
            var tabId = Guid.NewGuid();
            var someBoxId = 1L;
            var someUser = new User("some email", " some small image", "some largeImage", "some first name", "some middle name", "some last name", true, false); 
            var someCommand = new ChangeItemTabNameCommand(tabId, "some name", 1L,someBoxId);
            var someBox = new Box("some box name", someUser, Infrastructure.Enums.BoxPrivacySettings.MembersOnly);

            var someTab = new ItemTab(tabId, "some name", someBox);

            someBox.GetType().GetProperty("Id").SetValue(someBox, someBoxId);
            m_StubUserRepository.Stub(x => x.GetUserToBoxRelationShipType(1, 1)).Return(Infrastructure.Enums.UserRelationshipType.Owner);

            m_StubItemTabRepository.Stub(x => x.Get(tabId)).Return(someTab);

            var handler = new ChangeItemTabNameCommandHandler(m_StubItemTabRepository, m_StubUserRepository);
            handler.Handle(someCommand);

            m_StubItemTabRepository.AssertWasCalled(x => x.Save(someTab));
        }

       
    }
}
