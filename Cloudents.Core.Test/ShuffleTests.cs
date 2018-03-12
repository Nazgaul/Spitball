using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Core.Test
{
    [TestClass]
    public class ShuffleTests
    {
        private class ShuffleTestModel : IShuffleable
        {
            public string Host { get; set; }

            public int Result { get; set; }

            public object Bucket => Host;
           
        }

        [TestMethod]
        public void DoShuffle_Array_RightResult()
        {
            var list = new List<ShuffleTestModel>
            {
                new ShuffleTestModel {Host = "A", Result = 1},
                new ShuffleTestModel {Host = "A", Result = 2},
                new ShuffleTestModel {Host = "A", Result = 3},
                new ShuffleTestModel {Host = "B", Result = 1},
                new ShuffleTestModel {Host = "B", Result = 2},
                new ShuffleTestModel {Host = "C", Result = 1},
                new ShuffleTestModel {Host = "C", Result = 2},
                new ShuffleTestModel {Host = "A", Result = 4}
            };
            var shuffle = new Shuffle();
            var result = shuffle.DoShuffle(list).ToList();

            var expectedList = new List<ShuffleTestModel>
            {
                new ShuffleTestModel {Host = "A", Result = 1},
                new ShuffleTestModel {Host = "B", Result = 1},
                new ShuffleTestModel {Host = "A", Result = 2},
                new ShuffleTestModel {Host = "B", Result = 2},
                new ShuffleTestModel {Host = "A", Result = 3},
                new ShuffleTestModel {Host = "C", Result = 1},
                new ShuffleTestModel {Host = "A", Result = 4},
                new ShuffleTestModel {Host = "C", Result = 2}
            };
            result.Should().BeEquivalentTo(expectedList);
            //CollectionAssert.AreEqual(result, expectedList);
        }

        [TestMethod]
        public void DoShuffle_Array2_RightResult()
        {
            var list = new List<ShuffleTestModel>
            {
                new ShuffleTestModel {Host = "A", Result = 1},
                new ShuffleTestModel {Host = "A", Result = 2},
                new ShuffleTestModel {Host = "A", Result = 3},
                new ShuffleTestModel {Host = "A", Result = 4},
                new ShuffleTestModel {Host = "A", Result = 5},
                new ShuffleTestModel {Host = "A", Result = 6},
                new ShuffleTestModel {Host = "A", Result = 7},
                new ShuffleTestModel {Host = "A", Result = 8}
            };
            var shuffle = new Shuffle();
            var result = shuffle.DoShuffle(list).ToList();
            result.Should().BeEquivalentTo(list);
            //CollectionAssert.AreEqual(result, list);
        }
    }
}
