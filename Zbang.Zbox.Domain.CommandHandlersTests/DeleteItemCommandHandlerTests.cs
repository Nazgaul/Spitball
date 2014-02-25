using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Repositories;
using Rhino.Mocks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.CommandHandlers;
using System.Collections.Generic;

namespace Zbang.Zbox.Domain.CommandHandlersTests
{
    [TestClass]
    public class DeleteItemCommandHandlerTests
    {
        private IRepository<Box> m_StubBoxRepository;
        private IUserRepository m_StubUserRepository;
        private IBlobProvider m_StubBlobProvider;
        private IQueueProvider m_StubQueueProvider;
        //private IActionRepository m_StubActionRepository;
        private IRepository<Item> m_StubItemRepository;
        private IRepository<Reputation> m_StubReputationRepository;

        [TestInitialize]
        public void Setup()
        {
            m_StubBoxRepository = MockRepository.GenerateStub<IRepository<Box>>();
            m_StubUserRepository = MockRepository.GenerateStub<IUserRepository>();
            m_StubBlobProvider = MockRepository.GenerateStub<IBlobProvider>();
            m_StubQueueProvider = MockRepository.GenerateStub<IQueueProvider>();
          //  m_StubActionRepository = MockRepository.GenerateStub<IActionRepository>();
            m_StubItemRepository = MockRepository.GenerateStub<IRepository<Item>>();
            m_StubReputationRepository = MockRepository.GenerateStub<IRepository<Reputation>>();
        }

        [TestMethod]
        public void Handle_LastFileDeletedFromBox_BoxPictureRemoved()
        {
           // var stubBox = MockRepository.GenerateStub<Box>();
            ICollection<Item> itemsInBox = new List<Item>();
            //var stubFile = MockRepository.GenerateStub<File>();
            //var stubUser = MockRepository.GenerateStub<User>();
            var someUser = new User("some email", "some name", "some image", "some large image");
            var someThumbnail = "someThumbnailName";
            long someUserId = 1L, someBoxId = 2L, someItemId = 3L;
            someUser.GetType().GetProperty("Id").SetValue(someUser, someUserId);
            //stubBox.GetType().GetProperty("Id").SetValue(stubBox, someBoxId);

            var someBox = new Box("some box", someUser,Infrastructure.Enums.BoxPrivacySettings.MembersOnly);
            var someFile = new File("some ItemName", someUser, 5, "someblobName", someThumbnail, someBox);
            someFile.GetType().GetProperty("Id").SetValue(someFile, someItemId);
            var userItems = new List<Item>() { someFile };

            someBox.GetType().GetProperty("Picture").SetValue(someBox, someThumbnail);
            someBox.GetType().GetProperty("Items").SetValue(someBox, itemsInBox);
            //stubItemInBox.Stub(x=>

            var command = new DeleteItemCommand(someItemId, someUserId, someBoxId);
            var commandHandler = new DeleteItemCommandHandler(m_StubQueueProvider, m_StubBoxRepository,
                m_StubBlobProvider, m_StubUserRepository, m_StubItemRepository, m_StubReputationRepository);

            m_StubUserRepository.Stub(x => x.GetUserToBoxRelationShipType(someUserId, someBoxId)).Return(Infrastructure.Enums.UserRelationshipType.Owner);
            m_StubUserRepository.Stub(x => x.Load(someUserId)).Return(someUser);
            m_StubUserRepository.Stub(x => x.GetItemsByUser(someUserId)).Return(userItems);

            m_StubBoxRepository.Stub(x => x.Get(someBoxId)).Return(someBox);
            m_StubItemRepository.Stub(x => x.Get(someItemId)).Return(someFile);


            commandHandler.Handle(command);

            Assert.AreEqual(someBox.Picture, null);


            //stubBox.AssertWasCalled(x => x.RemovePicture());

        }
    }
}
