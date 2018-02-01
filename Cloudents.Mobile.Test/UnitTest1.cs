using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Cloudents.Core.Models;
using Cloudents.Mobile.Extensions;
using Cloudents.Mobile.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Mobile.Test
{
    [TestClass]
    public class UrlHelperExtensionTests
    {
        [TestMethod]
        public void AddObject_SomeObject_ReturnParseOk()
        {
               //UrlHelperExtension();
            var type = new PrivateType(typeof(UrlHelperExtension));
            
            var prefix = string.Empty;
            var val = new TutorRequest
            {
                Page = 0,
                Location = new GeoPoint()
                {
                    Latitude = 0,
                    Longitude = 0
                }
            };
            var nvc = new NameValueCollection();
            type.InvokeStatic("AddObject", BindingFlags.Static | BindingFlags.NonPublic, prefix, val, nvc);

            var nvcResult = new NameValueCollection
            {
                ["Page"] = 0.ToString(),
                ["Location.Latitude"] = 0.ToString(),
                ["Location.Longitude"] = 0.ToString()
            };


            CollectionAssert.AreEquivalent(
                nvcResult.AllKeys.ToDictionary(k => k, k => nvcResult[k]),
                nvc.AllKeys.ToDictionary(k => k, k => nvc[k]));
        }
    }
}
