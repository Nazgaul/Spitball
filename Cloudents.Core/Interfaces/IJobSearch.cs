using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;

namespace Cloudents.Core.Interfaces
{
    public interface IJobSearch
    {
        Task<ResultWithFacetDto<JobDto>> SearchAsync(IEnumerable<string> term,
            JobRequestFilter filter,
            JobRequestSort sort,
            IEnumerable<string> jobType,
            GeoPoint location,
            int page,
            CancellationToken token);
    }
}