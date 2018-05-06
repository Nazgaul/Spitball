using System.Reflection;
using Cloudents.Core.Models;
using Cloudents.Web.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Web.Test.UnitTests
{
    [TestClass]
    public class TempDataExtensionsTests
    {
        [TestMethod]
        public void Serialize_Location_CanSerialize()
        {
            var address = new Location(null, null, null);
            var serializeMethod = typeof(TempDataExtensions).GetMethod("Serialize", BindingFlags.Static | BindingFlags.NonPublic );
            if (serializeMethod == null)
            {
                Assert.Fail("Could not find method");
                return;
            }
            var genericSerializeMethod = serializeMethod.MakeGenericMethod(typeof(Location));
            genericSerializeMethod.Invoke(typeof(Location), new object[] { address });
            var success = true;
            Assert.IsTrue(success);

            //    var result2 = type.InvokeStatic("BuildArgument", BindingFlags.Static | BindingFlags.NonPublic, bookRequest2);

        }
    }
}