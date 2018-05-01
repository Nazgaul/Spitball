using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Extension;

namespace Cloudents.Core
{
    public class Shuffle : IShuffle
    {
        public IEnumerable<T> ShuffleByPriority<T>(IEnumerable<T> result) where T : IShuffleable
        {
            return result?.OrderBy(o =>
                o.PrioritySource.Priority * o.Order).ThenBy(n => n.PrioritySource.Priority).ThenBy(p => p.Order);
        }


        public IEnumerable<T> ShuffleBySource<T>(IEnumerable<T> result) where T : IShuffleable
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

            var order = 1;
            var objectToKeep = listOfResult.RemoveAndGet(0);
            objectToKeep.Order = order;
            var list = new List<T> { objectToKeep };

            while (listOfResult.Count > 0)
            {
                
                var lastElement = list.Last();
                var index = listOfResult.FindIndex(f => !Equals(f.PrioritySource.Source, lastElement.PrioritySource.Source));
                if (index == -1)
                {
                    index = 0;
                }

                var newObj = listOfResult.RemoveAndGet(index);
                newObj.Order = ++order;
                list.Add(newObj);
            }

            return list;
        }

      
    }
}