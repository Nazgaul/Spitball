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
    public class PlacesSearch : IPlacesSearch
    {
        private readonly IMapper m_Mapper;
        private readonly IRestClient m_RestClient;

        private const string Key = "AIzaSyAoFR5uWJy1cf76q-J46EoEbFVZCaLk93w";//"AIzaSyAhNIR9O5bBnPZoB0lm5qRNeNN6EzjTTBg";

        public PlacesSearch(IRestClient restClient, IMapper mapper)
        {
            m_RestClient = restClient;
            m_Mapper = mapper;
        }

        public async Task<IEnumerable<PlaceDto>> SearchNearbyAsync(string term, SearchRequestFilter filter,
            GeoPoint location, CancellationToken token)
        {
            if (term == null) throw new ArgumentNullException(nameof(term));
            if (location == null) throw new ArgumentNullException(nameof(location));
            var nvc = new NameValueCollection
            {
                ["location"] = $"{location.Longitude} {location.Latitude}",
                ["keyword"] = term,
                ["key"] = Key,
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
                opt.Items["key"] = Key;
            });
        }

        public async Task<PlaceDto> SearchAsync(string term, CancellationToken token)
        {
            if (term == null) throw new ArgumentNullException(nameof(term));

            var nvc = new NameValueCollection
            {
                ["query"] = term,
                ["key"] = Key,
            };


            var result = await m_RestClient.GetAsync(new Uri("https://maps.googleapis.com/maps/api/place/textsearch/json"), nvc, token).ConfigureAwait(false);
            var mapperResult = m_Mapper.Map<JObject, IEnumerable<PlaceDto>>(result, opt =>
           {
               opt.Items["width"] = 150;
               opt.Items["key"] = Key;
           });
            return mapperResult.FirstOrDefault();

        }
    }
}
