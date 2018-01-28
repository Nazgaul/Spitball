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
        Task<(string token, IEnumerable<PlaceDto> data)> SearchAsync(IEnumerable<string> term, PlacesRequestFilter filter,
            Location location, string nextPageToken, CancellationToken token);
    }
}