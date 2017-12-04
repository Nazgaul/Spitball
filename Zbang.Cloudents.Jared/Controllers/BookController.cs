using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Mobile.Server.Config;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController,RoutePrefix("api/book")]
    public class BookController : ApiController
    {
        private readonly IBookSearch _booksSearch;

        public BookController(IBookSearch booksSearch)
        {
            _booksSearch = booksSearch;
        }


        [Route("search")]
        public async Task<HttpResponseMessage> Get([FromUri]string[] term, int page, int? thumbnail, CancellationToken token)
        {
            if (term == null) throw new ArgumentNullException(nameof(term));
            var result = await _booksSearch.SearchAsync(string.Join(" ", term), thumbnail.GetValueOrDefault(150), page, token).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }

        [Route("buy"), HttpGet]
        public async Task<HttpResponseMessage> BuyAsync(string isbn13, int? thumbnail, CancellationToken token)
        {
            if (isbn13 == null) throw new ArgumentNullException(nameof(isbn13));
            var result = await _booksSearch.BuyAsync(isbn13, thumbnail.GetValueOrDefault(150), token).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }

        [Route("sell"), HttpGet]
        public async Task<HttpResponseMessage> SellAsync(string isbn13, int? thumbnail, CancellationToken token)
        {
            if (isbn13 == null) throw new ArgumentNullException(nameof(isbn13));
            var result = await _booksSearch.SellAsync(isbn13, thumbnail.GetValueOrDefault(150), token).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }
    }
}