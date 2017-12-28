using System;
using System.Linq;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Extension;
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
                return json["offer"].Select(s =>
                {
                    var merchantImage = s["merchant"]["image"].Value<string>();
                    var uri = new Uri(merchantImage);

                    uri = uri.ChangeToHttps();
                    //Uri uri = null;
                    //if (merchantImage != null)
                    //{
                    //    var uriBuilder = new UriBuilder(merchantImage)
                    //    {
                    //        Scheme = Uri.UriSchemeHttps,
                    //        Port = -1
                    //    };
                    //    uri = uriBuilder.Uri;
                    //}
                    return new BookPricesDto
                    {
                        Condition = s["condition"]["condition"].Value<string>(),
                        Image = uri,
                        Link = s["link"].Value<string>(),
                        Name = s["merchant"]["name"].Value<string>(),
                        Price = s["price"].Value<double>()
                    };
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
