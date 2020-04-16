using Cloudents.Core.Entities;

namespace Cloudents.Query
{
    public interface IQueryAdmin<TResult> : IQuery<TResult>
    {
        string Country { get; }
    }


    public interface IQueryAdmin2<TResult> : IQuery<TResult>
    {
        Country Country { get; }
    }
}
