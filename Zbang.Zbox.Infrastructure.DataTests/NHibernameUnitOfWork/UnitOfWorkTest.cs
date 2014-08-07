using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using Rhino.Mocks;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.UnitsOfWork;

namespace Zbang.Zbox.Infrastructure.DataTests.NHibernameUnitOfWork
{
    [TestClass]
    public class UnitOfWorkTest
    {
        private readonly MockRepository _mocks = new MockRepository();

        [TestInitialize]
        public void Setup()
        {
            var m_LocalStorageProvider = MockRepository.GenerateStub<ILocalStorageProvider>();
            Ioc.IocFactory.Unity.RegisterInstance<ILocalStorageProvider>(m_LocalStorageProvider);
        }

        [TestMethod]
        public void Can_Start_UnitOfWork()
        {
            var factory = _mocks.DynamicMock<IUnitOfWorkFactory>();
            var unitOfWork = _mocks.DynamicMock<IUnitOfWork>();

            // brute force attack to set my own factory via reflection
            var fieldInfo = typeof(UnitOfWork).GetField("UnitOfWorkFactory",
                BindingFlags.Static | BindingFlags.SetField | BindingFlags.NonPublic);
            fieldInfo.SetValue(null, factory);

            using (_mocks.Record())
            {
                Expect.Call(factory.Create()).Return(unitOfWork);
            }
            using (_mocks.Playback())
            {
                var uow = UnitOfWork.Start();
            }
        }
    }

    [TestClass]
    public class UnitOfWork_With_Factory_Fixture
    {
        private readonly MockRepository _mocks = new MockRepository();
        private IUnitOfWorkFactory _factory;
        private IUnitOfWork _unitOfWork;
        private ISession _session;

         [TestMethod]
        public void TestFixtureSetUp()
        {
            ResetUnitOfWork();
        }

        [TestInitialize]
        public void SetupContext()
        {
            var m_LocalStorageProvider = MockRepository.GenerateStub<ILocalStorageProvider>();
            Ioc.IocFactory.Unity.RegisterInstance<ILocalStorageProvider>(m_LocalStorageProvider);
            _factory = _mocks.DynamicMock<IUnitOfWorkFactory>();
            _unitOfWork = _mocks.DynamicMock<IUnitOfWork>();
            _session = _mocks.DynamicMock<ISession>();

            InstrumentUnitOfWork();

            _mocks.BackToRecordAll();
            SetupResult.For(_factory.Create()).Return(_unitOfWork);
            SetupResult.For(_factory.CurrentSession).Return(_session);

            
            _mocks.ReplayAll();
        }

        [TestCleanup]
        public void TearDownContext()
        {
            _mocks.VerifyAll();

            ResetUnitOfWork();
        }

        private void InstrumentUnitOfWork()
        {
            // brute force attack to set my own factory via reflection
            var fieldInfo = typeof(UnitOfWork).GetField("UnitOfWorkFactory",
                                BindingFlags.Static | BindingFlags.SetField | BindingFlags.NonPublic);
            fieldInfo.SetValue(null, _factory);
        }

        private void ResetUnitOfWork()
        {
            // assert that the UnitOfWork is reset
            var propertyInfo = typeof(UnitOfWork).GetProperty("CurrentUnitOfWork",
                                BindingFlags.Static | BindingFlags.SetProperty | BindingFlags.NonPublic);
            propertyInfo.SetValue(null, null, null);
            //var fieldInfo = typeof(UnitOfWork).GetField("_innerUnitOfWork",
            //                    BindingFlags.Static | BindingFlags.SetField | BindingFlags.NonPublic);
            //fieldInfo.SetValue(null, null);
        }

        [TestMethod]
        public void Can_Start_and_Dispose_UnitOfWork()
        {
            IUnitOfWork uow = UnitOfWork.Start();
            uow.Dispose();
        }

        [TestMethod]
        public void Can_access_current_unit_of_work()
        {
            IUnitOfWork uow = UnitOfWork.Start();
            var current = UnitOfWork.Current;
            uow.Dispose();
        }

        [TestMethod]
        public void Accessing_Current_UnitOfWork_if_not_started_throws()
        {
            try
            {
                var current = UnitOfWork.Current;
            }
            catch (InvalidOperationException)
            { }
        }

        [TestMethod]
        public void Starting_UnitOfWork_if_already_started_throws()
        {
            UnitOfWork.Start();
            try
            {
                UnitOfWork.Start();
            }
            catch (InvalidOperationException)
            { }
        }

        [TestMethod]
        public void Can_test_if_UnitOfWork_Is_Started()
        {
            Assert.IsFalse(UnitOfWork.IsStarted);

            IUnitOfWork uow = UnitOfWork.Start();
            Assert.IsTrue(UnitOfWork.IsStarted);
        }

        [TestMethod]
        public void Can_get_valid_current_session_if_UoW_is_started()
        {
            using (UnitOfWork.Start())
            {
                ISession session = UnitOfWork.CurrentSession;
                Assert.IsNotNull(session);
            }
        }

        [TestMethod]
        public void Get_current_session_if_UoW_is_not_started_throws()
        {
            try
            {
                ISession session = UnitOfWork.CurrentSession;
            }
            catch (InvalidOperationException)
            { }
        }
    }
}
