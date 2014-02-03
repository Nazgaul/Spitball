using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Zbox.Infrastructure.UnitsOfWork;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
using NHibernate;
using Rhino.Mocks;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.DataTests.NHibernameUnitOfWork
{
    [TestClass]
    public class UnitOfWorkFactoryTest
    {
        private IUnitOfWorkFactory _factory;

        [TestInitialize]
        public void SetupContext()
        {
            var m_LocalStorageProvider = MockRepository.GenerateStub<ILocalStorageProvider>();
            Ioc.IocFactory.Unity.RegisterInstance<ILocalStorageProvider>(m_LocalStorageProvider);
            _factory = (IUnitOfWorkFactory)Activator.CreateInstance(typeof(UnitOfWorkFactory), true);
        }

        [TestMethod]
        public void Can_create_unit_of_work()
        {
            IUnitOfWork implementor = _factory.Create();
            Assert.IsNotNull(implementor);
            Assert.IsNotNull(_factory.CurrentSession);
            Assert.AreEqual(FlushMode.Commit, _factory.CurrentSession.FlushMode);
        }

        [TestMethod]
        public void Can_configure_NHibernate()
        {
            var configuration = _factory.Configuration;
            Assert.IsNotNull(configuration);
            Assert.AreEqual("NHibernate.Connection.DriverConnectionProvider", configuration.Properties["connection.provider"]);
            //Assert.AreEqual("Zbang.Zbox.Infrastructure.Data.Dialect.ZboxDialect, Zbang.Zbox.Infrastructure.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
            //                configuration.Properties["dialect"]);
            //Assert.AreEqual("Zbang.Zbox.Infrastructure.Data.Driver.ZboxDriver, Zbang.Zbox.Infrastructure.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
            //                configuration.Properties["connection.driver_class"]);
            //Assert.AreEqual("Server=(local);Database=Test;Integrated Security=SSPI;",
            //                configuration.Properties["connection.connection_string"]);
        }

        [TestMethod]
        public void Can_create_and_access_session_factory()
        {
            var sessionFactory = _factory.SessionFactory;
            Assert.IsNotNull(sessionFactory);
            //Assert.AreEqual("NHibernate.Dialect.MsSql2005Dialect", sessionFactory.Dialect.ToString());
        }

        [TestMethod]
        public void Accessing_CurrentSession_when_no_session_open_throws()
        {
            try
            {
                var session = _factory.CurrentSession;
            }
            catch (InvalidOperationException)
            { }
        }
    }
}
