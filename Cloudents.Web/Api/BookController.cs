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
        private readonly IBookSearch _booksSearch;

        public BookController(IBookSearch booksSearch)
        {
            _booksSearch = booksSearch;
        }

        [Route("search")]
        public async Task<IActionResult> GetAsync(string[] term, int page, CancellationToken token)
        {
            var result = await _booksSearch.SearchAsync(term, page, token).ConfigureAwait(false);
            return Json(result);
        }

        [Route("buy"), HttpGet]
        public async Task<IActionResult> BuyAsync(string isbn13, CancellationToken token)
        {
            if (isbn13 == null) throw new ArgumentNullException(nameof(isbn13));
            var result = await _booksSearch.BuyAsync(isbn13,  token).ConfigureAwait(false);
            //result.Prices = result.Prices.Select((s,i) =>
            //{
            //    s.Link = Url.Action("Index", "Url", new
            //    {
            //        url = s.Link,
            //        host = "campusBooks",
            //        location = i
            //    });
            //    return s;
            //});
            return Json(result);
        }

        [Route("sell"), HttpGet]
        public async Task<IActionResult> SellAsync(string isbn13, CancellationToken token)
        {
            if (isbn13 == null) throw new ArgumentNullException(nameof(isbn13));
            var result = await _booksSearch.SellAsync(isbn13,  token).ConfigureAwait(false);
            return Json(result);
        }
    }
}