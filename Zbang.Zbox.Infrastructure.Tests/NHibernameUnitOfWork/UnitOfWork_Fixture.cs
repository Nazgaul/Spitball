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
    public class UnitOfWork_Fixture
    {
        private readonly MockRepository _mocks = new MockRepository();

        [Test]
        public void Can_Start_UnitOfWork()
        {
            var factory = _mocks.DynamicMock<IUnitOfWorkFactory>();
            var unitOfWork = _mocks.DynamicMock<IUnitOfWork>();

            var fieldInfo = typeof(UnitOfWork).GetField("_unitOfWorkFactory", BindingFlags.Static | BindingFlags.SetField | BindingFlags.NonPublic);

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
}
