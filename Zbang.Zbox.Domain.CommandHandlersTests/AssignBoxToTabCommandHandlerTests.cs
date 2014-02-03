using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;

namespace Zbang.Zbox.Domain.CommandHandlersTests
{
    [TestClass]
    public class AssignBoxToTabCommandHandlerTests
    {
        private IBoxTabRepository m_StubBoxTabRepository;
        private IUserBoxRelRepository m_StubUserBoxRelRepository;

        [TestInitialize]
        public void Setup()
        {
            m_StubBoxTabRepository = MockRepository.GenerateStub<IBoxTabRepository>();
            m_StubUserBoxRelRepository = MockRepository.GenerateStub<IUserBoxRelRepository>();
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_MessageNull_ThrowException()
        {
            var handler = new AssignBoxToTabCommandHandler(m_StubUserBoxRelRepository, m_StubBoxTabRepository);
            handler.Handle(null);
        }



        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_UserBoxRelNotExists_ThrowException()
        {
            long boxid = 1;
            long userid = 1;
            var command = new AssignBoxToTabCommand(boxid, Guid.NewGuid(), userid);
            m_StubUserBoxRelRepository.Stub(x => x.GetUserBoxRelationship(userid, boxid)).Return(null);

            var handler = new AssignBoxToTabCommandHandler(m_StubUserBoxRelRepository, m_StubBoxTabRepository);
            handler.Handle(command);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_BoxTabNotExists_ThrowException()
        {
            Guid boxTab = Guid.NewGuid();
            var command = new AssignBoxToTabCommand(1, boxTab, 1);
            m_StubBoxTabRepository.Stub(x => x.Get(boxTab)).Return(null);

            var handler = new AssignBoxToTabCommandHandler(m_StubUserBoxRelRepository, m_StubBoxTabRepository);
            handler.Handle(command);
        }

        [TestMethod]
        [ExpectedException(typeof (UnauthorizedAccessException))]
        public void Handle_BoxNotConnectedToUser_ThrowException()
        {
            Guid boxTabId = Guid.NewGuid();
            var someUser = new User("some email", "some user name", "some image", "some large image");
            var someBoxTab = new BoxTab(boxTabId, "some name", someUser);
            var someBox = new Box("some name", someUser);
            var someUserBoxRel = new UserBoxRel(someUser, someBox, Infrastructure.Enums.UserRelationshipType.Invite);

            var command = new AssignBoxToTabCommand(1, boxTabId, 1);

            m_StubBoxTabRepository.Stub(x => x.Get(boxTabId)).Return(someBoxTab);
            m_StubUserBoxRelRepository.Stub(x => x.GetUserBoxRelationship(1, 1)).Return(someUserBoxRel);

            var handler = new AssignBoxToTabCommandHandler(m_StubUserBoxRelRepository, m_StubBoxTabRepository);
            handler.Handle(command);
        }

        [TestMethod]
        public void Handle_InputOk_Saves()
        {
            Guid boxTab = Guid.NewGuid();
            var command = new AssignBoxToTabCommand(1, boxTab, 1);
            var someUser = new User("some email", "some user name", "some image", "some large image");
            var someBoxTab = new BoxTab(boxTab, "some name", someUser);
            var someBox = new Box("some name", someUser);
            var someUserBoxRel = new UserBoxRel(someUser, someBox, Infrastructure.Enums.UserRelationshipType.Owner);

            m_StubBoxTabRepository.Stub(x => x.Get(boxTab)).Return(someBoxTab);
            m_StubUserBoxRelRepository.Stub(x => x.GetUserBoxRelationship(1, 1)).Return(someUserBoxRel);


            var handler = new AssignBoxToTabCommandHandler(m_StubUserBoxRelRepository, m_StubBoxTabRepository);
            handler.Handle(command);

            m_StubBoxTabRepository.AssertWasCalled(x => x.Save(someBoxTab));
        }

    }
}
