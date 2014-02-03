using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlersTests
{
    [TestClass]
    public class RenameNodeCommandHandlerTests
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
            var handler = new RenameNodeCommandHandler(m_StubLibraryRepository);
            handler.Handle(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_EmptyName_ThrowException()
        {
            var someCommand = new RenameNodeCommand(string.Empty, Guid.NewGuid(), 1L);
            var handler = new RenameNodeCommandHandler(m_StubLibraryRepository);
            handler.Handle(someCommand);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_NodeDoesntExits_ThrowException()
        {
            var someCommand = new RenameNodeCommand("some name", Guid.NewGuid(), 1L);
            var handler = new RenameNodeCommandHandler(m_StubLibraryRepository);
            handler.Handle(someCommand);
        }

        [TestMethod]
        [ExpectedException(typeof (UnauthorizedAccessException))]
        public void Handle_NodeNotConnectToUniversity_ThrowException()
        {
            var someLibraryId = Guid.NewGuid();
            var someCommand = new RenameNodeCommand("some new name", someLibraryId, 1L);
            var someUniversity = new University("some name", "some image", "some largeimage", "some User name");
            someUniversity.GetType().GetProperty("Id").SetValue(someUniversity, 1L);

            var someOtherUniversity = new University("some name", "some image", "some largeimage", "some User name2");
            someOtherUniversity.GetType().GetProperty("Id").SetValue(someOtherUniversity, 2L);
            var someLibraryNode = new Library(someLibraryId, "some name", null, someOtherUniversity);

            var handler = new RenameNodeCommandHandler(m_StubLibraryRepository);
            m_StubLibraryRepository.Stub(x => x.Get(someLibraryId)).Return(someLibraryNode);

            handler.Handle(someCommand);
        }

        [TestMethod]
        public void Handle_InputOk_Save()
        {
            var someLibraryId = Guid.NewGuid();
            var someCommand = new RenameNodeCommand("some new name", someLibraryId, 1L);
            var someUniversity = new University("some name", "some image", "some largeimage", "some User name");
            someUniversity.GetType().GetProperty("Id").SetValue(someUniversity, 1L);

            var someLibraryNode = new Library(someLibraryId, "some name", null, someUniversity);

            var handler = new RenameNodeCommandHandler(m_StubLibraryRepository);
            m_StubLibraryRepository.Stub(x => x.Get(someLibraryId)).Return(someLibraryNode);

            handler.Handle(someCommand);

            m_StubLibraryRepository.AssertWasCalled(x => x.Save(someLibraryNode));
        }
    }
}
