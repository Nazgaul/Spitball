﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cloudents.Query.Documents;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Query.Chat;
using Cloudents.Query.Query;
using Cloudents.Query.Tutor;
using FluentAssertions;
using Xunit;
using Cloudents.Query.SearchSync;

namespace Cloudents.Infrastructure.Data.Test.IntegrationTests
{
    [Collection("Database collection")]
    public class ReadTests
    {
        //private readonly DapperRepository _dapperRepository;
        //private readonly AutoMock _autoMock;
        //private readonly IQueryBus _queryBus;
        // private readonly IContainer _container;
        DatabaseFixture fixture;

        public ReadTests(DatabaseFixture fixture)
        {
            this.fixture = fixture;
            // _autoMock = AutoMock.GetLoose();

        }

        //[Fact]
        //public async Task ChatConversationByIdQuery_Ok()
        //{
        //    var query = new ChatConversationByIdQuery(638, 0, null, "IL");

        //    var _ = await fixture.QueryBus.QueryAsync(query, default);


        //}

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task TutorSyncAzureSearchQuery_Ok(int version)
        {
            var query = new TutorSyncAzureSearchQuery(version);
            var query2 = new TutorSyncAzureSearchQuery(version);

            var _ = await fixture.QueryBus.QueryAsync(query, default);
            await fixture.QueryBus.QueryAsync(query2, default);


        }

        [Theory]
        [InlineData(638, 0, null, "IL", null)]
        [InlineData(638, 0, new[] { "x", "y" }, "IL", null)]
        [InlineData(0, 0, new[] { "x", "y" }, "IL",null)]
        [InlineData(0, 0, new[] { "x", "y" }, "IL", "economics")]
        [InlineData(638, 0, new[] { "x", "y" }, "IL", "economics")]
        [InlineData(638, 1, new[] { "x", "y" }, "IL", null)]
        
        public async Task DocumentAggregateQuery_Ok(long userId, int page, string[] filter, string country ,string course)
        {
            var query = new FeedAggregateQuery(userId, page, filter, country,course);

            await fixture.QueryBus.QueryAsync(query, default);


            result.OfType<QuestionFeedDto>().Should().Contain(c => c.UserId > 0);
            result.OfType<QuestionFeedDto>().Should().Contain(c => c.CultureInfo != null);
            //.All(a => a.UserId.Should().BeGreaterThan(0));
            //var dictionary = new Dictionary<string,bool>();
            //foreach (var x in result.Result)
            //{
            //    var values = x.AsDictionary();
            //    foreach (var value in values)
            //    {

            //        if (value.Value == default)
            //        {
            //            dictionary.TryAdd(value.Key, true);
            //            //dictionary.Add(value.Key,true);

            //            //resultOfTest = true;
            //        }
            //    }
            //}

            //var resultTests = dictionary.Where(w => w.Value).Select(s => s.Key).ToList();
            //resultTests.Should().BeEmpty();
        }


        //[Fact]
        //public async Task DocumentCourseQuery_Ok()
        //{
        //    var query = new DocumentCourseQuery(638, 0, "economics", null, "IL");

        //    var _ = await fixture.QueryBus.QueryAsync(query, default);
        //}


        //[Fact]
        //public async Task DocumentCourseQuery_Filter_Ok()
        //{
        //    var query = new DocumentCourseQuery(638, 0, "economics", new[] { "x", "y" }, "IL");

        //    var _ = await fixture.QueryBus.QueryAsync(query, default);
        //}


        [Fact]
        public async Task StudyRoomQuery_Ok()
        {
            var query = new StudyRoomQuery(Guid.Parse("0F70AF05-BAD4-4299-8341-AA38007858CF"), 159039);

            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task UserProfileAboutQuery_Ok()
        {
            var query = new UserProfileAboutQuery(638);

            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task UserDocumentsQueryHandler_Ok()
        {
            var query = new UserDataPagingByIdQuery(638, 0);
            var _ = await fixture.QueryBus.QueryAsync<IEnumerable<DocumentFeedDto>>(query, default);
        }

        [Fact]
        public async Task DocumentsQueryHandler_Ok()
        {
            var query = new IdsDocumentsQuery(new[] { 1L });
            var _ = await fixture.QueryBus.QueryAsync(query, default);

        }


        [Fact]
        public async Task QuestionsQueryHandler_Ok()
        {
            var ids = new[]
            {
                9077L,
            };
            var query = new IdsQuestionsQuery<long>(ids);

            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }


        [Fact]
        public async Task UserStudyRoomQuery_Ok()
        {
            var query = new UserStudyRoomQuery(638);

            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }


        [Fact]
        public async Task CourseSearchQuery_Ok()
        {
            var query = new CourseSearchQuery(638, 0);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData(638, "ארה\"ב", 0)]
        [InlineData(638, "ממ\"", 0)]
        [InlineData(638, "אלגברה ל", 0)]
        public async Task CourseSearchWithTermQuery_Ok(long userId, string term, int page)
        {
            var query = new CourseSearchWithTermQuery(userId, term, page);

            var _ = await fixture.QueryBus.QueryAsync(query, default);

            _.Should().HaveCountGreaterOrEqualTo(1);
        }

        [Fact]

        public async Task UserProfileQuery_Ok()
        {
            var query = new UserProfileQuery(638);

            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AccountUserDataById_Ok()
        {
            var query = new UserAccountQuery(638);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }


        [Fact]
        public async Task UserUnreadMessage_Ok()
        {
            var query = new UserUnreadMessageQuery(null);
            var result = await fixture.QueryBus.QueryAsync(query, default);


            query = new UserUnreadMessageQuery(result.First().Version);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task UserPurchaseDocumentByIdQuery_Ok()
        {
            var query = new UserPurchaseDocumentByIdQuery(638, 0);
            _ = await fixture.QueryBus.QueryAsync(query, default);

        }


    }
}