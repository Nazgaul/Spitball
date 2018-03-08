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
        public const int PageSize = 15;
        private readonly IShuffle _shuffle;

        public TutorSearch(IEnumerable<ITutorProvider> tutorSearch, IShuffle shuffle)
        {
            _tutorSearch = tutorSearch;
            _shuffle = shuffle;
        }

        [BuildLocalUrl("", PageSize, "page")]
        public async Task<IEnumerable<TutorDto>> SearchAsync(IEnumerable<string> term, TutorRequestFilter[] filters, TutorRequestSort sort, GeoPoint location, int page,
            bool isMobile, CancellationToken token)
        {
            var query = string.Join(" ", term ?? Enumerable.Empty<string>());
            if (string.IsNullOrWhiteSpace(query))
            {
                query = "economics";
            }

            //TODO: need to check if its relevant in here
            //if (sort == TutorRequestSort.Distance && location == null)
            //{
            //    throw new ArgumentException("Need to location");
            //}
            if (filters?.Contains(TutorRequestFilter.InPerson) == true && location == null)
                throw new ArgumentException("Need to location");

            var tasks = _tutorSearch.Select(s => s.SearchAsync(query, filters ?? new TutorRequestFilter[0], sort,
                location, page, isMobile, token));

            var result2 = await Task.WhenAll(tasks);
            var result = result2.Where(w => w != null).SelectMany(s => s);
            //var p = result2.Aggregate((b, next) =>
            //{
            //    return b.Union(next);
            //});


            //var result = await _tutorSearch.SelectManyAsync(s => s.SearchAsync(query, filters ?? new TutorRequestFilter[0], sort, location, page, isMobile, token)).ConfigureAwait(false);
            if (sort == TutorRequestSort.Price)
            {
                return result.OrderBy(o => o.Fee);
            }
            return _shuffle.DoShuffle(result);
        }
    }
}
