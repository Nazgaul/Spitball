using System;
using System.Linq;
using System.Threading.Tasks;
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
        //private IActionRepository m_StubActionRepository;
        private IRepository<Item> m_StubItemRepository;
        //private IRepository<Reputation> m_StubReputationRepository;
        private IRepository<Updates> m_StubUpdatesRepository;

        private IRepository<CommentReply> m_CommentRepliesRepository;
        private IRepository<Comment> m_CommentRepository;
        private IQueueProvider m_QueueProvider;


        [TestInitialize]
        public void Setup()
        {
            m_StubBoxRepository = MockRepository.GenerateStub<IRepository<Box>>();
            m_StubUserRepository = MockRepository.GenerateStub<IUserRepository>();
            m_StubBlobProvider = MockRepository.GenerateStub<IBlobProvider>();
            //  m_StubActionRepository = MockRepository.GenerateStub<IActionRepository>();
            m_StubItemRepository = MockRepository.GenerateStub<IRepository<Item>>();
           // m_StubReputationRepository = MockRepository.GenerateStub<IRepository<Reputation>>();
            m_StubUpdatesRepository = MockRepository.GenerateStub<IRepository<Updates>>();
            m_CommentRepliesRepository = MockRepository.GenerateStub<IRepository<CommentReply>>();
            m_CommentRepository = MockRepository.GenerateStub<IRepository<Comment>>();
            m_QueueProvider = MockRepository.GenerateStub<IQueueProvider>();
        }

    }
}
