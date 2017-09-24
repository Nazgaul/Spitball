using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Search.Entities;
using Microsoft.Azure.Search;

namespace Cloudents.Infrastructure.Search
{
    public class TutorSearch : ITutorSearch
    {
        private readonly ISearchIndexClient m_Client;
        private readonly IMapper m_Mapper;

        public TutorSearch(SearchServiceClient client, IMapper mapper)
        {
            m_Mapper = mapper;
            m_Client = client.Indexes.GetClient("tutors");
        }


        public async Task<IEnumerable<TutorDto>> SearchAsync(string term, CancellationToken token)
        {
            var result = await
                m_Client.Documents.SearchAsync<Tutor>(term, cancellationToken: token).ConfigureAwait(false);
            return m_Mapper.Map<IEnumerable<TutorDto>>(result.Results.Select(s => s.Document));
        }

    }
}
