using System.Linq;
using AutoMapper;
using Cloudents.Core.DTOs;
using Newtonsoft.Json.Linq;

namespace Cloudents.Infrastructure.Converters
{
    internal class BookDetailConverter : ITypeConverter<JObject, BookDetailsDto>
    {
        public BookDetailsDto Convert(JObject source, BookDetailsDto destination, ResolutionContext context)
        {
            var book = source["response"]["page"]["books"]["book"].First;
            var offers = book["offers"]?["group"]?.SelectMany(json =>
            {
                return json["offer"].Select(s => new BookPricesDto
                {
                    Condition = s["condition"]["condition"].Value<string>(),
                    Image = s["merchant"]["image"].Value<string>(),
                    Link = s["link"].Value<string>(),
                    Name = s["merchant"]["name"].Value<string>(),
                    Price = s["price"].Value<double>()
                });
            });
            return new BookDetailsDto
            {

                Details = context.Mapper.Map<JToken, BookSearchDto>(book), //book.ToObject<BookSearchDto>(),,
                Prices = offers
            };
        }
    }
}
