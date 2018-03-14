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
            JobRequestSort sort,
            IEnumerable<string> jobType,
            Location location,
            int page,
            HighlightTextFormat highlight,
            CancellationToken token);
    }


    public interface IJobProvider
    {
        Task<ResultWithFacetDto<JobDto>> SearchAsync(string term,
            JobRequestSort sort,
            IEnumerable<JobFilter> jobType,
            Location location,
            int page,
            HighlightTextFormat highlight,
            CancellationToken token);
    }
}