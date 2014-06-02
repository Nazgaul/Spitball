using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Zbox.Domain.CommandHandlers;
using Rhino.Mocks;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Enums;
using System.Collections.Generic;

namespace Zbang.Zbox.Domain.CommandHandlersTests
{
    [TestClass]
    public class UnitTest1
    {
        private IUserRepository m_StubUserRepository;
        private IItemTabRepository m_StubItemTabRepository;
        private IRepository<Item> m_StubItemRepository;

        [TestInitialize]
        public void Setup()
        {
            m_StubUserRepository = MockRepository.GenerateStub<IUserRepository>();
            m_StubItemTabRepository = MockRepository.GenerateStub<IItemTabRepository>();
            m_StubItemRepository = MockRepository.GenerateStub<IRepository<Item>>();
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_MessageNull_ThrowException()
        {
            var handler = new AssignItemToTabCommandHandler(m_StubUserRepository, m_StubItemTabRepository, m_StubItemRepository);
            handler.Handle(null);
        }



        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_ItemNotExists_ThrowException()
        {
            long boxid = 1;
            long userid = 1;
            var command = new AssignItemToTabCommand(new List<long>() { 1 }, Guid.NewGuid(), boxid, userid, false);
            m_StubUserRepository.Stub(x => x.GetUserToBoxRelationShipType(userid, boxid)).Return(UserRelationshipType.Owner);

            var handler = new AssignItemToTabCommandHandler(m_StubUserRepository, m_StubItemTabRepository, m_StubItemRepository);
            handler.Handle(command);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_ItemTabNotExists_ThrowException()
        {
            Guid ItemTab = Guid.NewGuid();
            long someUserId = 1, someBoxId = 1;
            var someItemId = new List<long>() { 1 };
            var someUser = new User("some email", " some small image", "some largeImage", "some first name", "some middle name", "some last name", true, false); 
            var someBox = new Box("some box name", someUser, Infrastructure.Enums.BoxPrivacySettings.MembersOnly);
            var someItem = new Link("some name", someUser, 1, someBox, "some name", "some thumbnail", "some thumbnailUrl");

            var command = new AssignItemToTabCommand(someItemId, ItemTab, someUserId, someBoxId, true);
            m_StubItemRepository.Stub(x => x.Get(someItemId)).Return(someItem);
            m_StubUserRepository.Stub(x => x.GetUserToBoxRelationShipType(someUserId, someBoxId)).Return(UserRelationshipType.Owner);



            var handler = new AssignItemToTabCommandHandler(m_StubUserRepository, m_StubItemTabRepository, m_StubItemRepository);
            handler.Handle(command);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void Handle_UserNotFollowBox_ThrowException()
        {
            Guid itemTabId = Guid.NewGuid();
            var someUser = new User("some email", " some small image", "some largeImage", "some first name", "some middle name", "some last name", true, false); 
            var someBox = new Box("some name", someUser, Infrastructure.Enums.BoxPrivacySettings.MembersOnly);
            var someItemTab = new ItemTab(itemTabId, "some name", someBox);

            var command = new AssignItemToTabCommand(new List<long>() { 1 }, itemTabId, 1, 1, true);
            m_StubUserRepository.Stub(x => x.GetUserToBoxRelationShipType(1, 1)).Return(UserRelationshipType.Invite);

            var handler = new AssignItemToTabCommandHandler(m_StubUserRepository, m_StubItemTabRepository, m_StubItemRepository);
            handler.Handle(command);
        }

        [TestMethod]
        public void Handle_InputOk_Saves()
        {
            Guid ItemTab = Guid.NewGuid();
            var command = new AssignItemToTabCommand(new List<long>() { 1 }, ItemTab, 1, 1, false);
            var someUser = new User("some email", " some small image", "some largeImage", "some first name", "some middle name", "some last name", true, false); 
            var someBox = new Box("some name", someUser, Infrastructure.Enums.BoxPrivacySettings.MembersOnly);
            var someItem = new Link("some name", someUser, 1, someBox, "some name", "some thumbnail", "some img url");


            var someItemTab = new ItemTab(ItemTab, "some name", someBox);
            //var someUserBoxRel = new UserBoxRel(someUser, someBox, Infrastructure.Enums.UserRelationshipType.Owner);

            m_StubItemTabRepository.Stub(x => x.Get(ItemTab)).Return(someItemTab);
            m_StubItemRepository.Stub(x => x.Get(1L)).Return(someItem);
            m_StubUserRepository.Stub(x => x.GetUserToBoxRelationShipType(1, 1)).Return(UserRelationshipType.Owner);


            var handler = new AssignItemToTabCommandHandler(m_StubUserRepository, m_StubItemTabRepository, m_StubItemRepository);
            handler.Handle(command);

            m_StubItemTabRepository.AssertWasCalled(x => x.Save(someItemTab));
        }
    }
}
