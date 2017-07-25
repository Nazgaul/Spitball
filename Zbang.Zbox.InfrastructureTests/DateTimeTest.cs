using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zbang.Zbox.InfrastructureTests
{
    [TestClass]
    public class DateTimeTest
    {
        [TestMethod]
        public void ParseExactRfc1123()
        {
            var str = "Wed, 24 May 2017 16:30:23 UTC";
            str = str.Replace("UTC", string.Empty).Trim();
            DateTimeOffset.TryParse(str, out DateTimeOffset p);


            Assert.AreEqual(p.Year, 2017);


        }
    }
}
