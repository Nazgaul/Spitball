using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Models;
using JetBrains.Annotations;

namespace Cloudents.Core.Interfaces
{
    public interface IUniversitySearch
    {
        [ItemCanBeNull]
        Task<IEnumerable<UniversityDto>> SearchAsync(string term,
            [CanBeNull]
            GeoPoint location,
            CancellationToken token);

        [ItemCanBeNull]
        Task<UniversityDto> GetApproximateUniversitiesAsync(
            [NotNull]
            GeoPoint location,
            CancellationToken token);
    }

    public interface IQuestionSearch
    {
        Task<ResultWithFacetDto<Question>> SearchAsync(string term, IEnumerable<string> facet, CancellationToken token);
    }
}
