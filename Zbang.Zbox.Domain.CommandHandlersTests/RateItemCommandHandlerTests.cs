using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Repositories;
using Rhino.Mocks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlersTests
{
    [TestClass]
    public class RateItemCommandHandlerTests
    {

        private IItemRateRepository m_StubItemRateRepositoy;
        private IRepository<Item> m_StubItemRepository;
        private IUserRepository m_StubUserRepository;

        [TestInitialize]
        public void Setup()
        {
            m_StubItemRateRepositoy = MockRepository.GenerateStub<IItemRateRepository>();
            m_StubItemRepository = MockRepository.GenerateStub<IRepository<Item>>();
            m_StubUserRepository = MockRepository.GenerateStub<IUserRepository>();

        }


        [TestMethod]
        public void Handle_RateItem3_SaveWithReputation5()
        {
            long itemid = 1, userid = 2;
            var command = new RateItemCommand(itemid, userid, 3, Guid.NewGuid());


            m_StubItemRateRepositoy.Stub(x => x.GetRateOfUser(userid, itemid)).Return(null);

            var user = new User("some email", "some name", "some image", "some large image");
            user.GetType().GetProperty("Id").SetValue(user, userid);
            var item = new Item("some name", user, 1, new Box("some box", user, Infrastructure.Enums.BoxPrivacySettings.MembersOnly), "some url");

            m_StubItemRepository.Stub(x => x.Load(itemid)).Return(item);
            m_StubUserRepository.Stub(x => x.Load(userid)).Return(user);

            var commandHandler = new RateItemCommandHandler(m_StubItemRateRepositoy, m_StubItemRepository, m_StubUserRepository);



            commandHandler.Handle(command);

            Assert.AreEqual(5, item.Uploader.Reputation);
        }

        [TestMethod]
        public void Handle_ReRateItem5_SaveWithReputation20()
        {
            long itemid = 1, userid = 2;
            var command = new RateItemCommand(itemid, userid, 5, Guid.NewGuid());




            var user = new User("some email", "some name", "some image", "some large image");
            user.GetType().GetProperty("Id").SetValue(user, userid);
            user.Reputation = 5;
            var item = new Item("some name", user, 1, new Box("some box", user, Infrastructure.Enums.BoxPrivacySettings.MembersOnly), "some url");

            m_StubItemRepository.Stub(x => x.Load(itemid)).Return(item);
            m_StubUserRepository.Stub(x => x.Load(userid)).Return(user);
            m_StubItemRateRepositoy.Stub(x => x.GetRateOfUser(userid, itemid)).Return(new ItemRate(user, item, Guid.NewGuid(), 3));
            var commandHandler = new RateItemCommandHandler(m_StubItemRateRepositoy, m_StubItemRepository, m_StubUserRepository);



            commandHandler.Handle(command);

            Assert.AreEqual(20, item.Uploader.Reputation);
        }
    }
}
