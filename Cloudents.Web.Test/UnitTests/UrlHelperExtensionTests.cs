using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Web.Test.UnitTests
{
    [TestClass]
    public class UrlHelperExtensionTests
    {
        [TestMethod]
        public void AddObject_SomeObject_ReturnParseOk()
        {
            //UrlHelperExtension();
            var prefix = string.Empty;
            var val = new TutorRequest
            {
                Page = 0,
                Location = new GeographicCoordinate
                {
                    Latitude = 0,
                    Longitude = 0
                }
            };

            var nvc = new NameValueCollection();
            var type = new PrivateType(typeof(UrlHelperExtension));
            type.InvokeStatic("AddObject", BindingFlags.Static | BindingFlags.NonPublic, prefix, val, nvc);

            var nvcResult = new NameValueCollection
            {
                ["Page"] = 0.ToString(),
                ["Location.Latitude"] = 0.ToString(),
                ["Location.Longitude"] = 0.ToString()
            };

            var dicExpected = nvcResult.AllKeys.ToDictionary(k => k, k => nvcResult[k]);
            var dicResult = nvc.AllKeys.ToDictionary(k => k, k => nvc[k]);

            CollectionAssert.AreEquivalent(dicExpected, dicResult);
        }


        [TestMethod]
        public void AddObject_AnonymousObject_ReturnParseOk()
        {
            var prefix = string.Empty;
            var val = new {nextPageToken= "Some_very_very_very_very_very_very_long_token" };

            var nvc = new NameValueCollection();
            var type = new PrivateType(typeof(UrlHelperExtension));
            type.InvokeStatic("AddObject", BindingFlags.Static | BindingFlags.NonPublic, prefix, val, nvc);

            var nvcResult = new NameValueCollection
            {
                ["nextPageToken"] = "Some_very_very_very_very_very_very_long_token",
            };
            var dicExpected = nvcResult.AllKeys.ToDictionary(k => k, k => nvcResult[k]);
            var dicResult = nvc.AllKeys.ToDictionary(k => k, k => nvc[k]);

            CollectionAssert.AreEquivalent(dicExpected, dicResult);
        }
    }
}
