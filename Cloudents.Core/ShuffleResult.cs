using System.Collections.Generic;
using JetBrains.Annotations;

namespace Cloudents.Core
{
    public interface IShuffleable 
    {
        PrioritySource PrioritySource { get; }
        int Order { get; set; }
    }

    //public interface ISourceShuffleable
    //{
    //    int Order { get; }

    //    object Bucket { get; }
    //}

    public interface IShuffle
    {
        [CanBeNull]
        IEnumerable<T> ShuffleByPriority<T>(IEnumerable<T> result) where T : IShuffleable;

        IEnumerable<T> ShuffleBySource<T>(IEnumerable<T> result) where T : IShuffleable;

    }
}
