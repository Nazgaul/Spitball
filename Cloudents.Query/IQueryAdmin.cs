using System.Security.Claims;
using System.Security.Principal;

namespace Cloudents.Query
{
    public interface IQueryAdmin<TResult> : IQuery<TResult>
    {
        string Country { get; }
    }
}
