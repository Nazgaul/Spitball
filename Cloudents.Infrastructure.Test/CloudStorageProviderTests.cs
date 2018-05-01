using System.Collections.Generic;
using System.Reflection;
using Cloudents.Core.Storage;
using Cloudents.Infrastructure.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Infrastructure.Test
{
    [TestClass]
    public class CloudStorageProviderTests
    {
        [TestMethod]
        public void GetQueues_NoInput_ReturnValue()
        {
            var cloudStorageProvider = new CloudStorageProvider("UseDevelopmentStorage=true");
            var obj = new PrivateObject(cloudStorageProvider);

            var val = obj.Invoke("GetQueues", BindingFlags.Static | BindingFlags.NonPublic);
            if (val is IEnumerable<QueueName> p)
            {
                var d = true;
                // ReSharper disable once UnusedVariable - used for test
                foreach (var t in p)
                {

                }
                Assert.IsTrue(d);
                return;
            }
            Assert.Fail("can't cast to IEnumerable<QueueName>");

        }
    }
}