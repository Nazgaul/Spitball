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

        public BookSearch(IMapper mapper)
        {
            m_Mapper = mapper;
        }

        public async Task<IEnumerable<BookSearchDto>> SearchAsync(string term, int page, CancellationToken token)
        {
            //var retVal = new List<BookSearchDto>();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var uri = new UriBuilder("http://api2.campusbooks.com/12/rest/search");
                var nvc = new NameValueCollection
                {
                    ["key"] = "sP8C5AHcdiT0tsMsotT",
                    ["keywords"] = term,
                    ["page"] = page.ToString(),
                    /*   "image_height": "\(104 * UIScreen.main.scale)",
            "image_width": "\(160 * UIScreen.main.scale)" ,*/
                    ["format"] = "json"
                };
                uri.AddQuery(nvc);

                var response = await client.GetAsync(uri.Uri, token).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode) return null;
                var str = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var o = JObject.Parse(str);
                //var books = o["response"]["page"]["results"]["book"].Children() as IEnumerable<JToken>;
                return m_Mapper.Map<JObject, IEnumerable<BookSearchDto>>(o);
                /*let books = result["response"]["page"]["results"]["book"]
            return books.arrayValue.map({ (j) -> BookDto in
                return BookDto(json: j)
            })*/

                //retVal.AddRange(o["results"].Children()
                //    .Select(result => new TutorDto
                //    {
                //        Url = $"https://tutorme.com/tutors/{result["id"].Value<string>()}",
                //        Image = result["avatar"]["x300"].Value<string>(),
                //        Name = result["shortName"].Value<string>(),
                //        Online = result["isOnline"].Value<bool>(),
                //        TermFound = result.ToString().Split(new[] { term }, StringSplitOptions.RemoveEmptyEntries).Length
                //    }));
            }
            //return retVal;
        }
    }
}
