using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Repositories;
using Rhino.Mocks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Domain.CommandHandlersTests
{
    [TestClass]
    public class RateItemCommandHandlerTests
    {

        private IItemRateRepository m_StubItemRateRepositoy;
        private IRepository<Item> m_StubItemRepository;
        private IUserRepository m_StubUserRepository;
        private IRepository<Reputation> m_StubReputationRepository;
        private IQueueProvider m_QueueProvider;

        [TestInitialize]
        public void Setup()
        {
            m_StubItemRateRepositoy = MockRepository.GenerateStub<IItemRateRepository>();
            m_StubItemRepository = MockRepository.GenerateStub<IRepository<Item>>();
            m_StubUserRepository = MockRepository.GenerateStub<IUserRepository>();
            m_StubReputationRepository = MockRepository.GenerateStub<IRepository<Reputation>>();
            m_QueueProvider = MockRepository.GenerateStub<IQueueProvider>();


        }


        [TestMethod]
        public async Task Handle_RateItem3_SaveWithReputation5()
        {
            long itemid = 1, userid = 2, boxid = 3;
            var command = new RateItemCommand(itemid, userid, Guid.NewGuid(), boxid);


            m_StubItemRateRepositoy.Stub(x => x.GetRateOfUser(userid, itemid)).Return(null);

            var user = new User("some email", "some largeImage", "some first name", "some last name", "en-US");
            user.GetType().GetProperty("Id").SetValue(user, userid);
            var item = new Link("some name", user, 1, new Box("some box", user, Infrastructure.Enums.BoxPrivacySettings.MembersOnly, Guid.NewGuid()), "some url", "some thumbnail", "some img url");

            m_StubItemRepository.Stub(x => x.Load(itemid)).Return(item);
            m_StubUserRepository.Stub(x => x.Load(userid)).Return(user);

            var commandHandler = new RateItemCommandHandler(m_StubItemRateRepositoy,
                m_StubItemRepository, m_StubUserRepository, m_QueueProvider);



            await commandHandler.HandleAsync(command);

            Assert.AreEqual(5, item.Uploader.Reputation);
        }

        [TestMethod]
        public async Task Handle_ReRateItem5_SaveWithReputation20()
        {
            long itemid = 1, userid = 2, boxid = 3;
            var command = new RateItemCommand(itemid, userid, Guid.NewGuid(), boxid);




            var user = new User("some email", "some largeImage", "some first name", "some last name", "en-US");
            user.GetType().GetProperty("Id").SetValue(user, userid);
            user.Reputation = 5;
            var item = new Link("some name", user, 1, new Box("some box", user, Infrastructure.Enums.BoxPrivacySettings.MembersOnly, Guid.NewGuid()), "some url", "some thumbnail", "some img url");

            m_StubItemRepository.Stub(x => x.Load(itemid)).Return(item);
            m_StubUserRepository.Stub(x => x.Load(userid)).Return(user);
            m_StubItemRateRepositoy.Stub(x => x.GetRateOfUser(userid, itemid)).Return(new ItemRate(user, item, Guid.NewGuid()));
            var commandHandler = new RateItemCommandHandler(m_StubItemRateRepositoy, m_StubItemRepository,
                m_StubUserRepository, m_QueueProvider);



            await commandHandler.HandleAsync(command);

            Assert.AreEqual(20, item.Uploader.Reputation);
        }
    }
}
