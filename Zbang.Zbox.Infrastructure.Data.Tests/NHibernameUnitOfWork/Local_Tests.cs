using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
using System.Threading;

namespace Zbang.Zbox.Infrastructure.Data.Tests.NHibernameUnitOfWork
{
    [TestFixture]
    public class Local_Tests
    {
        [SetUp]
        public void Setup()
        {
            Local.Data.Clear();
        }

        [Test]
        public void Data_StoreData_CanFindByKey()
        {
            Local.Data["Text"] = "This is a string";
            Local.Data["Number"] = 1;
            
            Assert.AreEqual("This is a string", Local.Data["Text"]);
            Assert.AreEqual(1, Local.Data["Number"]);            
        }

        [Test]
        public void Data_Clear_GetZeroItems()
        {
            Local.Data["Text"] = "This is a string";
            Local.Data["Number"] = 1;

            Local.Data.Clear();

            Assert.AreEqual(0, Local.Data.Count);            
        }

        [Test]
        public void Data_LocalData_IsThreadLocal()
        {
            int currentThreadId = Thread.CurrentThread.ManagedThreadId;

            Local.Data["one"] = "This is a string";
            Assert.AreEqual(1, Local.Data.Count);
             
            Thread backgroundThread = new Thread(this.RunInOtherThread);
            backgroundThread.Start(currentThreadId);
             
            Thread.Sleep(100);             
            Assert.AreEqual(1, Local.Data.Count);

            backgroundThread.Join();
        }

        private void RunInOtherThread(object callingThreadId)
        {
            int currentThreadId = Thread.CurrentThread.ManagedThreadId;

            Assert.AreNotEqual(callingThreadId, currentThreadId);
            Assert.AreEqual(0, Local.Data.Count);
            Local.Data["Text"] = "This is another string";
            Assert.AreEqual(1, Local.Data.Count);            
        }
    }
}
