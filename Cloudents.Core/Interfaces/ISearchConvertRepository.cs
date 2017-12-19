using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface ISearchConvertRepository
    {
        Task<(IEnumerable<string> universitySynonym, IEnumerable<string> courses)> ParseUniversityAndCoursesAsync(
            long? university, IEnumerable<long> course, CancellationToken token);
    }
}
