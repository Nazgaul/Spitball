using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.DTOs;
using Cloudents.Application.Enum;
using Cloudents.Application.Models;

namespace Cloudents.Application.Interfaces
{
    public interface IJobSearch
    {
        Task<ResultWithFacetDto<JobDto>> SearchAsync(IEnumerable<string> term,
            JobRequestSort sort,
            IEnumerable<string> jobType,
            Location location,
            int page,
            CancellationToken token);
    }
}