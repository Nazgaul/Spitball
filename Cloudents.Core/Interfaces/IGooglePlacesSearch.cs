using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Models;
using JetBrains.Annotations;

namespace Cloudents.Core.Interfaces
{
    public interface IGooglePlacesSearch
    {

        Task<(Address address, GeoPoint point)> GeoCodingByAddressAsync(string address, CancellationToken token);
        Task<(Address address, GeoPoint point)> GeoCodingByZipAsync(string zip, CancellationToken token);
        Task<(Address address, GeoPoint point)> ReverseGeocodingAsync(GeoPoint point, CancellationToken token);
    }

    public interface IGoogleAuth
    {
        [ItemCanBeNull]
        Task<AuthDto> LogInAsync(string token, CancellationToken cancellationToken);
    }
}