using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;
using JetBrains.Annotations;

namespace Cloudents.Core.Interfaces
{
    public interface IPlacesSearch
    {
        [ItemCanBeNull]
        Task<PlacesNearbyDto> SearchAsync(IEnumerable<string> term, PlacesRequestFilter filter,
            GeoPoint location, string nextPageToken, CancellationToken token);
    }
}