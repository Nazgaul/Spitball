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


        [Fact]
        public async Task AdminPageQuery_QuestionWithoutCorrectAnswer_Ok()
        {
            var query = new AdminQuestionWithoutCorrectAnswerPageQuery(0);
            await fixture.QueryBus.QueryAsync(query, default);
        }

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

        [Fact]
        public async Task AdminUserAnswersQuery_Ok()
        {
            var query = new AdminUserAnswersQuery(159039, 0);
            await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminUserDetailsQuery_Ok()
        {
            var q1 = new AdminUserDetailsQuery(159039.ToString());
            var q2 = new AdminUserDetailsQuery("Hadar@cloudents.com");
            var q3 = new AdminUserDetailsQuery("0523556456");
            var t1 =  fixture.QueryBus.QueryAsync(q1, default);
            var t2 =  fixture.QueryBus.QueryAsync(q2, default);
            var t3 =  fixture.QueryBus.QueryAsync(q3, default);
            await Task.WhenAll(t1, t2, t3);
        }

        [Fact]
        public async Task AdminUserDocumentsQuery_Ok()
        {
            var query = new AdminUserDocumentsQuery(159039, 0);
            await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminUserFlagsOthersQuery_Ok()
        {
            var q1 = new AdminUserFlagsOthersQuery(3, 0);
            var q2 = new AdminUserFlagsOthersQuery(2, 2);
            var t1 = fixture.QueryBus.QueryAsync(q1, default);
            var t2 = fixture.QueryBus.QueryAsync(q2, default);
            await Task.WhenAll(t1, t2);
        }

        [Fact]
        public async Task AdminUserPurchasedDocsQuery_Ok()
        {
            var query = new AdminUserPurchasedDocsQuery(159039, 0);
            await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminUserQuestionsQuery_Ok()
        {
            var query = new AdminUserQuestionsQuery(159039, 0);
            await fixture.QueryBus.QueryAsync(query, default);
        }
        [Fact]
        public async Task AdminAllCoursesEmptyQuery_Ok()
        {
            var query = new AdminAllCoursesEmptyQuery();
            await fixture.QueryBus.QueryAsync(query, default);
        }
        [Fact]
        public async Task AdminAllUniversitiesEmptyQuery_Ok()
        {
            var query = new AdminAllUniversitiesEmptyQuery();
            await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task FlaggedQuestionEmptyQuery_Ok()
        {
            var query = new FlaggedQuestionEmptyQuery();
            await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task FlaggedDocumentEmptyQuery_Ok()
        {
            var query = new FlaggedDocumentEmptyQuery();
            await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task FlaggedAnswerEmptyQuery_Ok()
        {
            var query = new FlaggedAnswerEmptyQuery();
            await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task PendingDocumentEmptyQuery_Ok()
        {
            var query = new PendingDocumentEmptyQuery();
            await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task SuspendedUsersEmptyQuery_Ok()
        {
            var query = new SuspendedUsersEmptyQuery();
            await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task FictivePendingQuestionEmptyQuery_Ok()
        {
            var query = new AdminFictivePendingQuestionEmptyQuery();
            await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task PendingQuestionsEmptyQuery_Ok()
        {
            var query = new AdminPendingQuestionsEmptyQuery();
            await fixture.QueryBus.QueryAsync(query, default);
        }
        [Fact]
        public async Task AdminCashOutEmptyQuery_Ok()
        {
            var query = new AdminCashOutEmptyQuery();
            await fixture.QueryBus.QueryAsync(query, default);
        }
    }
}
