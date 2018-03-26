using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Search.Tutor
{
    [UsedImplicitly]
    public class TutorSearch : ITutorSearch
    {
        private readonly IEnumerable<ITutorProvider> _tutorSearch;
        private readonly ITutorSuggestion _tutorSuggestion;
        public const int PageSize = 15;
        private readonly IShuffle _shuffle;

        public TutorSearch(IEnumerable<ITutorProvider> tutorSearch, IShuffle shuffle, ITutorSuggestion tutorSuggestion)
        {
            _tutorSearch = tutorSearch;
            _shuffle = shuffle;
            _tutorSuggestion = tutorSuggestion;
        }

        [BuildLocalUrl("", PageSize, "page")]
        public async Task<IEnumerable<TutorDto>> SearchAsync(IEnumerable<string> term, TutorRequestFilter[] filters, TutorRequestSort sort, GeoPoint location, int page,
            bool isMobile, CancellationToken token)
        {
            var query = string.Join(" ", term ?? Enumerable.Empty<string>());
            query = await _tutorSuggestion.GetValueAsync(query, token).ConfigureAwait(false) ?? query;
            if (string.IsNullOrWhiteSpace(query))
            {
                query = "Physics";
            }
            if (filters?.Contains(TutorRequestFilter.InPerson) == true && location == null)
                throw new ArgumentException("Need to location");

            var tasks = _tutorSearch.Select(s => s.SearchAsync(query, filters ?? Array.Empty<TutorRequestFilter>(), sort,
                location, page, isMobile, token));

            var result2 = await Task.WhenAll(tasks).ConfigureAwait(false);
            var result = result2.Where(w => w != null).SelectMany(s => s);
            if (sort == TutorRequestSort.Price)
            {
                return result.OrderBy(o => o.Fee);
            }
            return _shuffle.DoShuffle(result);
        }
    }
}
