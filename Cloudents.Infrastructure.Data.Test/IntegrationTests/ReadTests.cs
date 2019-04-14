﻿using System.Data.SqlClient;
using Cloudents.Query.Documents;
using System.Threading.Tasks;
using Cloudents.Query.Query;
using Xunit;

namespace Cloudents.Infrastructure.Data.Test.IntegrationTests
{
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

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
            var query = new DocumentAggregateQuery(638, 0,null);

            var result = await fixture._queryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task DocumentAggregateQuery_WithFilter_Ok()
        {
            var query = new DocumentAggregateQuery(638, 0, new []{"x", "y" });

            var result = await fixture._queryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task DocumentCourseQuery_Ok()
        {
            var query = new DocumentCourseQuery(638, 0, "economics",  null);

            var result = await fixture._queryBus.QueryAsync(query, default);
        }


        [Fact]
        public async Task DocumentCourseQuery_Filter_Ok()
        {
            var query = new DocumentCourseQuery(638, 0, "economics", new[] { "x", "y" });

            var result = await fixture._queryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task UserProfileAboutQuery_Ok()
        {
            var query = new UserProfileAboutQuery(638);

            var result = await fixture._queryBus.QueryAsync(query, default);
        }
    }
}