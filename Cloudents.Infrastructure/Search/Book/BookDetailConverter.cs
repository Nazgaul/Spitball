using System;
using System.Linq;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Extension;

namespace Cloudents.Infrastructure.Search.Book
{
    internal class BookDetailConverter : ITypeConverter<BookSearch.BookDetailResult, BookDetailsDto>
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
                    if (string.Equals(s.Merchant.Name, "ValoreBooks.com", StringComparison.InvariantCultureIgnoreCase))
                    {
                        link = BuildValoreBooksLink(bookDetail.Isbn13, condition);
                    }
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

            var retVal =  $"http://www.valorebooks.com/affiliate/buy/siteID=s7peQB/ISBN={isbn13}";
            if (!string.IsNullOrEmpty(type))
            {
                retVal += $"?default={type}";
            }

            return retVal;
        }
    }
}
