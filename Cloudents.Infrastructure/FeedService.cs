using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Core.Query;
using Cloudents.Core.Query.Feed;
using Cloudents.Query;
using Cloudents.Query.Documents;
using Cloudents.Query.Questions;
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
        private const int _tutorPageSize = 3;
        private const int _itemPageSize = 18;

        public FeedService(IQueryBus queryBus, ITutorSearch tutorSearch, IDocumentSearch searchProvider)
        {
            _queryBus = queryBus;
            _tutorSearch = tutorSearch;
            _searchProvider = searchProvider;
        }

        private IEnumerable<FeedDto> SortFeed(IList<FeedDto> itemsFeed, IList<TutorCardDto> tutorsFeed, int page)
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

        private Task<IEnumerable<TutorCardDto>> GetTutorsFeedAsync(long userId, string country, int page, string course, int tutorPageSize, CancellationToken token)
        {
            Task<IEnumerable<TutorCardDto>> tutorsTask;

            if (string.IsNullOrEmpty(course))
            {
                var tutorQuery = new TutorListQuery(userId, country, page, tutorPageSize);
                tutorsTask = _queryBus.QueryAsync(tutorQuery, token);
            }
            else
            {
                var tutorQuery = new TutorListByCourseQuery(course, userId, country, tutorPageSize, page);
                tutorsTask = _queryBus.QueryAsync(tutorQuery, token);
            }
            return tutorsTask;
        }

        public async Task<IEnumerable<FeedDto>> GetFeedAsync(GetFeedQuery query, CancellationToken token)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (query.Filter == null)
            {
                var feedQuery = new FeedAggregateQuery(query.UserId, query.Page, new string[] { }, query.Country, query.Course, _itemPageSize);

                Task<IEnumerable<TutorCardDto>> tutorsTask = GetTutorsFeedAsync(query.UserId, query.Country, query.Page, query.Course, _tutorPageSize, token);
               
                var itemsTask = _queryBus.QueryAsync(feedQuery, token);
                await Task.WhenAll(itemsTask, tutorsTask);
                return SortFeed(itemsTask.Result?.ToList(), tutorsTask.Result?.ToList(), query.Page);
            }

            if (query.Filter == Core.Enum.FeedType.Tutor)
            {
                Task<IEnumerable<TutorCardDto>> tutorsTask = GetTutorsFeedAsync(query.UserId, query.Country, query.Page, query.Course, 21, token);
                await Task.WhenAll(tutorsTask);
                return tutorsTask.Result;
            }

            if (query.Filter == Core.Enum.FeedType.Document || query.Filter == Core.Enum.FeedType.Video)
            {
                var feedQuery = new DocumentFeedWithFliterQuery(query.Page, query.UserId, query.Filter, query.Country, query.Course, 21);
                var itemsFeed = await _queryBus.QueryAsync(feedQuery, token);
                //Query docs/video
                return itemsFeed;
            }

            if (query.Filter == Core.Enum.FeedType.Question)
            {
                var feedQuery = new QuestionFeedWithFliterQuery(query.Page, query.UserId, query.Country, query.Course, 21);
                var itemsFeed = await _queryBus.QueryAsync(feedQuery, token);
                //Question query
                return itemsFeed;
            }

          
            return null;
            
            
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

        
            var feedQuery = new DocumentQuery(query.Profile, termToQuery, query.Course, _itemPageSize, null)
            {
                Page = query.Page,
            };

            var tutorQuery = new TutorListTabSearchQuery(termToQuery, query.Country, query.Page, _tutorPageSize);
            var tutorTask = _tutorSearch.SearchAsync(tutorQuery, token);
            var resultTask = _searchProvider.SearchDocumentsAsync(feedQuery, token);


            await Task.WhenAll(resultTask, tutorTask);
            var result = SortFeed(resultTask.Result?.ToList(), tutorTask.Result?.ToList(), query.Page);
            return result;
 
        }
    }
}
