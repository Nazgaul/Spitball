using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Domain.CommandHandlersTests
{
    [TestClass]
    public class UpdateThumbnailCommandHandlerTests
    {
        private IRepository<File> m_StubItemRepository;
        private IRepository<Box> m_StubBoxRepository;
        private IBlobProvider m_BlobProvider;

        [TestInitialize]
        public void Setup()
        {
            m_StubItemRepository = MockRepository.GenerateStub<IRepository<File>>();
            m_StubBoxRepository = MockRepository.GenerateStub<IRepository<Box>>();
            m_BlobProvider = MockRepository.GenerateStub<IBlobProvider>();

        }

        

       
    }
}
