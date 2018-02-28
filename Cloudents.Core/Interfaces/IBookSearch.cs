using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using JetBrains.Annotations;

namespace Cloudents.Core.Interfaces
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