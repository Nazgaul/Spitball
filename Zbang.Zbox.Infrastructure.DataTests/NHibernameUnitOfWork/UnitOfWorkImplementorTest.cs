using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.UnitsOfWork;
using NHibernate;
using System.Data;

namespace Zbang.Zbox.Infrastructure.DataTests.NHibernameUnitOfWork
{
    [TestClass]
    public class UnitOfWorkImplementorTest
    {
        private readonly MockRepository _mocks = new MockRepository();
        private IUnitOfWorkFactory _factory;
        private ISession _session;
        private IUnitOfWorkImplementor _uow;

        [TestInitialize]
        public void SetupContext()
        {
            _factory = _mocks.DynamicMock<IUnitOfWorkFactory>();
            _session = _mocks.DynamicMock<ISession>();
        }

        [TestMethod]
        public void Can_create_UnitOfWorkImplementor()
        {
            using (_mocks.Record())
            {

            }
            using (_mocks.Playback())
            {
                _uow = new UnitOfWorkImplementor(_factory, _session);
                Assert.AreSame(_factory, ((UnitOfWorkImplementor)_uow).Factory);
                Assert.AreSame(_session, ((UnitOfWorkImplementor)_uow).Session);
            }
        }

        [TestMethod]
        public void Can_Dispose_UnitOfWorkImplementor()
        {
            using (_mocks.Record())
            {
                Expect.Call(() => _factory.DisposeUnitOfWork(null)).IgnoreArguments();
                Expect.Call(_session.Dispose);
            }
            using (_mocks.Playback())
            {
                _uow = new UnitOfWorkImplementor(_factory, _session);
                _uow.Dispose();
            }
        }

        [TestMethod]
        public void Can_Flush_UnitOfWorkImplementor()
        {
            using (_mocks.Record())
            {
                Expect.Call(_session.Flush);
            }
            using (_mocks.Playback())
            {
                _uow = new UnitOfWorkImplementor(_factory, _session);
                _uow.Flush();
            }
        }

        [TestMethod]
        public void Can_BeginTransaction()
        {
            using (_mocks.Record())
            {
                Expect.Call(_session.BeginTransaction()).Return(null);
            }
            using (_mocks.Playback())
            {
                _uow = new UnitOfWorkImplementor(_factory, _session);
                var transaction = _uow.BeginTransaction();
                Assert.IsNotNull(transaction);
            }
        }

        [TestMethod]
        public void Can_BeginTransaction_specifying_isolation_level()
        {
            var isolationLevel = IsolationLevel.Serializable;
            using (_mocks.Record())
            {
                Expect.Call(_session.BeginTransaction(isolationLevel)).Return(null);
            }
            using (_mocks.Playback())
            {
                _uow = new UnitOfWorkImplementor(_factory, _session);
                var transaction = _uow.BeginTransaction(isolationLevel);
                Assert.IsNotNull(transaction);
            }
        }

        [TestMethod]
        public void Can_execute_TransactionalFlush()
        {
            var tx = _mocks.StrictMock<ITransaction>();// _mocks.CreateMock<ITransaction>();
            var session = _mocks.DynamicMock<ISession>();
            SetupResult.For(session.BeginTransaction(IsolationLevel.ReadCommitted)).Return(tx);

            _uow = _mocks.PartialMock<UnitOfWorkImplementor>(_factory, _session);

            using (_mocks.Record())
            {
                Expect.Call(tx.Commit);
                Expect.Call(tx.Dispose);
            }
            using (_mocks.Playback())
            {
                _uow = new UnitOfWorkImplementor(_factory, session);
                _uow.TransactionalFlush();
            }
        }

        [TestMethod]
        public void Can_execute_TransactionalFlush_specifying_isolation_level()
        {

            var tx = _mocks.StrictMock<ITransaction>();// _mocks.CreateMock<ITransaction>();
            var session = _mocks.DynamicMock<ISession>();
            SetupResult.For(session.BeginTransaction(IsolationLevel.Serializable)).Return(tx);

            _uow = _mocks.PartialMock<UnitOfWorkImplementor>(_factory, session);

            using (_mocks.Record())
            {
                Expect.Call(tx.Commit);
                Expect.Call(tx.Dispose);
            }
            using (_mocks.Playback())
            {
                _uow.TransactionalFlush(IsolationLevel.Serializable);
            }
        }
    }
}
