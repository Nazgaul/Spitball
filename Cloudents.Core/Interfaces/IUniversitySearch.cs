using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Models;

namespace Cloudents.Core.Interfaces
{
    public interface IUniversitySearch
    {
        Task<IEnumerable<UniversityDto>> SearchAsync(string term, GeoPoint location,
            CancellationToken token);
    }
}
