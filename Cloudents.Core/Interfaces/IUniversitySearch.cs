using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Models;
using JetBrains.Annotations;

namespace Cloudents.Core.Interfaces
{
    public interface IUniversitySearch
    {
        [ItemCanBeNull]
        Task<IEnumerable<UniversityDto>> SearchAsync(string term, GeoPoint location,
            CancellationToken token);

        [ItemCanBeNull]
        Task<IEnumerable<UniversityDto>> GetApproximateUniversitiesAsync(GeoPoint location,
            CancellationToken token);
    }
}
