using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;
using Newtonsoft.Json.Linq;

namespace Cloudents.Infrastructure.Search.Tutor
{
    public class TutorMeSearch : ITutorProvider
    {
        private readonly IMapper _mapper;
        private readonly IRestClient _restClient;

        public TutorMeSearch(IMapper mapper, IRestClient restClient)
        {
            _mapper = mapper;
            _restClient = restClient;
        }

        public Task<IEnumerable<TutorDto>> SearchAsync(string term, TutorRequestFilter[] filters,
            TutorRequestSort sort, GeoPoint location, int page, CancellationToken token)
        {
            if (Array.TrueForAll(filters, t => t == TutorRequestFilter.InPerson))
            {
                return Task.FromResult(Enumerable.Empty<TutorDto>());
            }
            return TutorMeApiAsync(term, page, token);
        }

        private async Task<IEnumerable<TutorDto>> TutorMeApiAsync(string term, int page, CancellationToken token)
        {
            //https://gist.github.com/barbuza/4b3666fa88cd326f18f2c464c8e4487c
            // page is 12

            var nvc = new NameValueCollection
            {
                ["search"] = term,
                ["offset"] = (page * 12).ToString()
            };
            var result = await _restClient.GetJsonAsync(new Uri("https://tutorme.com/api/v1/tutors/"), nvc, token).ConfigureAwait(false);
            return _mapper.Map<JObject, IEnumerable<TutorDto>>(result, opt => opt.Items["term"] = term);
        }
    }
}
