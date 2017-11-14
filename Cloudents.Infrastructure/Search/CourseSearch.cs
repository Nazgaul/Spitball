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
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Search
{
    public class CourseSearch : ICourseSearch
    {
        private readonly ISearchIndexClient m_Client;
        private readonly IMapper m_Mapper;

        public CourseSearch(ISearchServiceClient client, IMapper mapper)
        {
            m_Client = client.Indexes.GetClient("box2");
            m_Mapper = mapper;
        }

        public async Task<IEnumerable<CourseDto>> SearchAsync(string term, long universityId,
            CancellationToken token)
        {
            var result = await m_Client.Documents.SearchAsync<Course>(term + "*", new SearchParameters
            {
                Select = new[] { "id", "name2", "course2" },
                SearchFields = new[] { "course2", "name2" },
                Filter = $"universityId eq {universityId}"
            }, cancellationToken: token).ConfigureAwait(false);

            return m_Mapper.Map<IEnumerable<Course>, IEnumerable<CourseDto>>(result.Results.Select(s => s.Document));
        }
    }
}
