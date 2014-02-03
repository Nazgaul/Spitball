using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.CommandHandlers;
using Rhino.Mocks;

namespace Zbang.Zbox.Domain.CommandHandlersTests
{
    [TestClass]
    public class SubscribeToSharedBoxCommandHandlerTests
    {
        private IRepository<Box> m_StubBoxRepository;
        private IUserRepository m_StubUserRepository;
        private IRepository<UserBoxRel> m_StubUserBoxRelRepository;
        private IQueueProvider m_StubQueueProvider;

        [TestInitialize]
        public void Setup()
        {
            m_StubBoxRepository = MockRepository.GenerateStub<IRepository<Box>>();
            m_StubUserRepository = MockRepository.GenerateStub<IUserRepository>();
            m_StubUserBoxRelRepository = MockRepository.GenerateStub<IRepository<UserBoxRel>>();
            m_StubQueueProvider = MockRepository.GenerateStub<IQueueProvider>();
        }

        //[TestMethod]
        //public void Handle_UserSubscribeToBoxWhenNotInvited_UpdateUserTimeUpdated()
        //{

        //    long someUserId = 1, someBoxId = 2;
        //    var someUser = new User("some email", "some name", "smllimg", "largeimg");
        //    var someUser2 = new User("some email2", "some name2", "smllimg2", "largeimg2");
        //    var someBox = new Box("some box", someUser2);
        //    someBox.PrivacySettings.PrivacySetting = Infrastructure.Enums.BoxPrivacySettings.AnyoneWithUrl;

        //    var someCommand = new SubscribeToSharedBoxCommand(someUserId, someBoxId);

        //    m_StubUserRepository.Stub(x => x.Get(someUserId)).Return(someUser);
        //    m_StubUserRepository.Stub(x => x.GetUserToBoxRelationShipType(someUserId, someBoxId)).Return(Infrastructure.Enums.UserRelationshipType.None);
        //    m_StubBoxRepository.Stub(x => x.Get(someBoxId)).Return(someBox);


        //    someUser.GetType().GetProperty("Id").SetValue(someUser, someUserId);
        //    someBox.GetType().GetProperty("Id").SetValue(someBox, someBoxId);

        //    var handler = new SubscribeToSharedBoxCommandHandler(m_StubBoxRepository, m_StubUserRepository, m_StubUserBoxRelRepository, m_StubQueueProvider);
        //    handler.Handle(someCommand);


           

        //    m_StubUserRepository.AssertWasCalled(x=>x.Save(
        //}
    }
}
