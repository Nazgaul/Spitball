using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using System.Reflection;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
using Zbang.Zbox.Infrastructure.UnitsOfWork;

namespace Zbang.Zbox.Infrastructure.Tests
{
    [TestFixture]
    public class UnitOfWorkFactory_Fixture
    {
        private readonly MockRepository _mocks = new MockRepository();
        private IUnitOfWorkFactory _factory;
        private IUnitOfWork _unitOfWork;

        [SetUp]
        public void SetupContext()
        {
            _factory = _mocks.DynamicMock<IUnitOfWorkFactory>();
            _unitOfWork = _mocks.DynamicMock<IUnitOfWork>();

            // brute force attack to set my own factory via reflection
            var fieldInfo = typeof(UnitOfWork).GetField("_unitOfWorkFactory", BindingFlags.Static | BindingFlags.SetField | BindingFlags.NonPublic);
            fieldInfo.SetValue(null, _factory);

            _mocks.BackToRecordAll();
            SetupResult.For(_factory.Create()).Return(_unitOfWork);
            _mocks.ReplayAll();
        }

        [TearDown]
        public void TearDownContext()
        {
            _mocks.VerifyAll();

            // brute force attack to set my own factory via reflection
            var fieldInfo = typeof(UnitOfWork).GetField("_unitOfWorkFactory", BindingFlags.Static | BindingFlags.SetField | BindingFlags.NonPublic);
            fieldInfo.SetValue(null, null);

        }

        [Test]
        public void Starting_UnitOfWork_if_already_started_throws()
        {
            UnitOfWork.Start();

            try
            {
                UnitOfWork.Start();
            }
            catch
            { 
            }
        }

        [Test]
        public void Can_access_current_unit_of_work()
        {
            IUnitOfWork uow = UnitOfWork.Start();
            var current = UnitOfWork.Current;
            Assert.AreSame(uow, current);
        }
    }
}
