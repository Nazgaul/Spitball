using Cloudents.Core.Request;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Core.Test
{
    [TestClass]
    public class SyncAzureQueryTests
    {
        [TestMethod]
        public void ConvertFromString_Null_Init()
        {
            var result = SyncAzureQuery.ConvertFromString(null);
            var expected = new SyncAzureQuery(0,0);
            Assert.AreEqual(result,expected);
        }

        [TestMethod]
        public void ConvertFromString_EmptyString_Init()
        {
            var result = SyncAzureQuery.ConvertFromString(string.Empty);
            var expected = new SyncAzureQuery(0, 0);
            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void ConvertFromString_SomeValue_Init()
        {
            var input = "2|4";
            var result = SyncAzureQuery.ConvertFromString(input);
            var expected = new SyncAzureQuery(2, 4);
            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void ConvertFromString_FullCycle_Init()
        {
            var expected = new SyncAzureQuery(2, 4);
            var result = SyncAzureQuery.ConvertFromString(expected.ToString());
            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void ConvertFromString_InvalidInput_Init()
        {
            var input = "2|4|2";
            var result = SyncAzureQuery.ConvertFromString(input);
            var expected = new SyncAzureQuery(0, 0);
            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void ConvertFromString_InvalidInput2_Init()
        {
            var input = "2&4";
            var result = SyncAzureQuery.ConvertFromString(input);
            var expected = new SyncAzureQuery(0, 0);
            Assert.AreEqual(result, expected);
        }
    }
}