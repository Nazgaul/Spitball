using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.DTOs;
using JetBrains.Annotations;

namespace Cloudents.Application.Interfaces
{
    public interface IBookSearch
    {
        [ItemCanBeNull]
        Task<IEnumerable<BookSearchDto>> SearchAsync(IEnumerable<string> term, int page, CancellationToken token);

        [ItemCanBeNull]
        Task<BookDetailsDto> BuyAsync(string isbn13, CancellationToken token);

        [ItemCanBeNull]
        Task<BookDetailsDto> SellAsync(string isbn13, CancellationToken token);
    }
}