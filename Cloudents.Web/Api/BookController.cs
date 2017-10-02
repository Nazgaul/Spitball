using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Http;
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

        public async Task<IActionResult> Get(string term, int page, CancellationToken token)
        {
            if (term == null) throw new ArgumentNullException(nameof(term));
            var result = await m_BooksSearch.SearchAsync(term, page, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}