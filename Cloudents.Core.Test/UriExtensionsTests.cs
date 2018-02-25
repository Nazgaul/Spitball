using System;
using Cloudents.Core.Extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Core.Test
{
    [TestClass]
    public class UriExtensionsTests
    {
        [TestMethod]
        public void GetUriDomain_SomeUri_ActualDomain()
        {
            var uri = new Uri("https://www.coursehero.com/assets/img/coursehero_logo.png");
            var domain = uri.GetUriDomain();

            Assert.AreEqual("courseHero.com", domain, true);
        }
    }
}
