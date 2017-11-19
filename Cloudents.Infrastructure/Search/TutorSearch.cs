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
        private readonly IEnumerable<ITutorProvider> _tutorSearch;

        public TutorSearch(IEnumerable<ITutorProvider> tutorSearch)
        {
            _tutorSearch = tutorSearch;
        }

        // filter = FilterSortDto(filters: [.all,.online,.inPerson], sortArr: [.relevance, .price,.distance,.rating])
        public async Task<IEnumerable<TutorDto>> SearchAsync(string term, TutorRequestFilter filter, TutorRequestSort sort, GeoPoint location, int page,
            CancellationToken token)
        {
            if (sort == TutorRequestSort.Distance && location == null)
            {
                throw new ArgumentException("Need to location");
            }
            if (filter == TutorRequestFilter.InPerson && location == null)
                throw new ArgumentException("Need to location");
            var tasks = _tutorSearch.Select(s =>
                s.SearchAsync(term, filter, sort, location, page, token)).ToList();
            await Task.WhenAll(tasks).ConfigureAwait(false);
            return tasks.SelectMany(s => s.Result).OrderByDescending(o => o.TermCount);
        }
    }
}
