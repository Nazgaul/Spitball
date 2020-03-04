using Cloudents.Core.Interfaces;
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
using Autofac.Features.Indexed;
using Cloudents.Core.Enum;
using Cloudents.Core.DTOs.Feed;

namespace Cloudents.Infrastructure
{
    public class FeedService : IFeedService
    {

        private readonly IIndex<FeedType, IFeedTypeService> _services;

        public FeedService( IIndex<FeedType, IFeedTypeService> services)
        {
            _services = services;
        }



        public async Task<IEnumerable<FeedDto>> GetFeedAsync(GetFeedQuery query, CancellationToken token)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return await _services[query.Filter].GetFeedAsync(query, token);

            //if (query.Filter is null)
            //{
            //    var feedQuery = new FeedAggregateQuery(query.UserId, query.Page, query.Country, query.Course, ItemPageSize);
            //    var itemsTask = _queryBus.QueryAsync(feedQuery, token);
            //    var tutorsTask = GetTutorsFeedAsync(query.UserId, query.Country, query.Page, query.Course, TutorPageSize, token);

            //    await Task.WhenAll(itemsTask, tutorsTask);

            //    return SortFeed(itemsTask.Result.ToList(),
            //        tutorsTask.Result.ToList(),
            //        query.Page);
            //}


            //if (query.Filter == Core.Enum.FeedType.Tutor)
            //{
            //    return await GetTutorsFeedAsync(query.UserId, query.Country, query.Page, query.Course, 21, token);
            //    //await Task.WhenAll(tutorsTask);
            //    //return tutorsTask.Result;
            //}

            //if (query.Filter == Core.Enum.FeedType.Document || query.Filter == Core.Enum.FeedType.Video)
            //{
            //    var feedQuery = new DocumentFeedWithFliterQuery(query.Page, query.UserId, query.Filter, query.Country, query.Course, 21);
            //    var itemsFeed = await _queryBus.QueryAsync(feedQuery, token);
            //    //Query docs/video
            //    return itemsFeed;
            //}

            //if (query.Filter == Core.Enum.FeedType.Question)
            //{
            //    var feedQuery = new QuestionFeedWithFliterQuery(query.Page, query.UserId, query.Country, query.Course, 21);
            //    var itemsFeed = await _queryBus.QueryAsync(feedQuery, token);
            //    //Question query
            //    return itemsFeed;
            //}


            //return null;


        }

        public async Task<IEnumerable<FeedDto>> GetFeedAsync(GetFeedWithCourseQuery query, CancellationToken token)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return await _services[query.Filter].GetFeedAsync(query, token);
        }


        public async Task<IEnumerable<FeedDto>> GetFeedAsync(SearchFeedQuery query, CancellationToken token)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return await _services[query.Filter].GetFeedAsync(query, token);


            //if (query is null)
            //{
            //    throw new ArgumentNullException(nameof(query));
            //}

            //string termToQuery;
            //if (!string.IsNullOrEmpty(query.Course))
            //{
            //    termToQuery = $"{query.Term} {query.Course}".Trim();
            //}
            //else
            //{
            //    termToQuery = query.Term.Trim();
            //}
            //var feedQuery = new DocumentQuery(query.Profile, query.Term, query.Course, ItemPageSize, query.Filter);
            ////if (query.Filter == null)
            ////{
            ////    var feedQuery = new DocumentQuery(query.Profile, termToQuery, query.Course, ItemPageSize, query.Filter)
            ////    {
            ////        Page = query.Page,
            ////    };
            ////}
            //var tutorQuery = new TutorListTabSearchQuery(termToQuery, query.Country, query.Page, TutorPageSize);
            //var tutorTask = _tutorSearch.SearchAsync(tutorQuery, token);
            //var resultTask = _searchProvider.SearchDocumentsAsync(feedQuery, token);


            //await Task.WhenAll(resultTask, tutorTask);
            //var result = SortFeed(resultTask.Result?.ToList(), tutorTask.Result?.Result?.ToList(), query.Page);
            //return result;


            //if (query.Filter == Core.Enum.FeedType.Tutor)
            //{
            //    var tutorQuery = new TutorListTabSearchQuery(termToQuery, query.Country, query.Page, 21);
            //    var tutorsTask = _tutorSearch.SearchAsync(tutorQuery, token);
            //    await Task.WhenAll(tutorsTask);
            //    return tutorsTask.Result;
            //}

            //if (query.Filter == Core.Enum.FeedType.Document || query.Filter == Core.Enum.FeedType.Video)
            //{
            //    var feedQuery = new DocumentQuery(query.Profile, termToQuery, query.Course, 21, query.Filter)
            //    {
            //        Page = query.Page,
            //    };
            //    var resultTask = _searchProvider.SearchDocumentsAsync(feedQuery, token);
            //    await Task.WhenAll(resultTask);
            //    //Query docs/video
            //    return resultTask.Result;
            //}

            //return Enumerable.Empty<FeedDto>();
        }
    }

    public class AggregateFeedService : IFeedTypeService
    {
        private readonly IQueryBus _queryBus;
        private const int TutorPageSize = 3;
        private const int ItemPageSize = 18;



        private readonly TutorFeedService _tutorFeedService;
        private readonly DocumentFeedService _documentFeedService;

        public AggregateFeedService(IQueryBus queryBus, TutorFeedService.Factory tutorFeedService, DocumentFeedService.Factory documentFactory)
        {
            _queryBus = queryBus;
            _tutorFeedService = tutorFeedService.Invoke(TutorPageSize);
            _documentFeedService = documentFactory.Invoke(ItemPageSize);
        }

        public async Task<IEnumerable<FeedDto>> GetFeedAsync(GetFeedQuery query, CancellationToken token)
        {
            var feedQuery = new FeedAggregateQuery(query.UserId, query.Page, query.Country, null, ItemPageSize);
            var itemsTask = _queryBus.QueryAsync(feedQuery, token);


            var tutorsTask = _tutorFeedService.GetFeedAsync(query, token);
            await Task.WhenAll(itemsTask, tutorsTask);

            return SortFeed(itemsTask.Result.ToList(),
                tutorsTask.Result.ToList(),
                query.Page);
        }

        public async Task<IEnumerable<FeedDto>> GetFeedAsync(GetFeedWithCourseQuery query, CancellationToken token)
        {
            var feedQuery = new FeedAggregateQuery(query.UserId, query.Page, query.Country, query.Course, ItemPageSize);
            var itemsTask = _queryBus.QueryAsync(feedQuery, token);


            var tutorsTask = _tutorFeedService.GetFeedAsync(query, token);
            await Task.WhenAll(itemsTask, tutorsTask);

            return SortFeed(itemsTask.Result.ToList(),
                tutorsTask.Result.ToList(),
                query.Page);
        }

        public async Task<IEnumerable<FeedDto>> GetFeedAsync(SearchFeedQuery query, CancellationToken token)
        {
            var itemsTask = _documentFeedService.GetFeedAsync(query, token);
            var tutorsTask = _tutorFeedService.GetFeedAsync(query, token);

            await Task.WhenAll(itemsTask, tutorsTask);

            return SortFeed(itemsTask.Result.ToList(),
                tutorsTask.Result.ToList(),
                query.Page);
        }


        private static IEnumerable<FeedDto> SortFeed(IList<FeedDto> itemsFeed, IList<FeedDto> tutorsFeed, int page)
        {

            if (itemsFeed is null)
            {
                return tutorsFeed;
            }
            if (tutorsFeed is null)
            {
                return itemsFeed;
            }

            //foreach (var item in itemsFeed)
            //{
            //    if (item is DocumentFeedDto d)
            //    {
            //        if (d.User != null)
            //        {
            //            d.User.Image = _urlBuilder.BuildUserImageEndpoint(d.User.Id, d.User.Image);
            //        }
            //    }
            //    else if (item is QuestionFeedDto q)
            //    {
            //        q.User.Image = _urlBuilder.BuildUserImageEndpoint(q.User.Id, q.User.Image);
            //        if (q.FirstAnswer != null)
            //        {
            //            q.FirstAnswer.User.Image = _urlBuilder.BuildUserImageEndpoint(q.FirstAnswer.User.Id, q.FirstAnswer.User.Image);
            //        }
            //    }
            //}


            var tutorLocationPageZero = new[] { 2, 12, 19 };
            var tutorLocationPage = new[] { 6, 13, 20 };

            var locations = new[] { tutorLocationPageZero, tutorLocationPage };

            var pageLocations = locations.ElementAtOrDefault(page) ?? tutorLocationPage;


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
    }


    public class DocumentFeedService : IFeedTypeService
    {
        private readonly IQueryBus _queryBus;
        private readonly IDocumentSearch _searchProvider;
        private readonly int _pageSize;

        public DocumentFeedService(IQueryBus queryBus, IDocumentSearch searchProvider, int pageSize = 21)
        {
            _queryBus = queryBus;
            _searchProvider = searchProvider;
            _pageSize = pageSize;
        }

        public delegate DocumentFeedService Factory(int pageSize);



        public Task<IEnumerable<FeedDto>> GetFeedAsync(GetFeedQuery query, CancellationToken token)
        {
            var newQuery = GetFeedWithCourseQuery.FromBase(query);
            return GetFeedAsync(newQuery, token);
        }

        public async Task<IEnumerable<FeedDto>> GetFeedAsync(GetFeedWithCourseQuery query, CancellationToken token)
        {
            var feedQuery = new DocumentFeedWithFilterQuery(query.Page, query.UserId, query.Filter,
                query.Country, query.Course, _pageSize);
            return await _queryBus.QueryAsync(feedQuery, token);
        }

        public async Task<IEnumerable<FeedDto>> GetFeedAsync(SearchFeedQuery query, CancellationToken token)
        {
            DocumentType? filter = null;
            switch (query.Filter)
            {
                case FeedType.Document:
                    filter = DocumentType.Document;
                    break;
                case FeedType.Video:
                    filter = DocumentType.Video;
                    break;
                    //default:
                    //    //throw new ArgumentOutOfRangeException();
            }
            var documentQuery = new DocumentQuery(query.Profile, query.Term, query.Course, query.Page, _pageSize, filter);
            return await _searchProvider.SearchDocumentsAsync(documentQuery, token);
        }
    }

    public class TutorFeedService : IFeedTypeService
    {
        private readonly IQueryBus _queryBus;
        private readonly ITutorSearch _tutorSearch;
        private readonly int _pageSize;

        public delegate TutorFeedService Factory(int pageSize);

        public TutorFeedService(IQueryBus queryBus, ITutorSearch tutorSearch, int pageSize = 21)
        {
            _queryBus = queryBus;
            _tutorSearch = tutorSearch;
            _pageSize = pageSize;
        }



        public async Task<IEnumerable<FeedDto>> GetFeedAsync(GetFeedQuery query, CancellationToken token)
        {
            var tutorQuery = new TutorListQuery(query.UserId, query.Country, query.Page, _pageSize);
            return await _queryBus.QueryAsync(tutorQuery, token).ContinueWith(r => r.Result.Result, token);
        }

        public async Task<IEnumerable<FeedDto>> GetFeedAsync(GetFeedWithCourseQuery query, CancellationToken token)
        {
            var tutorQuery = new TutorListByCourseQuery(query.Course, query.UserId, query.Country, _pageSize, query.Page);
            return await _queryBus.QueryAsync(tutorQuery, token);
        }

        public async Task<IEnumerable<FeedDto>> GetFeedAsync(SearchFeedQuery query, CancellationToken token)
        {
            string termToQuery;
            if (!string.IsNullOrEmpty(query.Course))
            {
                termToQuery = $"{query.Term} {query.Course}".Trim();
            }
            else
            {
                termToQuery = query.Term.Trim();
            }
            var tutorQuery = new TutorListTabSearchQuery(termToQuery, query.Country, query.Page, _pageSize);
            return await _tutorSearch.SearchAsync(tutorQuery, token).ContinueWith(r => r.Result.Result, token);
        }
    }

    public class QuestionFeedService : IFeedTypeService
    {
        private readonly IQueryBus _queryBus;

        public QuestionFeedService(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        public async Task<IEnumerable<FeedDto>> GetFeedAsync(GetFeedQuery query, CancellationToken token)
        {
            var feedQuery = new QuestionFeedWithFliterQuery(query.Page, query.UserId, query.Country, null, 21);
            return await _queryBus.QueryAsync(feedQuery, token);
        }

        public async Task<IEnumerable<FeedDto>> GetFeedAsync(GetFeedWithCourseQuery query, CancellationToken token)
        {
            var feedQuery = new QuestionFeedWithFliterQuery(query.Page, query.UserId, query.Country, query.Course, 21);
            return await _queryBus.QueryAsync(feedQuery, token);
        }

        public Task<IEnumerable<FeedDto>> GetFeedAsync(SearchFeedQuery query, CancellationToken token)
        {
            return Task.FromResult(Enumerable.Empty<FeedDto>());
        }
    }
}
