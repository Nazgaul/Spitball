﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cloudents.Query.Documents;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Query;
using Cloudents.Query.Chat;
using Cloudents.Query.Email;
using Cloudents.Query.HomePage;
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
        [InlineData(0, 0, new[] { "x", "y" }, "IL", null)]
        [InlineData(0, 0, new[] { "x", "y" }, "IL", "economics")]
        [InlineData(638, 0, new[] { "x", "y" }, "IL", "economics")]
        [InlineData(638, 1, new[] { "x", "y" }, "IL", null)]

        public async Task DocumentAggregateQuery_Ok(long userId, int page, string[] filter, string country, string course)
        {
            var query = new FeedAggregateQuery(userId, page, filter, country, course, 18);

            var result = await fixture.QueryBus.QueryAsync(query, default);
            result.Should().NotBeNullOrEmpty();

            result.OfType<QuestionFeedDto>().Should().Contain(c => c.User.Id > 0);
            result.OfType<QuestionFeedDto>().Should().Contain(c => c.CultureInfo != null);

        }

        [Theory]
        [InlineData(50084, 638)]
        public async Task DocumentById_Ok(long documentId, long? userId)
        {
            var query = new DocumentById(documentId, userId);

            var result = await fixture.QueryBus.QueryAsync(query, default);
            result.Should().NotBeNull();
        }




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
        public async Task UserQuestionFeedDtoQueryHandler_Ok()
        {
            var query = new UserDataPagingByIdQuery(638, 0);
            var _ = await fixture.QueryBus.QueryAsync<IEnumerable<QuestionFeedDto>>(query, default);
        }

        [Fact]
        public async Task UserAnswerFeedDtoQueryHandler_Ok()
        {
            var query = new UserAnswersByIdQuery(638, 0);
            var _ = await fixture.QueryBus.QueryAsync<IEnumerable<QuestionFeedDto>>(query, default);
        }


        [Theory]
        [InlineData(1)]
        [InlineData(50084)]
        [InlineData(50050)]
        public async Task DocumentsQueryHandler_Ok(long documentId)
        {
            var query = new IdsDocumentsQuery(new[] { documentId });
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

        [Theory]
        [InlineData(638)]
        [InlineData(11)]
        [InlineData(605)]
        [InlineData(36)]
        [InlineData(160105)]
        [InlineData(150713)]
        [InlineData(160446)]
        [InlineData(161238)]

        public async Task UserProfileQuery_Ok(long id)
        {
            var query = new UserProfileQuery(id);

            var result = await fixture.QueryBus.QueryAsync(query, default);

            result.Should().NotBeNull();
            
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

        [Fact]
        public async Task QuestionDataByIdQuery_Ok()
        {
            var query = new QuestionDataByIdQuery(10626);
            _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData(638,"IL")]
        [InlineData(1696, "IN")]
        public async Task UserBalanceQuery_Ok(long id,string country)
        {
            
            var query = new UserBalanceQuery(id);
            var result = await fixture.QueryBus.QueryAsync(query, default);

           
        }

        [Fact]
        public async Task GetUpdatesEmailUsersQuery_Ok()
        {
            var query = new GetUpdatesEmailUsersQuery(DateTime.UtcNow.AddDays(-1), 0);
            var result = await fixture.QueryBus.QueryAsync(query, default);

        }

        [Theory]
        [InlineData("IL")]
        [InlineData("IN")]
        public async Task TopTutorsQuery_Ok(string country)
        {
            var query = new TopTutorsQuery(country,12);
            _ = await fixture.QueryBus.QueryAsync(query, default);
        }


        [Theory]
        [InlineData("IL")]
        [InlineData("IN")]
        public async Task TopDocumentsQuery_Ok(string country)
        {
            var query = new TopDocumentsQuery(country, 5);
            _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task StatsQuery_Ok()
        {
            var query = new StatsQuery();
            _ = await fixture.QueryBus.QueryAsync(query, default);
        }


        [Theory]
        [InlineData("IL")]
        [InlineData("IN")]
        public async Task ReviewsQuery_Ok(string country)
        {
            var query = new ReviewsQuery(country, 5);
            _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        


    }
}