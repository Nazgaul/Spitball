//using Cloudents.Core.Models;
//using Xunit;

//namespace Cloudents.Core.Test
//{
//    public class GeoPointTests
//    {
//        [Fact]
//        public void OperatorEqual_SameValue_True()
//        {
//            var point1 = new GeoPoint(-74.0059738159179f, 40.71277618408203f);
//            var point2 = new GeoPoint(-74.0059738159179f, 40.71277618408203f);
//            var c = point1 == point2;
//            Assert.True(c);
//        }

//        [Fact]
//        public void OperatorEqual_OneNull_False()
//        {
//            var point1 = new GeoPoint(-74.0059738159179f, 40.71277618408203f);
//            const GeoPoint point2 = null;
//            // ReSharper disable once ConditionIsAlwaysTrueOrFalse test purpose
//            var c = point1 == point2;
//            Assert.False(c);
//        }
//    }
//}
