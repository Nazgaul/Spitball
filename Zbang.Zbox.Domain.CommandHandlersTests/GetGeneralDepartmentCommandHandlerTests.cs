using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Zbox.Domain.CommandHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Repositories;
using Rhino.Mocks;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Domain.CommandHandlers.Tests
{
    [TestClass()]
    public class GetGeneralDepartmentCommandHandlerTests
    {
        private IRepository<Library> m_department;
        private IRepository<User> m_user;
        private IGuidIdGenerator m_IdGenerator;


        [TestInitialize]
        public void Setup()
        {
            var localStorageProvider = MockRepository.GenerateStub<ILocalStorageProvider>();

            IocFactory.IocWrapper.RegisterInstance(localStorageProvider);
            m_department = MockRepository.GenerateMock<IRepository<Library>>();
            m_user = MockRepository.GenerateMock<IRepository<User>>();
            m_IdGenerator = MockRepository.GenerateMock<IGuidIdGenerator>();
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExecuteAsyncTest()
        {
            var commandHandler = new GetGeneralDepartmentCommandHandler(m_department,m_user,m_IdGenerator);
            
            var res = commandHandler.Execute(null);
        }
    }
}