using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;
using JetBrains.Annotations;

namespace Cloudents.Core.Interfaces
{
    public interface IGooglePlacesSearch
    {
        [ItemCanBeNull]
        Task<PlacesNearbyDto> SearchNearbyAsync(IEnumerable<string> term,
            PlacesRequestFilter filter,
            [NotNull] GeoPoint location,
            string nextPageToken, CancellationToken token);

        [ItemCanBeNull]
        Task<PlaceDto> ByIdAsync(string id, CancellationToken token);
        Task<(Address address, GeoPoint point)> GeoCodingByAddressAsync(string address, CancellationToken token);
        Task<(Address address, GeoPoint point)> GeoCodingByZipAsync(string zip, CancellationToken token);
        Task<(Address address, GeoPoint point)> ReverseGeocodingAsync(GeoPoint point, CancellationToken token);
    }
}