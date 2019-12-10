﻿using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Core.Query.Feed;
using Cloudents.Query;
using Cloudents.Query.Documents;
using Cloudents.Query.Tutor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure
{
    public class FeedService : IFeedService
    {
        private readonly IQueryBus _queryBus;
        private readonly ITutorSearch _tutorSearch;
        private readonly IDocumentSearch _searchProvider;
        private const int TutorPageSize = 3;
        private const int ItemPageSize = 18;

        public FeedService(IQueryBus queryBus, ITutorSearch tutorSearch, IDocumentSearch searchProvider)
        {
            _queryBus = queryBus;
            _tutorSearch = tutorSearch;
            _searchProvider = searchProvider;
        }

        private static IEnumerable<FeedDto> SortFeed(IList<FeedDto> itemsFeed, IList<TutorCardDto> tutorsFeed, int page)
        {
            if (itemsFeed == null)
            {
                return tutorsFeed;
            }
            if (tutorsFeed == null)
            {
                return itemsFeed;
            }

            var tutorlocationPageZero = new[] { 2, 12, 19 };
            var tutorlocationPage = new[] { 6, 13, 20 };

            var locations = new[] { tutorlocationPageZero, tutorlocationPage };

            var pageLocations = locations.ElementAtOrDefault(page) ?? tutorlocationPage;


            foreach (var item in pageLocations)
            {
                var tutor = tutorsFeed.FirstOrDefault();
                if (tutor is null)
                {
                    break;
                }
                itemsFeed.Insert(Math.Min(itemsFeed.Count, item), tutor);
                tutorsFeed.RemoveAt(0);
            }

            return itemsFeed;
        }

        public async Task<IEnumerable<FeedDto>> GetFeedAsync(GetFeedQuery query, CancellationToken token)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var feedQuery = new FeedAggregateQuery(query.UserId, query.Page, query.Filter, query.Country, query.Course, ItemPageSize);
            Task<IEnumerable<TutorCardDto>> tutorsTask;
            var itemsTask = _queryBus.QueryAsync(feedQuery, token);

            //Task<ListWithCountDto<TutorCardDto>> tutorsTask = Task.FromResult<ListWithCountDto<TutorCardDto>>(null);

            if (string.IsNullOrEmpty(query.Course))
            {
                var tutorQuery = new TutorListQuery(query.UserId, query.Country, query.Page, TutorPageSize);
                var task  = _queryBus.QueryAsync(tutorQuery, token);
                await Task.WhenAll(itemsTask, task);

                return SortFeed(itemsTask.Result?.ToList(),
                    task.Result.Result.ToList(),
                    query.Page);
            }
        
            else
            {
                var tutorQuery = new TutorListByCourseQuery(query.Course, query.UserId, query.Country, TutorPageSize, query.Page);
                tutorsTask = _queryBus.QueryAsync(tutorQuery, token);

                await Task.WhenAll(itemsTask, tutorsTask);

                return SortFeed(itemsTask.Result?.ToList(),
                    tutorsTask.Result.ToList(),
                    query.Page);
            }
        }


        public async Task<IEnumerable<FeedDto>> GetFeedAsync(SearchFeedQuery query, CancellationToken token)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            string termToQuery;
            if (!string.IsNullOrEmpty(query.Course))
            {
                termToQuery = $"{query.Term} {query.Course}".Trim();
            }
            else
            {
                termToQuery = query.Term.Trim();
            }
            var feedQuery = new DocumentQuery(query.Profile, query.Term, query.Course, ItemPageSize, query.Filter?.Where(w => !string.IsNullOrEmpty(w)))
            {
                Page = query.Page,
            };

            var tutorQuery = new TutorListTabSearchQuery(query.Term, query.Country, query.Page, TutorPageSize);
            var tutorTask = _tutorSearch.SearchAsync(tutorQuery, token);
            var resultTask = _searchProvider.SearchDocumentsAsync(feedQuery, token);


            await Task.WhenAll(resultTask, tutorTask);
            var result = SortFeed(resultTask.Result?.ToList(), tutorTask.Result?.Result?.ToList(), query.Page);
            return result;
        }
    }
}
