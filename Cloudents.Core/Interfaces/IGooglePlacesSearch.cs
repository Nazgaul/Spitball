using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;

namespace Cloudents.Core.Interfaces
{
    public interface IGooglePlacesSearch
    {
        Task<(string token, IEnumerable<PlaceDto> data)> SearchNearbyAsync(IEnumerable<string> term, PlacesRequestFilter filter,
            Location location, string nextPageToken, CancellationToken token);

        Task<PlaceDto> ByIdAsync(string id, CancellationToken token);
        Task<Location> GeoCodingByAddressAsync(string address, CancellationToken token);
        Task<Location> GeoCodingByZipAsync(string zip, CancellationToken token);
        //Task<AddressDto> ReverseGeocodingAsync(Location point, CancellationToken token);
    }
}