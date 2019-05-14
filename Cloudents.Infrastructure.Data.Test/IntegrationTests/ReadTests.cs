﻿using System;
using System.Collections.Generic;
using Cloudents.Query.Documents;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Query.Query;
using Cloudents.Query.Tutor;
using FluentAssertions;
using Xunit;

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
        [Fact]
        public async Task DocumentAggregateQuery_Ok()
        {
            var query = new DocumentAggregateQuery(638, 0, null, "IL");

            var _ = await fixture.QueryBus.QueryAsync(query, default);


        }

        [Fact]
        public async Task DocumentAggregateQuery_WithFilter_Ok()
        {
            var query = new DocumentAggregateQuery(638, 0, new[] { "x", "y" }, "IL");

            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task DocumentAggregateQuery_NoUser_Ok()
        {
            var query = new DocumentAggregateQuery(0, 0, new[] { "x", "y" }, "IL");

            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task DocumentCourseQuery_Ok()
        {
            var query = new DocumentCourseQuery(638, 0, "economics", null);

            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }


        [Fact]
        public async Task DocumentCourseQuery_Filter_Ok()
        {
            var query = new DocumentCourseQuery(638, 0, "economics", new[] { "x", "y" });

            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }


        [Fact]
        public async Task StudyRoomQuery_Ok()
        {
            var query = new StudyRoomQuery(Guid.Parse("0F70AF05-BAD4-4299-8341-AA38007858CF"), 159039);

            var x = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task UserProfileAboutQuery_Ok()
        {
            var query = new UserProfileAboutQuery(638);

            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }


        [Fact]
        public async Task QuestionsQueryHandler_Ok()
        {
            var ids = new[]
            {
                9077L,
            };
            var query = new IdsQuery<long>(ids);

            var _ = await fixture.QueryBus.QueryAsync<IEnumerable<QuestionFeedDto>>(query, default);
        }


        [Fact]
        public async Task UserStudyRoomQuery_Ok()
        {
            var query = new UserStudyRoomQuery(638);

            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData(638,null,0)]
        [InlineData(638, "ארה\"ב",0)]
        [InlineData(638, "ממ\"",0)]
        [InlineData(638, "אלגברה ל", 0)]
        public async Task CourseSearchQuery_Ok(long userId,string term,int page)
        {
            var query = new CourseSearchQuery(userId, term, page);

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
            var user = await fixture.QueryBus.QueryAsync(query, default);
        }
    }
}