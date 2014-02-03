using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Cloudents.Mvc4WebRole.Helpers;

namespace Zbang.Zbox.Mvc3WebRoleTests.Helpers
{
    [TestClass]
    public class UrlBuilderTests
    {
        [TestMethod]
        public void NameToQueryString_QueryWithSpaces_ReturnHyphen()
        {
            var url = "invitation only";
            var retVal = UrlBuilder.NameToQueryString(url);
            Assert.AreEqual(retVal, "invitation-only");
        }

        [TestMethod]
        public void NameToQeuryString_QueryWithSeveralSpaces_ReturnOneHypen()
        {
            var url = "invitation    only";
            var retVal = UrlBuilder.NameToQueryString(url);
            Assert.AreEqual(retVal, "invitation-only");
        }

        [TestMethod]
        public void NameToQueryString_QueryWithSpacesAndDash_ReturnOneHypen()
        {
            var url = "oxford - YouTube";
            var retVal = UrlBuilder.NameToQueryString(url);
            Assert.AreEqual(retVal, "oxford-youtube");
        }
    }
}
