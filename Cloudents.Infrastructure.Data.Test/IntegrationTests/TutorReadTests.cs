﻿using System.Linq;
using Cloudents.Query.Tutor;
using FluentAssertions;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Xunit;
using Cloudents.Query.Users;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Data.Test.IntegrationTests
{
    [Collection("Database collection")]
    public class TutorReadTests
    {
        private readonly DatabaseFixture _fixture;

        public TutorReadTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            // _autoMock = AutoMock.GetLoose();

        }

        [Theory]
        [InlineData("xxx", 0, "IL", 5)]
        [InlineData("Economics", 638, "IL", 5)]
        [InlineData("Economics", 0, "IL", 100)]
        [InlineData("Economics", 638, "IN", 5)]
        [InlineData("xxx", 0, "IN", 5)]

        public async Task TutorListByCourseQuery_Ok(string course, long userId, string countryStr, int count)
        {
            var country = Country.FromCountry(countryStr);
            var query = new TutorListByCourseQuery(course, userId, country, count);
            var result = await _fixture.QueryBus.QueryAsync(query, default);
            foreach (var tutorCardDto in result)
            {
                //var test = Country.FromCountry(countryStr);
                //tutorCardDto.SbCountry.Should().BeEquivalentTo(test);
            }
        }


        [Theory]
        [InlineData(0, "IL", 0)]
        [InlineData(0, "IN", 0)]
        [InlineData(638, "US", 0)]

        public async Task TutorListQuery_Ok(long userId, string countryStr, int page)
        {

            var country = Country.FromCountry(countryStr);


            var query = new TutorListQuery(userId, country, page);
            var result = await _fixture.QueryBus.QueryAsync(query, default);
            result.Should().NotBeNull();
            result.Count.Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData(638)]
        public async Task TutorNotificationQuery_Ok(long userId)
        {
            var query = new TutorNotificationQuery(userId);
            var result = await _fixture.QueryBus.QueryAsync(query);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(638)]

        public async Task TutorListQuery_PageCountOk(long userId)
        {
            var query = new TutorListQuery(userId, Country.Israel, 0, int.MaxValue);
            var result = await _fixture.QueryBus.QueryAsync(query, default);
            result.Should().NotBeNull();
            var count = result.Count;
            result.Count.Should().Be(result.Result.Count());

            var query2 = new TutorListQuery(userId, Country.Israel, 0);
            var result2 = await _fixture.QueryBus.QueryAsync(query2, default);
            result2.Count.Should().Be(count);
        }

        [Theory]
        [InlineData(159039)]
        [InlineData(162107)]
        [InlineData(638)]
        [InlineData(162481)]
        public async Task TutorActionsQuery_Ok(long userId)
        {
            var country = await _fixture.StatelessSession.Query<User>().Where(w => w.Id == userId).Select(s => s.SbCountry).SingleOrDefaultAsync();
            if (country == null)
            {
               return;
            }

            var query = new TutorActionsQuery(userId, country!);
            var result = await _fixture.QueryBus.QueryAsync(query);
        }

        [Fact]
        public async Task UserCouponsQuery_Ok()
        {
            var query = new UserCouponsQuery(159039);
            var _ = await _fixture.QueryBus.QueryAsync(query, default);
        }
    }

    [Collection("Database collection")]
    public class TutorRepositoryReadTests
    {
        private readonly DatabaseFixture _fixture;

        public TutorRepositoryReadTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            // _autoMock = AutoMock.GetLoose();

        }

        [Fact]
        public async Task GetTutorsByCourseAsync_Ok()
        {
            var res = await _fixture.TutorRepository.GetTutorsByCourseAsync("פיזיקה 2", 161238, "IL", default);
            res.Should().NotBeEmpty();
        }
    }


    [Collection("Database collection")]
    public class ReadTutorRepositoryReadTests
    {
        private readonly DatabaseFixture _fixture;

        public ReadTutorRepositoryReadTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetReadTutorAsync_Ok()
        {
            var tutorId = await _fixture.StatelessSession.Query<Tutor>()
                .Where(w => w.State == ItemState.Ok).Select(s => s.Id).Take(1)
                            .SingleOrDefaultAsync();
            var res = await _fixture.ReadTutorRepository.GetReadTutorAsync(tutorId, default);
            res.Should().NotBeNull();
        }
    }


}