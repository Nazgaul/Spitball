using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Query;

namespace Cloudents.Core.Interfaces
{
    public interface IBlogProvider
    {
        Task<IEnumerable<DashboardBlogDto>> GetBlogAsync(BlogQuery query, CancellationToken token);
    }
}