//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NUnit.Framework;
//using Rhino.Mocks;
//using System.Reflection;

//namespace Zbang.Zbox.Infrastructure.Tests
//{
//    [TestFixture]
//    public class UnitOfWork_Fixture_Rename
//    {
//        private readonly MockRepository _mocks = new MockRepository();

//        [Test]
//        public void Can_Start_UnitOfWork()
//        {
//            var factory = _mocks.DynamicMock<IUnitOfWorkFactory>();
//            var unitOfWork = _mocks.DynamicMock<IUnitOfWork>();

//            // brute force attack to set my own factory via reflection
//            var fieldInfo = typeof(UnitOfWork).GetField("_unitOfWorkFactory",
//                BindingFlags.Static | BindingFlags.SetField | BindingFlags.NonPublic);
//            fieldInfo.SetValue(null, factory);

//            using (_mocks.Record())
//            {
//                Expect.Call(factory.Create()).Return(unitOfWork);
//            }
//            using (_mocks.Playback())
//            {
//                var uow = UnitOfWork.Start();
//            }
//        }
//    }

//    [TestFixture]
//    public class UnitOfWork_With_Factory_Fixture
//    {
//        private readonly MockRepository _mocks = new MockRepository();
//        private IUnitOfWorkFactory _factory;
//        private IUnitOfWork _unitOfWork;
//        private ISession _session;

//        [TestFixtureSetUp]
//        public void TestFixtureSetUp()
//        {
//            ResetUnitOfWork();
//        }

//        [SetUp]
//        public void SetupContext()
//        {
//            _factory = _mocks.DynamicMock<IUnitOfWorkFactory>();
//            _unitOfWork = _mocks.DynamicMock<IUnitOfWork>();
//            _session = _mocks.DynamicMock<ISession>();

//            InstrumentUnitOfWork();

//            _mocks.BackToRecordAll();
//            SetupResult.For(_factory.Create()).Return(_unitOfWork);
//            SetupResult.For(_factory.CurrentSession).Return(_session);
//            _mocks.ReplayAll();
//        }

//        [TearDown]
//        public void TearDownContext()
//        {
//            _mocks.VerifyAll();

//            ResetUnitOfWork();
//        }

//        private void InstrumentUnitOfWork()
//        {
//            // brute force attack to set my own factory via reflection
//            var fieldInfo = typeof(UnitOfWork).GetField("_unitOfWorkFactory",
//                                BindingFlags.Static | BindingFlags.SetField | BindingFlags.NonPublic);
//            fieldInfo.SetValue(null, _factory);
//        }

//        private void ResetUnitOfWork()
//        {
//            // assert that the UnitOfWork is reset
//            var propertyInfo = typeof(UnitOfWork).GetProperty("CurrentUnitOfWork",
//                                BindingFlags.Static | BindingFlags.SetProperty | BindingFlags.NonPublic);
//            propertyInfo.SetValue(null, null, null);
//            //var fieldInfo = typeof(UnitOfWork).GetField("_innerUnitOfWork",
//            //                    BindingFlags.Static | BindingFlags.SetField | BindingFlags.NonPublic);
//            //fieldInfo.SetValue(null, null);
//        }

//        [Test]
//        public void Can_Start_and_Dispose_UnitOfWork()
//        {
//            IUnitOfWork uow = UnitOfWork.Start();
//            uow.Dispose();
//        }

//        [Test]
//        public void Can_access_current_unit_of_work()
//        {
//            IUnitOfWork uow = UnitOfWork.Start();
//            var current = UnitOfWork.Current;
//            uow.Dispose();
//        }

//        [Test]
//        public void Accessing_Current_UnitOfWork_if_not_started_throws()
//        {
//            try
//            {
//                var current = UnitOfWork.Current;
//            }
//            catch (InvalidOperationException ex)
//            { }
//        }

//        [Test]
//        public void Starting_UnitOfWork_if_already_started_throws()
//        {
//            UnitOfWork.Start();
//            try
//            {
//                UnitOfWork.Start();
//            }
//            catch (InvalidOperationException ex)
//            { }
//        }

//        [Test]
//        public void Can_test_if_UnitOfWork_Is_Started()
//        {
//            Assert.IsFalse(UnitOfWork.IsStarted);

//            IUnitOfWork uow = UnitOfWork.Start();
//            Assert.IsTrue(UnitOfWork.IsStarted);
//        }

//        [Test]
//        public void Can_get_valid_current_session_if_UoW_is_started()
//        {
//            using (UnitOfWork.Start())
//            {
//                ISession session = UnitOfWork.CurrentSession;
//                Assert.IsNotNull(session);
//            }
//        }

//        [Test]
//        public void Get_current_session_if_UoW_is_not_started_throws()
//        {
//            try
//            {
//                ISession session = UnitOfWork.CurrentSession;
//            }
//            catch (InvalidOperationException ex)
//            { }
//        }
//    }
//}
