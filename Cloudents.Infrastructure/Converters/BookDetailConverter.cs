using System;
using System.Linq;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Extension;
using Cloudents.Infrastructure.Search;

namespace Cloudents.Infrastructure.Converters
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
            if (string.Equals(source.Response.Status, "error", StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }
            var book = source.Response.Page.Books.Book[0];
            var offers = book.Offers?.Group?.SelectMany(json =>
            {
                return json.Offer.Select(s =>
                {
                    var merchantImage = s.Merchant.Image;
                    var uri = new Uri(merchantImage);

                    uri = uri.ChangeToHttps();
                    return new BookPricesDto
                    {
                        Condition = s.Condition.Condition,
                        Image = uri,
                        Link = s.Link,
                        Name = s.Merchant.Name,
                        Price = s.Price
                    };
                });
            });
            return new BookDetailsDto
            {

                Details = _mapper.Map<BookSearch.BookDetail, BookSearchDto>(book), //book.ToObject<BookSearchDto>(),,
                Prices = offers
            };
        }
    }
}
