using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Web.Http;
using Zbang.Cloudents.Jared.Extensions;
using Zbang.Cloudents.Jared.Filters;
using Zbang.Cloudents.Jared.Models;

namespace Zbang.Cloudents.Jared.Controllers
{
    /// <summary>
    /// The book api controller
    /// </summary>

    [MobileAppController, RoutePrefix("api/book"), ApiVersion("2017-12-19")]
    [ControllerName(nameof(BookController))]
    public class Book2Controller : ApiController
    {
        private readonly IBookSearch _booksSearch;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="booksSearch"></param>
        public Book2Controller(IBookSearch booksSearch)
        {
            _booksSearch = booksSearch;
        }


        /// <summary>
        /// Search book vertical end point
        /// </summary>
        /// <param name="bookRequest"></param>
        /// <param name="token"></param>
        /// <returns>List of book</returns>
        /// <exception cref="ArgumentNullException">term cannot be empty</exception>
        //[Route("search"), MapToApiVersion("2017-01-01")]
        //public async Task<IHttpActionResult> Get([FromUri]BookRequest bookRequest, CancellationToken token)
        //{
        //    bookRequest = bookRequest ?? new BookRequest();
        //    var result = await _booksSearch.SearchAsync(bookRequest.Term, bookRequest.Thumbnail.GetValueOrDefault(150), bookRequest.Page.GetValueOrDefault(), token).ConfigureAwait(false);
        //    return Ok(result);
        //}

        /// <summary>
        /// Search book vertical end point with next page token add to query string api-version = 2017-12-19
        /// </summary>
        /// <param name="bookRequest"></param>
        /// <param name="token"></param>
        /// <returns>List of book</returns>
        /// <exception cref="ArgumentNullException">term cannot be empty</exception>
        //[VersionedRoute("search", "2017-12-19", Name = "BookSearch"), HttpGet]
        [HttpGet, Route("search", Name = "BookSearch")]
        public async Task<IHttpActionResult> SearchV2Async([FromUri]BookRequest bookRequest, CancellationToken token)
        {
            bookRequest = bookRequest ?? new BookRequest();
            var result = await _booksSearch.SearchAsync(bookRequest.Term, bookRequest.Thumbnail.GetValueOrDefault(150), bookRequest.Page.GetValueOrDefault(), token).ConfigureAwait(false);
            var nextPageLink = Url.NextPageLink("BookSearch", new
            {
                api_version = "2017-12-19"
            }, bookRequest);
            return Ok(new
            {
                result,
                nextPageLink
            }
            );
        }

        /// <summary>
        /// Buy book api 
        /// </summary>
        /// <param name="isbn13">the book serial number</param>
        /// <param name="thumbnail">size of width of image size</param>
        /// <param name="token"></param>
        /// <returns>The book details + list of merchant and their prices</returns>
        /// <exception cref="ArgumentNullException">The isbn is empty</exception>
        [Route("buy"), HttpGet]
        public async Task<IHttpActionResult> BuyAsync(string isbn13, int? thumbnail, CancellationToken token)
        {
            if (isbn13 == null) throw new ArgumentNullException(nameof(isbn13));
            var result = await _booksSearch.BuyAsync(isbn13, thumbnail.GetValueOrDefault(150), token).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Sell book api 
        /// </summary>
        /// <param name="isbn13">the book serial number</param>
        /// <param name="thumbnail">size of width of image size</param>
        /// <param name="token"></param>
        /// <returns>The book details + list of merchant and their prices</returns>
        /// <exception cref="ArgumentNullException">The isbn is empty</exception>
        [Route("sell"), HttpGet]
        public async Task<IHttpActionResult> SellAsync(string isbn13, int? thumbnail, CancellationToken token)
        {
            if (isbn13 == null) throw new ArgumentNullException(nameof(isbn13));
            var result = await _booksSearch.SellAsync(isbn13, thumbnail.GetValueOrDefault(150), token).ConfigureAwait(false);
            return Ok(result);
        }
    }
}