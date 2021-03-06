﻿//using Cloudents.Core.DTOs;
//using Cloudents.Core.Interfaces;
//using Cloudents.Core.Query.Feed;
//using Cloudents.Query;
//using Cloudents.Query.Documents;
//using Cloudents.Query.Tutor;
//using FluentAssertions;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Xunit;

//namespace Cloudents.Infrastructure.Test
//{
//    public class FeedSortTests
//    {
//        private readonly Mock<IQueryBus> _queryBus;
//        private readonly Mock<ITutorSearch> _tutorSearch;
//        private readonly Mock<IDocumentSearch> _documentSearch;
//        private readonly Mock<IUrlBuilder> _urlBuilder;

//        public FeedSortTests()
//        {
//            _queryBus = new Mock<IQueryBus>();
//            _tutorSearch = new Mock<ITutorSearch>();
//            _documentSearch = new Mock<IDocumentSearch>();
//            _urlBuilder = new Mock<IUrlBuilder>();
//        }

//        [Fact]
//        public async Task GetFeedAsync_First_Page_Ok()
//        {
//            IList<FeedDto> itemsFeed = Enumerable.Range(0, 18).Select(s => new DocumentFeedDto()).ToList<FeedDto>();
//            ListWithCountDto<TutorCardDto> tutorsFeed = new ListWithCountDto<TutorCardDto>()
//            {
//                Result = Enumerable.Range(0, 3).Select(s => new TutorCardDto()).ToList(),
//                Count = 3
//            };
//            _queryBus.Setup(s => s.QueryAsync(It.IsAny<TutorListQuery>(), default)).ReturnsAsync(tutorsFeed);
//            _queryBus.Setup(s => s.QueryAsync(It.IsAny<FeedAggregateQuery>(), default)).ReturnsAsync(itemsFeed);

//            var feedService = new FeedService(_queryBus.Object, _tutorSearch.Object, _documentSearch.Object, _urlBuilder.Object);

//            var res = (await feedService.GetFeedAsync(new GetFeedQuery(162107, 0, null, "IL", null), default)).ToList();

//            res[2].GetType().Should().Be(typeof(TutorCardDto));
//            res[12].GetType().Should().Be(typeof(TutorCardDto));
//            res[19].GetType().Should().Be(typeof(TutorCardDto));

//        }

//        [Fact]
//        public async Task GetFeedAsync_Null_Exception()
//        {
//            var feedService = new FeedService(_queryBus.Object, _tutorSearch.Object, _documentSearch.Object, _urlBuilder.Object);
//            //GetFeedQuery query = null;
//            //SearchFeedQuery searchQuery = null;
//            await Assert.ThrowsAsync<ArgumentNullException>(() => feedService.GetFeedAsync((GetFeedQuery)null, default));
//            await Assert.ThrowsAsync<ArgumentNullException>(() => feedService.GetFeedAsync((SearchFeedQuery)null, default));
//        }

//        [Fact]
//        public async Task GetFeedAsync_Second_Page_Ok()
//        {

//            IList<FeedDto> itemsFeed = Enumerable.Range(0, 18).Select(s => new DocumentFeedDto()).ToList<FeedDto>();
//            ListWithCountDto<TutorCardDto> tutorsFeed = new ListWithCountDto<TutorCardDto>()
//            {
//                Result = Enumerable.Range(0, 3).Select(s => new TutorCardDto()).ToList(),
//                Count = 3
//            };


//            _queryBus.Setup(s => s.QueryAsync(It.IsAny<TutorListQuery>(), default)).ReturnsAsync(tutorsFeed);
//            _queryBus.Setup(s => s.QueryAsync(It.IsAny<FeedAggregateQuery>(), default)).ReturnsAsync(itemsFeed);

//            var feedService = new FeedService(_queryBus.Object, _tutorSearch.Object, _documentSearch.Object, _urlBuilder.Object);

//            var res = (await feedService.GetFeedAsync(new GetFeedQuery(162107, 1, null, "IL", null), default)).ToList();
//            res[6].GetType().Should().Be(typeof(TutorCardDto));
//            res[13].GetType().Should().Be(typeof(TutorCardDto));
//            res[20].GetType().Should().Be(typeof(TutorCardDto));
//        }

//        [Fact]
//        public async Task GetFeedAsync_First_Page_12_Items_Ok()
//        {
//            IList<FeedDto> itemsFeed = Enumerable.Range(0, 12).Select(s => new DocumentFeedDto()).ToList<FeedDto>();
//            ListWithCountDto<TutorCardDto> tutorsFeed = new ListWithCountDto<TutorCardDto>()
//            {
//                Result = Enumerable.Range(0, 3).Select(s => new TutorCardDto()).ToList(),
//                Count = 3
//            };

//            _queryBus.Setup(s => s.QueryAsync(It.IsAny<TutorListQuery>(), default)).ReturnsAsync(tutorsFeed);
//            _queryBus.Setup(s => s.QueryAsync(It.IsAny<FeedAggregateQuery>(), default)).ReturnsAsync(itemsFeed);

//            var feedService = new FeedService(_queryBus.Object, _tutorSearch.Object, _documentSearch.Object, _urlBuilder.Object);

//            var res = (await feedService.GetFeedAsync(new GetFeedQuery(162107, 0, null, "IL", null), default)).ToList();
//            res[2].GetType().Should().Be(typeof(TutorCardDto));
//            res[12].GetType().Should().Be(typeof(TutorCardDto));
//            res[res.Count - 1].GetType().Should().Be(typeof(TutorCardDto));
//            res.Contains(null).Should().BeFalse();
//        }

//        [Fact]
//        public async Task GetFeedAsync_First_Page_No_Items_Ok()
//        {
//            IList<FeedDto> itemsFeed = new List<FeedDto>();
//            ListWithCountDto<TutorCardDto> tutorsFeed = new ListWithCountDto<TutorCardDto>()
//            {
//                Result = Enumerable.Range(0, 3).Select(s => new TutorCardDto()).ToList(),
//                Count = 3
//            };

//            _queryBus.Setup(s => s.QueryAsync(It.IsAny<TutorListQuery>(), default)).ReturnsAsync(tutorsFeed);
//            _queryBus.Setup(s => s.QueryAsync(It.IsAny<FeedAggregateQuery>(), default)).ReturnsAsync(itemsFeed);

//            var feedService = new FeedService(_queryBus.Object, _tutorSearch.Object, _documentSearch.Object, _urlBuilder.Object);

//            var res = (await feedService.GetFeedAsync(new GetFeedQuery(162107, 0, null, "IL", null), default)).ToList();
//            res[0].GetType().Should().Be(typeof(TutorCardDto));
//            res[1].GetType().Should().Be(typeof(TutorCardDto));
//            res[res.Count - 1].GetType().Should().Be(typeof(TutorCardDto));
//            res.Contains(null).Should().BeFalse();
//        }


//        [Fact]
//        public async Task GetFeedAsync_First_Page_Return_Null_Items_Ok()
//        {
//            _queryBus.Setup(s => s.QueryAsync(It.IsAny<TutorListQuery>(), default)).ReturnsAsync((ListWithCountDto<TutorCardDto>)null);
//            _queryBus.Setup(s => s.QueryAsync(It.IsAny<TutorListByCourseQuery>(), default)).ReturnsAsync((IEnumerable<TutorCardDto>)null);
//            _queryBus.Setup(s => s.QueryAsync(It.IsAny<FeedAggregateQuery>(), default)).ReturnsAsync((IList<FeedDto>)null);

//            var feedService = new FeedService(_queryBus.Object, _tutorSearch.Object, _documentSearch.Object, _urlBuilder.Object);

//            var res = (await feedService.GetFeedAsync(new GetFeedQuery(162107, 0, null, "IL", null), default))?.ToList();
//            res.Should().BeNull();
//        }




//        [Fact]
//        public async Task GetFeedAsync_First_Page_1_Tutor_Ok()
//        {

//            IList<FeedDto> itemsFeed = Enumerable.Range(0, 18).Select(s => new DocumentFeedDto()).ToList<FeedDto>();
//            ListWithCountDto<TutorCardDto> tutorsFeed = new ListWithCountDto<TutorCardDto>()
//            {
//                Result = Enumerable.Range(0, 1).Select(s => new TutorCardDto()).ToList(),
//                Count = 1
//            };

//            _queryBus.Setup(s => s.QueryAsync(It.IsAny<TutorListQuery>(), default)).ReturnsAsync(tutorsFeed);
//            _queryBus.Setup(s => s.QueryAsync(It.IsAny<FeedAggregateQuery>(), default)).ReturnsAsync(itemsFeed);

//            var feedService = new FeedService(_queryBus.Object, _tutorSearch.Object, _documentSearch.Object, _urlBuilder.Object);

//            var res = (await feedService.GetFeedAsync(new GetFeedQuery(162107, 0, null, "IL", null), default))?.ToList();

//            res[2].GetType().Should().Be(typeof(TutorCardDto));
//            res.Contains(null).Should().BeFalse();
//        }

//        [Fact]
//        public async Task GetFeedAsync_First_Page_No_Tutor_Ok()
//        {
//            IList<FeedDto> itemsFeed = Enumerable.Range(0, 18).Select(s => new DocumentFeedDto()).ToList<FeedDto>();
//            ListWithCountDto<TutorCardDto> tutorsFeed = new ListWithCountDto<TutorCardDto>();

//            _queryBus.Setup(s => s.QueryAsync(It.IsAny<TutorListQuery>(), default)).ReturnsAsync(tutorsFeed);
//            _queryBus.Setup(s => s.QueryAsync(It.IsAny<FeedAggregateQuery>(), default)).ReturnsAsync(itemsFeed);

//            var feedService = new FeedService(_queryBus.Object, _tutorSearch.Object, _documentSearch.Object, _urlBuilder.Object);

//            var res = (await feedService.GetFeedAsync(new GetFeedQuery(162107, 0, null, "IL", null), default))?.ToList();

//            res.Count.Should().Be(18);
//            res.Contains(null).Should().BeFalse();
//        }
//    }
//}
