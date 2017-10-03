using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;

namespace Cloudents.Core.Interfaces
{
    public interface IBookSearch
    {
        Task<IEnumerable<BookSearchDto>> SearchAsync(string term, int imageWidth, int page, CancellationToken token);
        Task<BookDetailsDto> BuyAsync(string isbn13, int imageWidth, CancellationToken token);

        Task<BookDetailsDto> SellAsync(string isbn13, int imageWidth, CancellationToken token);
    }
}