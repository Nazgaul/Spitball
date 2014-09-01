using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlersTests
{
    [TestClass]
    public class DeleteNodeFromLibraryCommandHandlerTests
    {
        private IRepository<Library> m_StubLibraryRepository;

        [TestInitialize]
        public void Setup()
        {
            m_StubLibraryRepository = MockRepository.GenerateStub<IRepository<Library>>();

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_NullInput_ThrowException()
        {
            var handler = new DeleteNodeFromLibraryCommandHandler(m_StubLibraryRepository);
            handler.Handle(null);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void Handle_NodeNotConnectToUniversity_ThrowException()
        {
            var someLibraryId = Guid.NewGuid();
            var someCommand = new DeleteNodeFromLibraryCommand(someLibraryId, 1L);
            var someUniversity = new University2("some name", "some image", "some largeimage", "some User name");
            someUniversity.GetType().GetProperty("Id").SetValue(someUniversity, 1L);

            var someOtherUniversity = new University2("some name", "some image", "some largeimage", "some User name2");
            someOtherUniversity.GetType().GetProperty("Id").SetValue(someOtherUniversity, 2L);
            var someLibraryNode = new Library(someLibraryId, "some name", null, someOtherUniversity);

            var handler = new DeleteNodeFromLibraryCommandHandler(m_StubLibraryRepository);
            m_StubLibraryRepository.Stub(x => x.Get(someLibraryId)).Return(someLibraryNode);

            handler.Handle(someCommand);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_NodeNotExits_ThrowException()
        {
            var someCommand = new DeleteNodeFromLibraryCommand(Guid.NewGuid(), 1L);
            var handler = new DeleteNodeFromLibraryCommandHandler(m_StubLibraryRepository);
            handler.Handle(someCommand);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void Handle_NodeGotChildren_ThrowException()
        {
            var someLibraryId = Guid.NewGuid();
            var someCommand = new DeleteNodeFromLibraryCommand(someLibraryId, 1L);

            var someUniversity = new University2("some name", "some image", "some largeimage", "some User name");
            someUniversity.GetType().GetProperty("Id").SetValue(someUniversity, 1L);

            var someLibraryNode = new Library(someLibraryId, "some name", null, someUniversity);
            someLibraryNode.CreateSubLibrary(Guid.NewGuid(), "some name");
            m_StubLibraryRepository.Stub(x => x.Get(someLibraryId)).Return(someLibraryNode);

            var handler = new DeleteNodeFromLibraryCommandHandler(m_StubLibraryRepository);
            handler.Handle(someCommand);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void Handle_NodeGotBoxes_ThrowException()
        {
            var someLibraryId = Guid.NewGuid();
            var someCommand = new DeleteNodeFromLibraryCommand(someLibraryId, 1L);

            var someUniversity = new University2("some name", "some image", "some largeimage", "some User name");
            someUniversity.GetType().GetProperty("Id").SetValue(someUniversity, 1L);

            var someLibraryNode = new Library(someLibraryId, "some name", null, someUniversity);
            someLibraryNode.Boxes.Add(new Box("some box name", someUniversity, Infrastructure.Enums.BoxPrivacySettings.MembersOnly));

            m_StubLibraryRepository.Stub(x => x.Get(someLibraryId)).Return(someLibraryNode);

            var handler = new DeleteNodeFromLibraryCommandHandler(m_StubLibraryRepository);
            handler.Handle(someCommand);
        }

        [TestMethod]
        public void Handle_InputOk_Save()
        {
            var someLibraryId = Guid.NewGuid();
            var someCommand = new DeleteNodeFromLibraryCommand(someLibraryId, 1L);
            var someUniversity = new University2("some name", "some image", "some largeimage", "some User name");
            someUniversity.GetType().GetProperty("Id").SetValue(someUniversity, 1L);

            var someLibraryNode = new Library(someLibraryId, "some name", null, someUniversity);

            var handler = new DeleteNodeFromLibraryCommandHandler(m_StubLibraryRepository);
            m_StubLibraryRepository.Stub(x => x.Get(someLibraryId)).Return(someLibraryNode);

            handler.Handle(someCommand);

            m_StubLibraryRepository.AssertWasCalled(x => x.Delete(someLibraryNode));
        }


    }
}
