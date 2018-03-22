using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Core
{
    public interface IShuffleable
    {
        PrioritySource PrioritySource { get; }
        int Order { get; set; }
    }

    public interface IShuffle
    {
        IEnumerable<T> DoShuffle<T>(IEnumerable<T> result) where T : IShuffleable;
    }

    public class Shuffle : IShuffle
    {
        public IEnumerable<T> DoShuffle<T>(IEnumerable<T> result) where T : IShuffleable
        {
            return result;
            return result.OrderBy(o =>
                o.PrioritySource.Priority * o.Order).ThenBy(n => n.PrioritySource.Priority).ThenBy(p => p.Order);
        }
    }
}
