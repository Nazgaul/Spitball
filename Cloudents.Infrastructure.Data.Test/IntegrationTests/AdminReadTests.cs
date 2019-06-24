using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Enum;
using Cloudents.Query;
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
            await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminConversationDetailsQuery_Ok()
        {
            var query = new AdminConversationDetailsQuery("638_159039");
            await fixture.QueryBus.QueryAsync(query, default);
        }
        [Fact]
        public async Task AdminConversationsQuery_Ok()
        {
            var query1 = new AdminConversationsQuery(159039, 0, null, null);
            var query2 = new AdminConversationsQuery(159039, 0, ChatRoomStatus.Default, ChatRoomAssign.None);

            var task1 = fixture.QueryBus.QueryAsync(query1, default);
            var task2 = fixture.QueryBus.QueryAsync(query2, default);
            await Task.WhenAll(task1, task2);
        }

        [Fact]
        public async Task AdminCoursesQuery_Ok()
        {
            var query = new AdminConversationDetailsQuery("638_159039");
            await fixture.QueryBus.QueryAsync(query, default);
        }

        //TODO: check all AdminEmptyQuery handlers
     

        [Fact]
        public async Task AdminLanguageQuery_Courses_Ok()
        {
            var queryHe = new AdminLanguageQuery("he");
            var queryNull = new AdminLanguageQuery(null);
            var t1 = fixture.QueryBus.QueryAsync<IList<PendingCoursesDto>>(queryHe, default);
            var t2 = fixture.QueryBus.QueryAsync<IList<PendingCoursesDto>>(queryNull, default);
            await Task.WhenAll(t1, t2);
        }



        [Fact]
        public async Task AdminLanguageQuery_University_Ok()
        {
            var queryHe = new AdminLanguageQuery("il");
            var queryNull = new AdminLanguageQuery(null);
            var t1 = fixture.QueryBus.QueryAsync<IList<PendingUniversitiesDto>>(queryHe, default);
            var t2 = fixture.QueryBus.QueryAsync<IList<PendingUniversitiesDto>>(queryNull, default);
            await Task.WhenAll(t1, t2);
        }
        

        //[Fact]
        //public async Task AdminPageQuery_QuestionWithoutCorrectAnswer_Ok()
        //{
        //    var query = new AdminPageQuery(0);
        //    await fixture.QueryBus.QueryAsync(query, default);
        //}

        [Fact]
        public async Task AdminPendingTutorsQuery_Ok()
        {
            var query = new AdminPendingTutorsQuery();
            await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminSessionsQuery_Ok()
        {
            var query = new AdminSessionsQuery(159039);
            await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminStudyRoomQuery_Ok()
        {
            var query = new AdminStudyRoomQuery();
            await fixture.QueryBus.QueryAsync(query, default);
        }


        [Fact]
        public async Task AdminSubjectsQuery_Ok()
        {
            var query = new AdminSubjectsQuery();
            await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminUniversitiesQuery_Ok()
        {
            var query = new AdminUniversitiesQuery("IL", ItemState.Ok);
            var query2 = new AdminUniversitiesQuery(null, ItemState.Ok);
            var t1 = fixture.QueryBus.QueryAsync(query, default);
            var t2 = fixture.QueryBus.QueryAsync(query2, default);
            await Task.WhenAll(t1, t2);
        }
    }
}
