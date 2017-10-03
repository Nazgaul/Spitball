using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/Book")]
    public class BookController : Controller
    {
        private readonly IBookSearch m_BooksSearch;

        public BookController(IBookSearch booksSearch)
        {
            m_BooksSearch = booksSearch;
        }

        [Route("search")]
        public async Task<IActionResult> Get(string[] term, int page, CancellationToken token)
        {
            if (term == null) throw new ArgumentNullException(nameof(term));
            var result = await m_BooksSearch.SearchAsync(string.Join(" ", term), 250, page, token).ConfigureAwait(false);
            return Json(result);
        }

        [Route("buy"), HttpGet]
        public async Task<IActionResult> BuyAsync(string isbn13, CancellationToken token)
        {
            if (isbn13 == null) throw new ArgumentNullException(nameof(isbn13));
            var result = await m_BooksSearch.BuyAsync(isbn13, 250, token).ConfigureAwait(false);
            return Json(result);
        }

        [Route("sell"), HttpGet]
        public async Task<IActionResult> SellAsync(string isbn13, CancellationToken token)
        {
            if (isbn13 == null) throw new ArgumentNullException(nameof(isbn13));
            var result = await m_BooksSearch.SellAsync(isbn13, 250, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}