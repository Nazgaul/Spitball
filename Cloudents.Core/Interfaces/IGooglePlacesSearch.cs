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
            GeoPoint location, string nextPageToken, CancellationToken token);

        //Task<PlaceDto> SearchAsync(string term, CancellationToken token);
        Task<PlaceDto> ByIdAsync(string id, CancellationToken token);
        Task<GeoPoint> GeoCodingAsync(string location, CancellationToken token);
    }

    public interface IPlacesSearch
    {
        Task<(string token, IEnumerable<PlaceDto> data)> SearchAsync(IEnumerable<string> term, PlacesRequestFilter filter,
            GeoPoint location, string nextPageToken, CancellationToken token);
    }
}