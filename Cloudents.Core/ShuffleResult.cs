using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloudents.Core.Extension;

namespace Cloudents.Core
{
    public interface IShuffleable
    {
        object Bucket { get; }
    }

    public static class Shuffle<T> where T : IShuffleable
    {
       // private readonly List<T> _arr = new List<T>();

        //public ShuffleResult(IEnumerable<T> arr)
        //{
        //    _arr = arr.ToList();
        //}

        public static IEnumerable<T> DoShuffle(IEnumerable<T> result)
        {
            var listOfResult = result.ToList();
            var objectToKeep = listOfResult.RemoveAndGet(0);

            var list = new List<T> { objectToKeep };

            while (listOfResult.Count > 0)
            {
                var lastElement = list.Last();
                var index = listOfResult.FindIndex(f => !Equals(f.Bucket, lastElement.Bucket));
                list.Add(listOfResult.RemoveAndGet(index));
            }

            return list;
        }
    }
}
