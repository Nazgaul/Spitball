using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;

namespace Cloudents.Core.Interfaces
{
    public interface IBookSearch
    {
        Task<IEnumerable<BookSearchDto>> SearchAsync(string term, int page, CancellationToken token);
    }
}