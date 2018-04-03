using System.Collections.Generic;
using JetBrains.Annotations;

namespace Cloudents.Core
{
    public interface IShuffleable
    {
        PrioritySource PrioritySource { get; }
        int Order { get; set; }
    }

    public interface IShuffle
    {
        [CanBeNull]
        IEnumerable<T> DoShuffle<T>(IEnumerable<T> result) where T : IShuffleable;
    }
}
