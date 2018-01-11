using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Interfaces;
using Cloudents.Mobile.Extensions;
using Cloudents.Mobile.Filters;
using Cloudents.Mobile.Models;
using Microsoft.Azure.Mobile.Server.Config;

namespace Cloudents.Mobile.Controllers
{
    /// <summary>
    /// The book api controller
    /// </summary>

    [MobileAppController, RoutePrefix("api/book")]
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
        /// Search book vertical end point with next page token add to query string api-version = 2017-12-19
        /// </summary>
        /// <param name="bookRequest"></param>
        /// <param name="token"></param>
        /// <returns>List of book</returns>
        /// <exception cref="ArgumentNullException">term cannot be empty</exception>
        [VersionedRoute("search", "2017-12-19", Name = "BookSearch"), HttpGet]
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

        
    }
}