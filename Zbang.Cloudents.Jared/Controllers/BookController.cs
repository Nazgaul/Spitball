using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.Jared.Models;

namespace Zbang.Cloudents.Jared.Controllers
{
    /// <summary>
    /// The book api controller
    /// </summary>
    [MobileAppController,RoutePrefix("api/book")]
    public class BookController : ApiController
    {
        private readonly IBookSearch _booksSearch;

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
        /// <exception cref="ArgumentNullException">term cannot be empty</exception>
        [Route("search")]
        public async Task<HttpResponseMessage> Get([FromUri]BookRequest bookRequest, CancellationToken token)
        {
            if (bookRequest.Term == null) throw new ArgumentNullException(nameof(bookRequest.Term));
            var result = await _booksSearch.SearchAsync(string.Join(" ", bookRequest.Term), bookRequest.Thumbnail.GetValueOrDefault(150), bookRequest.Page.GetValueOrDefault(), token).ConfigureAwait(false);
            return Request.CreateResponse(result);
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
        public async Task<HttpResponseMessage> BuyAsync(string isbn13, int? thumbnail, CancellationToken token)
        {
            if (isbn13 == null) throw new ArgumentNullException(nameof(isbn13));
            var result = await _booksSearch.BuyAsync(isbn13, thumbnail.GetValueOrDefault(150), token).ConfigureAwait(false);
            return Request.CreateResponse(result);
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
        public async Task<HttpResponseMessage> SellAsync(string isbn13, int? thumbnail, CancellationToken token)
        {
            if (isbn13 == null) throw new ArgumentNullException(nameof(isbn13));
            var result = await _booksSearch.SellAsync(isbn13, thumbnail.GetValueOrDefault(150), token).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }
    }
}