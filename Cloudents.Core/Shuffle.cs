using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Core
{
    public class Shuffle : IShuffle
    {
        public IEnumerable<T> DoShuffle<T>(IEnumerable<T> result) where T : IShuffleable
        {
            return result.OrderBy(o =>
                o.PrioritySource.Priority * o.Order).ThenBy(n => n.PrioritySource.Priority).ThenBy(p => p.Order);
        }
    }
}