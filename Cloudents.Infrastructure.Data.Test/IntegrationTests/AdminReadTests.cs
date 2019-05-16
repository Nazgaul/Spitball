﻿using System.Threading.Tasks;
using Cloudents.Query.Query.Admin;
using Xunit;

namespace Cloudents.Infrastructure.Data.Test.IntegrationTests
{
    [Collection("Database collection")]
    public class AdminReadTests
    {
        DatabaseFixture fixture;

        public AdminReadTests(DatabaseFixture fixture)
        {
            this.fixture = fixture;
            // _autoMock = AutoMock.GetLoose();

        }


        [Fact]
        public async Task DocumentAggregateQuery_Ok()
        {
            var query = new AdminUserAnswersQuery(638, 0);

            var result = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminPaymentsQuery_Ok()
        {
            var query = new AdminPaymentsQuery();

            var result = await fixture.QueryBus.QueryAsync(query, default);
        }
    }
}
