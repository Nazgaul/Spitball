using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IEnumerable<TutorDto>> SearchAsync(IEnumerable<string> term, TutorRequestFilter[] filters, TutorRequestSort sort, GeoPoint location, int page,
            CancellationToken token)
        {
            //term = term ?? new[] { "economics" };

            var query = string.Join(" ", term ?? Enumerable.Empty<string>()) ?? "economics";
            if (sort == TutorRequestSort.Distance && location == null)
            {
                throw new ArgumentException("Need to location");
            }
            if (filters?.Contains(TutorRequestFilter.InPerson) == true && location == null)
                throw new ArgumentException("Need to location");
            var tasks = _tutorSearch.Select(s =>
                s.SearchAsync(query, filters, sort, location, page, token)).ToList();
            await Task.WhenAll(tasks).ConfigureAwait(false);
            return tasks.SelectMany(s => s.Result).OrderByDescending(o => o.TermCount);
        }
    }
}
