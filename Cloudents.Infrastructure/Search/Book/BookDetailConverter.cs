using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Extension;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Search.Book
{
    [UsedImplicitly]
    public class BookDetailConverter : ITypeConverter<BookSearch.BookDetailResult, BookDetailsDto>
    {
        private readonly IMapper _mapper;

        public BookDetailConverter(IMapper mapper)
        {
            _mapper = mapper;
        }

        public BookDetailsDto Convert(BookSearch.BookDetailResult source, BookDetailsDto destination, ResolutionContext context)
        {
            var book = source.Response.Page.Books.Book[0];
            var bookDetail = _mapper.Map<BookSearch.BookDetail, BookSearchDto>(book);
            var offers = book.Offers?.Group?.SelectMany(json =>
            {
                return json.Offer.Select(s =>
                {
                    var link = s.Link;
                    Enum.TryParse(s.Condition.Condition, true, out BookCondition condition);

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
                Prices = offers
            };
        }

        
        private string ChangeUrlIfNeeded(string merchantName, string isbn13, BookCondition condition)
        {
            var function = _convert.Where(w => merchantName.Contains(w.Key, StringComparison.OrdinalIgnoreCase)).Select(s => s.Value).FirstOrDefault();
            var newUrl = function?.Invoke(isbn13, condition);
            return newUrl;
        }

        private readonly Dictionary<string, Func<string, BookCondition, string>> _convert =
            new Dictionary<string, Func<string, BookCondition, string>>(StringComparer.OrdinalIgnoreCase)
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
        private static string BuildCheggTextBookRental(string isbn13, BookCondition condition)
        {
            if (condition == BookCondition.Rental)
            {
                return $"http://chggtrx.com/click.track?CID=267582&AFID=418708&ADID=1088031&SID=&isbn_ean={isbn13}";
            }

            return null;
        }

        private static string BuildValoreBooksLink(string isbn13, BookCondition condition)
        {
            var type = string.Empty;
            switch (condition)
            {
                case BookCondition.None:
                    break;
                case BookCondition.New:
                    type = "new";
                    break;
                case BookCondition.Rental:
                    type = "rent";
                    break;
                case BookCondition.EBook:
                    break;
                case BookCondition.Used:
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
