﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Mobile.Extensions;
using Cloudents.Mobile.Models;
using Microsoft.Azure.Mobile.Server.Config;

namespace Cloudents.Mobile.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// The book api controller
    /// </summary>

    [MobileAppController, RoutePrefix("api/book")]
    public class BookController : ApiController
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

        public async Task<IHttpActionResult> Get([FromUri]BookRequest bookRequest, CancellationToken token)
        {
            bookRequest = bookRequest ?? new BookRequest();
            var result = (await _booksSearch.SearchAsync(bookRequest.Term,  bookRequest.Page.GetValueOrDefault(), token).ConfigureAwait(false)).ToListIgnoreNull();

            string nextPageLink = null;
            if (result.Count > 0)
            {
                nextPageLink = Url.NextPageLink("BookSearch", null, bookRequest);
            }
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
        /// <param name="token"></param>
        /// <returns>The book details + list of merchant and their prices</returns>
        /// <exception cref="ArgumentNullException">The isbn is empty</exception>
        [Route("buy"), HttpGet]
        public async Task<IHttpActionResult> BuyAsync(string isbn13,  CancellationToken token)
        {
            if (isbn13 == null) throw new ArgumentNullException(nameof(isbn13));
            var result = await _booksSearch.BuyAsync(isbn13,  token).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Sell book api
        /// </summary>
        /// <param name="isbn13">the book serial number</param>
        /// <param name="token"></param>
        /// <returns>The book details + list of merchant and their prices</returns>
        /// <exception cref="ArgumentNullException">The isbn is empty</exception>
        [Route("sell"), HttpGet]
        public async Task<IHttpActionResult> SellAsync(string isbn13,  CancellationToken token)
        {
            if (isbn13 == null) throw new ArgumentNullException(nameof(isbn13));
            var result = await _booksSearch.SellAsync(isbn13,  token).ConfigureAwait(false);
            return Ok(result);
        }
    }
}