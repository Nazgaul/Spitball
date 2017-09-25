using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;
using Cloudents.Core.Request;

namespace Cloudents.Core.Interfaces
{
    public interface ITutorSearch
    {
        Task<IEnumerable<TutorDto>> SearchAsync(string term, SearchRequestFilter filter, SearchRequestSort sort, Location location, CancellationToken token);
    }

    public interface ITitleSearch
    {
        Task<string> SearchAsync(string term, CancellationToken token);
    }

    public interface IJobSearch
    {
        Task<IEnumerable<JobDto>> SearchAsync(
            string term,
            SearchRequestFilter filter,
            SearchRequestSort sort,
            string jobType,
            Location location,
            CancellationToken token);
    }
}
