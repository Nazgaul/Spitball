using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;

namespace Cloudents.Infrastructure.Search
{
    public class TutorSearch : ITutorSearch
    {
        private readonly IEnumerable<ITutorProvider> m_TutorSearch;

        public TutorSearch(IEnumerable<ITutorProvider> tutorSearch)
        {
            m_TutorSearch = tutorSearch;
        }

        public async Task<IEnumerable<TutorDto>> SearchAsync(string term, SearchRequestFilter filter, SearchRequestSort sort, GeoPoint location, int page,
            CancellationToken token)
        {
            var tasks = m_TutorSearch.Select(s =>
                s.SearchAsync(term, filter, sort, location, page, token)).ToList();
            await Task.WhenAll(tasks).ConfigureAwait(false);

            return tasks.SelectMany(s => s.Result).OrderByDescending(o => o.TermCount);
        }
    }
}
