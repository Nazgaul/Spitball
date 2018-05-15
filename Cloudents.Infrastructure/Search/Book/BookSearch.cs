using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using Newtonsoft.Json;
using IMapper = AutoMapper.IMapper;

namespace Cloudents.Infrastructure.Search.Book
{
    [UsedImplicitly]
    public class BookSearch : IBookSearch
    {
        //https://partners.campusbooks.com/api.php
        private readonly IMapper _mapper;
        private readonly IRestClient _restClient;

        private const string Key = "sP8C5AHcdiT0tsMsotT";
        private const string Url = "https://api2.campusbooks.com/13/rest/books";
        public BookSearch(IMapper mapper, IRestClient restClient)
        {
            _mapper = mapper;
            _restClient = restClient;
        }

        [Cache(TimeConst.Day, "book", false)]
        public async Task<IEnumerable<BookSearchDto>> SearchAsync(IEnumerable<string> term, int page, CancellationToken token)
        {
            var query = string.Join(" ", term ?? new [] { "textbooks" });

            var nvc = new NameValueCollection
            {
                ["key"] = Key,
                ["keywords"] = query,
                ["page"] = (++page).ToString(),
                ["f"] = "search",
                ["image_width"] = 150.ToString(),
                ["format"] = "json"
            };
            var result = await MakeApiCallAsync(nvc, token).ConfigureAwait(false);
            if (ValidateSearchResult(page, result))
            {
                return _mapper.Map<BookDetailResult, IEnumerable<BookSearchDto>>(result);
            }
            return null;
        }

        private static bool ValidateSearchResult(int page, BookDetailResult result)
        {
            if (result?.Response.Page.Books?.TotalPages == null)
            {
                return false;
            }

            if (result.Response.Page.Books.TotalPages < page)
            {
                return false;
            }

            return true;
        }

        private async Task<BookDetailResult> MakeApiCallAsync(NameValueCollection nvc, CancellationToken token)
        {
            var result = await _restClient.GetAsync<BookDetailResult>(new Uri(Url), nvc, token).ConfigureAwait(false);
            if (result == null)
            {
                return null;
            }
            if (string.Equals(result.Response.Status, "error", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            return result;
        }

        [BuildLocalUrl(nameof(BookDetailsDto.Prices))]
        [Cache(TimeConst.Minute * 5, "book-buy", false)]

        public Task<BookDetailsDto> BuyAsync(string isbn13, CancellationToken token)
        {
            return BuyOrSellApiAsync(isbn13, false, token);
        }

        private async Task<BookDetailsDto> BuyOrSellApiAsync(string isbn13, bool sell,
            CancellationToken token)
        {
            if (isbn13 == null) throw new ArgumentNullException(nameof(isbn13));
            var nvc = new NameValueCollection
            {
                ["key"] = Key,
                ["f"] = "search,prices",
                ["isbn"] = isbn13,
                ["image_width"] = 150.ToString(),
                ["format"] = "json",
                ["type"] = sell ? "buyback" : "Buy"
            };
            //https://api2.campusbooks.com/13/rest/books?key=sP8C5AHcdiT0tsMsotT&f=search,prices&format=json&isbn=9780446556224&type=buyback

            var result = await MakeApiCallAsync(nvc, token).ConfigureAwait(false);
            if (result == null)
            {
                return null;
            }
            var mappedResult = _mapper.Map<BookDetailResult, BookDetailsDto>(result);
            //if (mappedResult == null)
            //{
            //    return null;
            //}
            mappedResult.Prices = mappedResult.Prices?.OrderBy(o => o.Price);
            return mappedResult;
        }

        [BuildLocalUrl(nameof(BookDetailsDto.Prices))]
        [Cache(TimeConst.Minute * 5, "book-sell", false)]
        public async Task<BookDetailsDto> SellAsync(string isbn13, CancellationToken token)
        {
            var result = await BuyOrSellApiAsync(isbn13, true, token).ConfigureAwait(false);
            if (result == null)
            {
                return null;
            }
            result.Prices = result.Prices?.Where(w => w.Condition == Core.DTOs.BookCondition.Used);
            return result;
        }

        public class BookDetailResult
        {
            public Response Response { get; set; }
        }

        public class Response
        {
            public Page Page { get; set; }
            public string Status { get; [UsedImplicitly] set; }
        }

        public class Page
        {
            public Books Books { get; set; }
        }

        public class Books
        {
            public BookDetail[] Book { get; [UsedImplicitly] set; }
            [JsonProperty("total_pages")]
            public int TotalPages { get; set; }
        }

        [UsedImplicitly]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
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

        [UsedImplicitly]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class BookImage
        {
            public string Image { get; set; }
        }


        [UsedImplicitly]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Offers
        {
            public BookGroup[] Group { get; set; }
        }

        [UsedImplicitly]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class BookGroup
        {
            public BookOffer[] Offer { get; set; }
        }

        [UsedImplicitly]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class BookOffer
        {
            public BookCondition Condition { get; set; }
            public Merchant Merchant { get; set; }
            public double Price { get; set; }
            public string Link { get; set; }
        }

        [UsedImplicitly]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class BookCondition
        {
            //public int Id { get; set; }
            public string Condition { get; set; }
        }
        [UsedImplicitly]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Merchant
        {
            //public string Id { get; set; }
            public string Name { get; set; }
            public string Image { get; set; }
            //public string Notes { get; set; }
        }
    }
}
