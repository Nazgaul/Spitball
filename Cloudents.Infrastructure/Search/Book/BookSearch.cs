using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Extension;

namespace Cloudents.Infrastructure.Search.Book
{
    [UsedImplicitly]
    public class BookSearch : IBookSearch
    {
        //https://partners.campusbooks.com/api.php
        private readonly IRestClient _restClient;

        private const string Key = "sP8C5AHcdiT0tsMsotT";
        private const string Url = "https://api2.campusbooks.com/13/rest/books";

        public BookSearch(IRestClient restClient)
        {
            _restClient = restClient;
        }

        [Cache(TimeConst.Day, "book", false)]
        public async Task<IEnumerable<BookSearchDto>> SearchAsync(IEnumerable<string> term, int page, CancellationToken token)
        {
            var query = string.Join(" ", term ?? new[] { "textbooks" });

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

                return result.Response.Page.Books?.Book.Select(jo => new BookSearchDto
                {
                    Image = jo.Image?.Image,
                    Author = jo.Author,
                    Binding = jo.Binding,
                    Edition = jo.Edition,
                    Isbn10 = jo.Isbn10,
                    Isbn13 = jo.Isbn13,
                    Title = jo.Title

                });
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

            var book = result.Response.Page.Books.Book[0];
            var bookDetail = new BookSearchDto
            {
                Image = book.Image?.Image,
                Author = book.Author,
                Binding = book.Binding,
                Edition = book.Edition,
                Isbn10 = book.Isbn10,
                Isbn13 = book.Isbn13,
                Title = book.Title

            };
            var offers = book.Offers?.Group?.SelectMany(json =>
            {
                return json.Offer.Select(s =>
                {
                    var link = s.Link;
                    Enum.TryParse(s.Condition.Condition, true, out Core.DTOs.BookCondition condition);

                    link = ChangeUrlIfNeeded(s.Merchant.Name, bookDetail.Isbn13, condition) ?? link;
                    var merchantImage = s.Merchant.Image;
                    var uri = new Uri(merchantImage);

                    uri = uri.ChangeToHttps();
                    return new BookPricesDto
                    {
                        Condition = condition,
                        Image = uri,
                        Link = link,
                        Name = s.Merchant.Name,
                        Price = s.Price
                    };
                });
            });
            return new BookDetailsDto
            {

                Details = bookDetail,
                Prices = offers?.OrderBy(o=>o.Price)
            };
           
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


        private string ChangeUrlIfNeeded(string merchantName, string isbn13, Core.DTOs.BookCondition condition)
        {
            var function = _convert.Where(w => merchantName.Contains(w.Key, StringComparison.OrdinalIgnoreCase)).Select(s => s.Value).FirstOrDefault();
            var newUrl = function?.Invoke(isbn13, condition);
            return newUrl;
        }

        private readonly Dictionary<string, Func<string, Core.DTOs.BookCondition, string>> _convert =
            new Dictionary<string, Func<string, Core.DTOs.BookCondition, string>>(StringComparer.OrdinalIgnoreCase)
            {
                {"ValoreBooks", BuildValoreBooksLink},
                {"Chegg", BuildCheggTextBookRental}
            };

        /// <summary>
        /// Change to chegg textbook rental
        /// </summary>
        /// <param name="isbn13"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        /// <remarks>bug 9923</remarks>
        private static string BuildCheggTextBookRental(string isbn13, Core.DTOs.BookCondition condition)
        {
            if (condition == Core.DTOs.BookCondition.Rental)
            {
                return $"http://chggtrx.com/click.track?CID=267582&AFID=418708&ADID=1088031&SID=&isbn_ean={isbn13}";
            }

            return null;
        }

        private static string BuildValoreBooksLink(string isbn13, Core.DTOs.BookCondition condition)
        {
            var type = string.Empty;
            switch (condition)
            {
                case Core.DTOs.BookCondition.None:
                    break;
                case Core.DTOs.BookCondition.New:
                    type = "new";
                    break;
                case Core.DTOs.BookCondition.Rental:
                    type = "rent";
                    break;
                case Core.DTOs.BookCondition.EBook:
                    break;
                case Core.DTOs.BookCondition.Used:
                    type = "used";
                    break;
            }

            var retVal = $"http://www.valorebooks.com/affiliate/buy/siteID=s7peQB/ISBN={isbn13}";
            if (!string.IsNullOrEmpty(type))
            {
                retVal += $"?default={type}";
            }

            return retVal;
        }
    }
}
