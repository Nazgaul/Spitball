using Cloudents.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Core.Test
{
    [TestClass]
    public class GeoPointTests
    {
        [TestMethod]
        public void OperatorEqual_SameValue_True()
        {
            var point1 = new GeoPoint()
            {
                Latitude = 40.71277618408203,
                Longitude = -74.00597381591797
            };
            var point2 = new GeoPoint()
            {
                Latitude = 40.71277618408203,
                Longitude = -74.00597381591797
            };
            var c = point1 == point2;
            Assert.IsTrue(c);
        }

        [TestMethod]
        public void OperatorEqual_OneNull_False()
        {
            var point1 = new GeoPoint()
            {
                Latitude = 40.71277618408203,
                Longitude = -74.00597381591797
            };
            const GeoPoint point2 = null;
            var c = point1 == point2;
            Assert.IsFalse(c);
        }
    }
}
