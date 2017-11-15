using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Newtonsoft.Json.Linq;

namespace Cloudents.Infrastructure.Search
{
    public class BookSearch : IBookSearch
    {
        private readonly IMapper m_Mapper;
        private readonly IRestClient m_RestClient;

        private const string Key = "sP8C5AHcdiT0tsMsotT";

        public BookSearch(IMapper mapper, IRestClient restClient)
        {
            m_Mapper = mapper;
            m_RestClient = restClient;
        }

        [Cache(TimeConst.Day, "book")]
        public async Task<IEnumerable<BookSearchDto>> SearchAsync(string term, int imageWidth, int page, CancellationToken token)
        {
            var nvc = new NameValueCollection
            {
                ["key"] = Key,
                ["keywords"] = term,
                ["page"] = (++page).ToString(),
                ["f"] = "search",
                ["image_width"] = imageWidth.ToString(),
                ["format"] = "json"
            };
            var result = await m_RestClient.GetJsonAsync(new Uri("http://api2.campusbooks.com/13/rest/books"), nvc, token).ConfigureAwait(false);
            return m_Mapper.Map<JObject, IEnumerable<BookSearchDto>>(result);
        }

        public Task<BookDetailsDto> BuyAsync(string isbn13, int imageWidth, CancellationToken token)
        {
            return BuyOrSellApiAsync(isbn13, imageWidth, false, token);
        }

        private async Task<BookDetailsDto> BuyOrSellApiAsync(string isbn13, int imageWidth, bool sell,
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
            //http://api2.campusbooks.com/13/rest/books?key=sP8C5AHcdiT0tsMsotT&f=search,prices&format=json&isbn=9780446556224&type=buyback
            var result = await m_RestClient.GetJsonAsync(new Uri("http://api2.campusbooks.com/13/rest/books"), nvc, token).ConfigureAwait(false);
            return m_Mapper.Map<JObject, BookDetailsDto>(result);
        }

        public Task<BookDetailsDto> SellAsync(string isbn13, int imageWidth, CancellationToken token)
        {
            return BuyOrSellApiAsync(isbn13, imageWidth, true, token);
        }
    }
}
