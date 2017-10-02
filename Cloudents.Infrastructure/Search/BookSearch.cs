using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Newtonsoft.Json.Linq;

namespace Cloudents.Infrastructure.Search
{
    public class BookSearch : IBookSearch
    {
        private readonly IMapper m_Mapper;
        private readonly IRestClient m_RestClient;

        public BookSearch(IMapper mapper, IRestClient restClient)
        {
            m_Mapper = mapper;
            m_RestClient = restClient;
        }

        public async Task<IEnumerable<BookSearchDto>> SearchAsync(string term, int page, CancellationToken token)
        {
            //var retVal = new List<BookSearchDto>();
            var nvc = new NameValueCollection
            {
                ["key"] = "sP8C5AHcdiT0tsMsotT",
                ["keywords"] = term,
                ["page"] = page.ToString(),
                /*   "image_height": "\(104 * UIScreen.main.scale)",
        "image_width": "\(160 * UIScreen.main.scale)" ,*/
                ["format"] = "json"
            };
            var result = await m_RestClient.GetAsync(new Uri("http://api2.campusbooks.com/12/rest/search"), nvc, token).ConfigureAwait(false);
            return m_Mapper.Map<JObject, IEnumerable<BookSearchDto>>(result);
        }
    }
}
