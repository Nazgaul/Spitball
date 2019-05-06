using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Models;

namespace Cloudents.Core.Interfaces
{
    public interface IGooglePlacesSearch
    {
        //Task<(Address address, GeoPoint point)> GeoCodingByZipAsync(string zip, CancellationToken token);
        Task<(Address address, GeoPoint point)> ReverseGeocodingAsync(GeoPoint point, CancellationToken token);
    }
}