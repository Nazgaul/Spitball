using System;
using Cloudents.Core.Extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Infrastructure.Test
{
    [TestClass]

    public class UriExtensionsTests
    {
        [TestMethod]
        public void ChangeHost_SomeUrl_HostChanged()
        {
            var uri = new Uri("https://zboxstorage.blob.core.windows.net/spitball/studysoup.png");
          var result =   uri.ChangeHost("az32006.vo.msecnd.net");
            var expectedResult = new Uri("https://az32006.vo.msecnd.net/spitball/studysoup.png");
            Assert.AreEqual(expectedResult, result);
        }
    }
}
