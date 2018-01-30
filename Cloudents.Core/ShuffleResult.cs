using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Extension;

namespace Cloudents.Core
{
    public interface IShuffleable
    {
        object Bucket { get; }
    }

    public static class Shuffle<T> where T : IShuffleable
    {
       public static IEnumerable<T> DoShuffle(IEnumerable<T> result)
        {
            if (result == null)
            {
                return null;
            }
            var listOfResult = result.ToList();
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
