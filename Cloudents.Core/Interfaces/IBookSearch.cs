using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;

namespace Cloudents.Core.Interfaces
{
    public interface IBookSearch
    {
        Task<IEnumerable<BookSearchDto>> SearchAsync(IEnumerable<string> term, int page, CancellationToken token);
        Task<BookDetailsDto> BuyAsync(string isbn13, CancellationToken token);

        Task<BookDetailsDto> SellAsync(string isbn13, CancellationToken token);
    }
}