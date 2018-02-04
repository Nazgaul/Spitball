using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Search
{
    public class BookSearch : IBookSearch
    {
        //https://partners.campusbooks.com/api.php
        private readonly IMapper _mapper;
        private readonly IRestClient _restClient;

        private const string Key = "sP8C5AHcdiT0tsMsotT";

        public BookSearch(IMapper mapper, IRestClient restClient)
        {
            _mapper = mapper;
            _restClient = restClient;
        }

        [Cache(TimeConst.Day, "book")]
        public async Task<IEnumerable<BookSearchDto>> SearchAsync(IEnumerable<string> term, int imageWidth, int page, CancellationToken token)
        {
            var query = string.Join(" ", term ?? Enumerable.Empty<string>()) ?? "textbooks";

            var nvc = new NameValueCollection
            {
                ["key"] = Key,
                ["keywords"] = query,
                ["page"] = (++page).ToString(),
                ["f"] = "search",
                ["image_width"] = imageWidth.ToString(),
                ["format"] = "json"
            };
            var resultStr = await _restClient.GetAsync(new Uri("https://api2.campusbooks.com/13/rest/books"), nvc, token).ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<BookDetailResult>(resultStr);

            return _mapper.Map<BookDetailResult, IEnumerable<BookSearchDto>>(result);
        }

        [Cache(TimeConst.Day, "book-buy")]
        public Task<BookDetailsDto> BuyAsync(string isbn13, int imageWidth, CancellationToken token)
        {
            return BuyOrSellApiAsync(isbn13, imageWidth, false, token);
        }

        protected async Task<BookDetailsDto> BuyOrSellApiAsync(string isbn13, int imageWidth, bool sell,
            CancellationToken token)
        {
            if (isbn13 == null) throw new ArgumentNullException(nameof(isbn13));
            var nvc = new NameValueCollection
            {
                ["key"] = Key,
                ["f"] = "search,prices",
                ["isbn"] = isbn13,
                ["image_width"] = imageWidth.ToString(),
                ["format"] = "json",
                ["type"] = sell ? "buyback" : "Buy"
            };
            //https://api2.campusbooks.com/13/rest/books?key=sP8C5AHcdiT0tsMsotT&f=search,prices&format=json&isbn=9780446556224&type=buyback
            var resultStr = await _restClient.GetAsync(new Uri("https://api2.campusbooks.com/13/rest/books"), nvc, token).ConfigureAwait(false);

            var result = JsonConvert.DeserializeObject<BookDetailResult>(resultStr);

            var mappedResult = _mapper.Map<BookDetailResult, BookDetailsDto>(result);
            mappedResult.Prices = mappedResult.Prices?.OrderBy(o => o.Price);
            return mappedResult;
        }

        [Cache(TimeConst.Day, "book-sell")]
        public async Task<BookDetailsDto> SellAsync(string isbn13, int imageWidth, CancellationToken token)
        {
            var result = await BuyOrSellApiAsync(isbn13, imageWidth, true, token).ConfigureAwait(false);
            result.Prices = result.Prices?.Where(w => string.Equals(w.Condition, "used", StringComparison.InvariantCultureIgnoreCase));
            return result;
        }



        public class BookDetailResult
        {
            public Response Response { get; set; }
        }

        public class Response
        {
            public Page Page { get; set; }
        }


        public class Page
        {
            public Books Books { get; set; }
        }

        public class Books
        {
            public BookDetail[] Book { get; set; }
        }

        public class BookDetail
        {
            public string Isbn10 { get; set; }
            public string Isbn13 { get; set; }
            public string Author { get; set; }
            public string Binding { get; set; }
            public string Edition { get; set; }
            public BookImage Image { get; set; }
            public string Title { get; set; }
            public Offers Offers { get; set; }
        }

        public class BookImage
        {
            public string Image { get; set; }
        }

        public class Offers
        {
            public BookGroup[] Group { get; set; }
        }

        public class BookGroup
        {
            public BookOffer[] Offer { get; set; }
        }

        public class BookOffer
        {
            public BookCondition Condition { get; set; }
            public Merchant Merchant { get; set; }
            public double Price { get; set; }
            public string Link { get; set; }
        }

        public class BookCondition
        {
            public int Id { get; set; }
            public string Condition { get; set; }
        }

        public class Merchant
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Image { get; set; }
            public string Notes { get; set; }
        }
    }
}
