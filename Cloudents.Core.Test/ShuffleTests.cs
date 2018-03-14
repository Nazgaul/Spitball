using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Core.Test
{
    [TestClass]
    public class ShuffleTests
    {

        //private class TestPrioritySource : PrioritySource
        //{
        //    protected TestPrioritySource(string source, int priority) : base(source, priority)
        //    {
        //    }

        //    public static readonly PrioritySource TestA = new TestPrioritySource("A", 1);
        //    public static readonly PrioritySource TestB = new TestPrioritySource("B", 2);
        //    public static readonly PrioritySource TestC = new TestPrioritySource("C", 3);

        //}
        public static readonly PrioritySource TestA = new PrioritySource(1);
        public static readonly PrioritySource TestB = new PrioritySource(2);
        public static readonly PrioritySource TestC = new PrioritySource(3);

        private class ShuffleTestModel : IShuffleable
        {
           
            

            public PrioritySource PrioritySource { get; set; }
            public int Order { get; set; }
        }

        [TestMethod]
        public void DoShuffle_Array_RightResult()
        {
            var list = new List<ShuffleTestModel>
            {
                new ShuffleTestModel {PrioritySource = TestA, Order = 0},
                new ShuffleTestModel {PrioritySource = TestA, Order = 1},
                new ShuffleTestModel {PrioritySource = TestA, Order = 2},
                new ShuffleTestModel {PrioritySource = TestB, Order = 3},
                new ShuffleTestModel {PrioritySource = TestB, Order = 4},
                new ShuffleTestModel {PrioritySource = TestC, Order = 5},
                new ShuffleTestModel {PrioritySource = TestC, Order = 6},
                new ShuffleTestModel {PrioritySource = TestA, Order = 7}
            };
            var shuffle = new Shuffle();
            var result = shuffle.DoShuffle(list).ToList();

            var expectedList = new List<ShuffleTestModel>
            {
                new ShuffleTestModel {PrioritySource = TestA, Order = 0},
                new ShuffleTestModel {PrioritySource = TestA, Order = 1},
                new ShuffleTestModel {PrioritySource = TestA, Order = 2},
                new ShuffleTestModel {PrioritySource = TestB, Order = 3},
                new ShuffleTestModel {PrioritySource = TestA, Order = 7},
                new ShuffleTestModel {PrioritySource = TestB, Order = 4},
                new ShuffleTestModel {PrioritySource = TestC, Order = 5},
                new ShuffleTestModel {PrioritySource = TestC, Order = 6}
            };
            result.Should().BeEquivalentTo(expectedList);
            //CollectionAssert.AreEqual(result, expectedList);
        }

        [TestMethod]
        public void DoShuffle_Array2_RightResult()
        {
            var list = new List<ShuffleTestModel>
            {
                new ShuffleTestModel {PrioritySource = TestA, Order = 0},
                new ShuffleTestModel {PrioritySource = TestA, Order = 1},
                new ShuffleTestModel {PrioritySource = TestA, Order = 2},
                new ShuffleTestModel {PrioritySource = TestA, Order = 3},
                new ShuffleTestModel {PrioritySource = TestA, Order = 4},
                new ShuffleTestModel {PrioritySource = TestA, Order = 5},
                new ShuffleTestModel {PrioritySource = TestA, Order = 6},
                new ShuffleTestModel {PrioritySource = TestA, Order = 7}
            };
            var shuffle = new Shuffle();
            var result = shuffle.DoShuffle(list).ToList();
            result.Should().BeEquivalentTo(list);
            //CollectionAssert.AreEqual(result, list);
        }
    }
}
