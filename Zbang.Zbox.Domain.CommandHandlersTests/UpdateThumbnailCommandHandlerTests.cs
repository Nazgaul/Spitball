using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System.Collections.Generic;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlersTests
{
    [TestClass]
    public class UpdateThumbnailCommandHandlerTests
    {
        private IRepository<File> m_StubItemRepository;
        private IRepository<Box> m_StubBoxRepository;

        [TestInitialize]
        public void Setup()
        {
            m_StubItemRepository = MockRepository.GenerateStub<IRepository<File>>();
            m_StubBoxRepository = MockRepository.GenerateStub<IRepository<Box>>();

        }

        [TestMethod]
        public void Handle_FirstFileInBox_UpdateBoxPicture()
        {
            var someItemId = 1L;
            var someBlobUrl = "someblobname";
            var someUploaderUser = new User("some email", " some small image", "some largeImage", "some first name", "some middle name", "some last name", true);
            var someBox = new Box("some box name", someUploaderUser, Infrastructure.Enums.BoxPrivacySettings.MembersOnly);
            var someFile = new File("some name", someUploaderUser, 1, someBlobUrl, "default first img", someBox);

            m_StubItemRepository.Stub(x => x.Get(someItemId)).Return(someFile);

            var someCommand = new UpdateThumbnailCommand(someItemId, "some new thumbnailUrl", someBlobUrl, someBlobUrl, "xxxx");
            var commandHandle = new UpdateThumbnailCommandHandler(m_StubItemRepository, m_StubBoxRepository);

            commandHandle.Handle(someCommand);

            m_StubBoxRepository.AssertWasCalled(w => w.Save(someBox));
        }

        [TestMethod]
        public void Handle_BoxHaveSeveralFiles_DontUpdateBoxPicture()
        {
            // var stubBox = MockRepository.GenerateStub<Box>();
            //var stubItemInBox = MockRepository.GenerateStub<ICollection<Item>>();
            var someItemId = 1L;
            var someBlobUrl = "someblobname";
            var someUploaderUser = new User("some email", " some small image", "some largeImage", "some first name", "some middle name", "some last name", true);
            var someBox = new Box("some box name", someUploaderUser, Infrastructure.Enums.BoxPrivacySettings.MembersOnly);

            var someFile = new File("some name", someUploaderUser, 1, someBlobUrl, "default first img", someBox);

            someBox.AddPicture("some box picture");
            // stubBox.GetType().GetProperty("Items").SetValue(stubBox, stubItemInBox);
            // stubItemInBox.Stub(x => x.Count).Return(5);

            m_StubItemRepository.Stub(x => x.Get(someItemId)).Return(someFile);

            var someCommand = new UpdateThumbnailCommand(someItemId, "some new thumbnailUrl", someBlobUrl, someBlobUrl, "xxx");
            var commandHandle = new UpdateThumbnailCommandHandler(m_StubItemRepository, m_StubBoxRepository);

            commandHandle.Handle(someCommand);

            m_StubBoxRepository.AssertWasNotCalled(w => w.Save(someBox));
        }
    }
}
