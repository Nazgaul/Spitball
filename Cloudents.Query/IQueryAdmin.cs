namespace Cloudents.Query
{
    public interface IQueryAdmin<TResult> : IQuery<TResult>
    {
        string Country { get; }
    }
}
