using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;

namespace Cloudents.Core.Interfaces
{
    public interface IPlacesSearch
    {
        Task<IEnumerable<PlaceDto>> SearchNearbyAsync(string term, SearchRequestFilter filter,
            GeoPoint location, CancellationToken token);

        Task<PlaceDto> SearchAsync(string term, CancellationToken token);
    }
}