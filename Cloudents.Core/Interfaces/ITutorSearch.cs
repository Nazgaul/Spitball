using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;

namespace Cloudents.Core.Interfaces
{
    public interface ITutorSearch
    {
        Task<IEnumerable<TutorDto>> SearchAsync(string term, SearchRequestFilter filter, SearchRequestSort sort, GeoPoint location, CancellationToken token);
    }

    public interface IPurchaseSearch
    {
        Task<IEnumerable<PlaceDto>> SearchAsync(string term, SearchRequestFilter filter,
            GeoPoint location, CancellationToken token);
    }
}
