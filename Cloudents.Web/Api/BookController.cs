using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    /// <inheritdoc />
    /// <summary>
    /// The book api controller
    /// </summary>

    [Route("api/[controller]"), ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookSearch _booksSearch;

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="booksSearch"></param>
        public BookController(IBookSearch booksSearch)
        {
            _booksSearch = booksSearch;
        }

        /// <summary>
        /// Search book vertical end point
        /// </summary>
        /// <param name="bookRequest"></param>
        /// <param name="token"></param>
        /// <returns>List of book</returns>
        [Route("search", Name = "BookSearch"), HttpGet]
        public async Task<WebResponseWithFacet<BookSearchDto>> GetAsync([FromQuery]BookRequest bookRequest, CancellationToken token)
        {
            bookRequest = bookRequest ?? new BookRequest();
            var result = (await _booksSearch.SearchAsync(bookRequest.Term, bookRequest.Page, token)).ToListIgnoreNull();
            string nextPageLink = null;
            if (result.Count > 0)
            {
                nextPageLink = Url.NextPageLink("BookSearch", null, bookRequest);
            }
            return new WebResponseWithFacet<BookSearchDto>
            {
                Result = result,
                NextPageLink = nextPageLink
            };
        }

        /// <summary>
        /// Buy book api
        /// </summary>
        /// <param name="isbn13">the book serial number</param>
        /// <param name="token"></param>
        /// <returns>The book details + list of merchant and their prices</returns>
        /// <exception cref="ArgumentNullException">The isbn is empty</exception>
        [Route("buy"), HttpGet]
        public async Task<BookDetailsDto> BuyAsync(string isbn13, CancellationToken token)
        {
            if (isbn13 == null) throw new ArgumentNullException(nameof(isbn13));
            var result = await _booksSearch.BuyAsync(isbn13, token);
            return result;
        }

        /// <summary>
        /// Sell book api
        /// </summary>
        /// <param name="isbn13">the book serial number</param>
        /// <param name="token"></param>
        /// <returns>The book details + list of merchant and their prices</returns>
        /// <exception cref="ArgumentNullException">The isbn is empty</exception>
        [Route("sell"), HttpGet]
        public async Task<BookDetailsDto> SellAsync(string isbn13, CancellationToken token)
        {
            if (isbn13 == null) throw new ArgumentNullException(nameof(isbn13));
            var result = await _booksSearch.SellAsync(isbn13, token);
            return result;
        }
    }
}