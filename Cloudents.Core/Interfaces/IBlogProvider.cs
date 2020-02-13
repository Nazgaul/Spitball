using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;

namespace Cloudents.Core.Interfaces
{
    public interface IBlogProvider
    {
        Task<IEnumerable<DashboardBlogDto>> GetBlogAsync(Country country, CancellationToken token);
    }
}