using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;

namespace Cloudents.Core.Interfaces
{
    public interface ICourseSearch
    {
        Task<IEnumerable<CourseDto>> SearchAsync(string term, long universityId,
            CancellationToken token);
    }
}
