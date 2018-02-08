using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;

namespace Cloudents.Infrastructure.Search.Tutor
{
    public class TutorSearch : ITutorSearch
    {
        private readonly IEnumerable<ITutorProvider> _tutorSearch;
        public const int PageSize = 15;
        private readonly IUrlRedirectBuilder _urlRedirectBuilder;


        public TutorSearch(IEnumerable<ITutorProvider> tutorSearch, IUrlRedirectBuilder urlRedirectBuilder)
        {
            _tutorSearch = tutorSearch;
            _urlRedirectBuilder = urlRedirectBuilder;
        }

        public async Task<IEnumerable<TutorDto>> SearchAsync(IEnumerable<string> term, TutorRequestFilter[] filters, TutorRequestSort sort, GeoPoint location, int page,
            CancellationToken token)
        {
            var query = string.Join(" ", term ?? Enumerable.Empty<string>());
            if (string.IsNullOrWhiteSpace(query))
            {
                query = "economics";
            }
            if (sort == TutorRequestSort.Distance && location == null)
            {
                throw new ArgumentException("Need to location");
            }
            if (filters?.Contains(TutorRequestFilter.InPerson) == true && location == null)
                throw new ArgumentException("Need to location");
            var tasks = _tutorSearch.Select(s =>
                s.SearchAsync(query, filters, sort, location, page, token)).ToList();
            await Task.WhenAll(tasks).ConfigureAwait(false);
            var result = tasks.SelectMany(s => s.Result);//.OrderByDescending(o => o.TermCount);
            return _urlRedirectBuilder.BuildUrl(result, page, PageSize);
        }
    }
}
