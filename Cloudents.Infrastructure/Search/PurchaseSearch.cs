using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Newtonsoft.Json.Linq;

namespace Cloudents.Infrastructure.Search
{
    public class PurchaseSearch : IPurchaseSearch
    {
        private readonly IMapper m_Mapper;
        private readonly IRestClient m_RestClient;

        private const string key = "AIzaSyAhNIR9O5bBnPZoB0lm5qRNeNN6EzjTTBg";

        public PurchaseSearch(IRestClient restClient, IMapper mapper)
        {
            m_RestClient = restClient;
            m_Mapper = mapper;
        }

        public async Task<IEnumerable<PlaceDto>> SearchAsync(string term, SearchRequestFilter filter,
            GeoPoint location, CancellationToken token)
        {
            if (term == null) throw new ArgumentNullException(nameof(term));
            if (location == null) throw new ArgumentNullException(nameof(location));
            var nvc = new NameValueCollection
            {
                ["location"] = $"{location.Latitude} {location.Longitude}",
                ["keyword"] = term,
                ["key"] = key,
                ["rankby"] = "distance"
            };
            if (filter == SearchRequestFilter.OpenNow)
            {
                nvc.Add("opennow", true.ToString());
            }
            var result = await m_RestClient.GetAsync(new Uri("https://maps.googleapis.com/maps/api/place/nearbysearch/json"), nvc, token).ConfigureAwait(false);
            return m_Mapper.Map<JObject, IEnumerable<PlaceDto>>(result, opt =>
            {
                opt.Items["width"] = 150;
                opt.Items["key"] = key;
            });
        }
    }
}
