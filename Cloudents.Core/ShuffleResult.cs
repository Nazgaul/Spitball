using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Extension;

namespace Cloudents.Core
{
    public interface IShuffleable
    {
        object Bucket { get; }
    }

    public interface IShuffle
    {
        IEnumerable<T> DoShuffle<T>(IEnumerable<T> result) where T : IShuffleable;
    }

    public class Shuffle : IShuffle
    {
        public IEnumerable<T> DoShuffle<T>(IEnumerable<T> result) where T : IShuffleable
        {
            if (result == null)
            {
                return null;
            }
            var listOfResult = result.ToList();
            if (listOfResult.Count == 0)
            {
                return listOfResult;
            }
            var objectToKeep = listOfResult.RemoveAndGet(0);

            var list = new List<T> { objectToKeep };

            while (listOfResult.Count > 0)
            {
                var lastElement = list.Last();
                var index = listOfResult.FindIndex(f => !Equals(f.Bucket, lastElement.Bucket));
                if (index == -1)
                {
                    index = 0;
                }
                list.Add(listOfResult.RemoveAndGet(index));
            }

            return list;
        }
    }
}
