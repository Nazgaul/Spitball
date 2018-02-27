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
            var point1 = new GeoPoint(-74.0059738159179f, 40.71277618408203f);
            var point2 = new GeoPoint(-74.0059738159179f, 40.71277618408203f);
            var c = point1 == point2;
            Assert.IsTrue(c);
        }

        [TestMethod]
        public void OperatorEqual_OneNull_False()
        {
            var point1 = new GeoPoint(-74.0059738159179f, 40.71277618408203f);
            const GeoPoint point2 = null;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse test purpose
            var c = point1 == point2;
            Assert.IsFalse(c);
        }
    }
}
