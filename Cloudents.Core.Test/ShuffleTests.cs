using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Core.Test
{
    [TestClass]
    public class ShuffleTests
    {
        class ShuffleTestModel : IShuffleable
        {
            public string Host { get; set; }

            public int Result { get; set; }


            public object Bucket => Host;

            public override bool Equals(object obj)
            {
                if (obj is ShuffleTestModel p)
                {
                    return p.Host == Host && p.Result == Result;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return this.Result.GetHashCode() * 11 + this.Host.GetHashCode() * 13;
            }
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
            var result = Shuffle<ShuffleTestModel>.DoShuffle(list).ToList();


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

            CollectionAssert.AreEqual(result,expectedList);

        }
    }
}
