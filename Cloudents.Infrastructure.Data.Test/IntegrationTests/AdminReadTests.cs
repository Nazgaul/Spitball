using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Query.Admin;
using Cloudents.Query.Query.Admin;
using Xunit;

namespace Cloudents.Infrastructure.Data.Test.IntegrationTests
{
    [Collection("Database collection")]
    public class AdminReadTests
    {
        private readonly DatabaseFixture _fixture;

        public AdminReadTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            // _autoMock = AutoMock.GetLoose();

        }


        [Fact]
        public async Task DocumentAggregateQuery_Ok()
        {
            var query = new AdminUserAnswersQuery(638, 0, "IL");
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminConversationDetailsQuery_Ok()
        {
            var query = new AdminConversationDetailsQuery("638_159039", null);
            await _fixture.QueryBus.QueryAsync(query, default);
        }
        [Fact]
        public async Task AdminConversationsQuery_Ok()
        {
            var query1 = new AdminConversationsQuery(159039, 0, null, null, null,null);
            var query2 = new AdminConversationsQuery(159039, 0, null, ChatRoomStatus.SessionScheduled, "" ,WaitingFor.All);

            var task1 = _fixture.QueryBus.QueryAsync(query1, default);
            var task2 = _fixture.QueryBus.QueryAsync(query2, default);
            await Task.WhenAll(task1, task2);
        }

        [Fact]
        public async Task AdminCoursesQuery_Ok()
        {
            var query = new AdminConversationDetailsQuery("638_159039", null);
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        //TODO: check all AdminEmptyQuery handlers
     

        //[Fact]
        //public async Task AdminLanguageQuery_Courses_Ok()
        //{
        //    var queryHe = new AdminLanguageQuery("he");
        //    var queryNull = new AdminLanguageQuery(null);
        //    var t1 = fixture.QueryBus.QueryAsync<IList<PendingCoursesDto>>(queryHe, default);
        //    var t2 = fixture.QueryBus.QueryAsync<IList<PendingCoursesDto>>(queryNull, default);
        //    await Task.WhenAll(t1, t2);
        //}



        //[Fact]
        //public async Task AdminLanguageQuery_University_Ok()
        //{
        //    var queryHe = new AdminLanguageQuery("il");
        //    var queryNull = new AdminLanguageQuery(null);
        //    var t1 = fixture.QueryBus.QueryAsync<IList<PendingUniversitiesDto>>(queryHe, default);
        //    var t2 = fixture.QueryBus.QueryAsync<IList<PendingUniversitiesDto>>(queryNull, default);
        //    await Task.WhenAll(t1, t2);
        //}


        [Fact]
        public async Task AdminPageQuery_QuestionWithoutCorrectAnswer_Ok()
        {
            var query = new AdminQuestionWithoutCorrectAnswerPageQuery(0, "IL");
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminPendingTutorsQuery_Ok()
        {
            var query = new AdminPendingTutorsQuery("IL");
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminSessionsQuery_Ok()
        {
            var query = new AdminSessionsQuery(159039, "IL");
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminStudyRoomQuery_Ok()
        {
            var query = new AdminStudyRoomQuery("IL");
            await _fixture.QueryBus.QueryAsync(query, default);
        }


        [Fact]
        public async Task AdminSubjectsQuery_Ok()
        {
            var query = new AdminSubjectsQuery();
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminUniversitiesQuery_Ok()
        {
            var query = new AdminUniversitiesQuery("IL", ItemState.Ok);
            var query2 = new AdminUniversitiesQuery(null, ItemState.Ok);
            var t1 = _fixture.QueryBus.QueryAsync(query, default);
            var t2 = _fixture.QueryBus.QueryAsync(query2, default);
            await Task.WhenAll(t1, t2);
        }

        [Fact]
        public async Task AdminUserAnswersQuery_Ok()
        {
            var query = new AdminUserAnswersQuery(159039, 0, "IL");
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminPaymentsQuery_Ok()
        {
            var q1 = new AdminPaymentsQuery(string.Empty);
            var q2 = new AdminUserDetailsQuery("Hadar@cloudents.com", "IL");
            var q3 = new AdminUserDetailsQuery("0523556456", "IL");
            var t1 =  _fixture.QueryBus.QueryAsync(q1, default);
            var t2 =  _fixture.QueryBus.QueryAsync(q2, default);
            var t3 =  _fixture.QueryBus.QueryAsync(q3, default);
            await Task.WhenAll(t1, t2, t3);
        }

        [Fact]
        public async Task AdminUserDocumentsQuery_Ok()
        {
            var query = new AdminUserDocumentsQuery(159039, 0, "IL");
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminUserFlagsOthersQuery_Ok()
        {
            var q1 = new AdminUserFlagsOthersQuery(3, 0, "IL");
            var q2 = new AdminUserFlagsOthersQuery(2, 2, "IL");
            var t1 = _fixture.QueryBus.QueryAsync(q1, default);
            var t2 = _fixture.QueryBus.QueryAsync(q2, default);
            await Task.WhenAll(t1, t2);
        }

        [Fact]
        public async Task AdminUserPurchasedDocsQuery_Ok()
        {
            var query = new AdminUserPurchasedDocsQuery(159039, 0, "IL");
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminUserQuestionsQuery_Ok()
        {
            var query = new AdminUserQuestionsQuery(159039, 0, "IL");
            await _fixture.QueryBus.QueryAsync(query, default);
        }
        [Fact]
        public async Task AdminAllCoursesEmptyQuery_Ok()
        {
            var query = new AdminAllCoursesEmptyQuery();
            await _fixture.QueryBus.QueryAsync(query, default);
        }
        [Fact]
        public async Task AdminAllUniversitiesEmptyQuery_Ok()
        {
            var query = new AdminAllUniversitiesQuery("IL");
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task FlaggedQuestionEmptyQuery_Ok()
        {
            var query = new FlaggedQuestionQuery("IL");
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task FlaggedDocumentEmptyQuery_Ok()
        {
            var query = new FlaggedDocumentQuery("IL");
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task FlaggedAnswerEmptyQuery_Ok()
        {
            var query = new FlaggedAnswerQuery("IL");
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task PendingDocumentEmptyQuery_Ok()
        {
            var query = new PendingDocumentQuery(203300, "IL");
            await _fixture.QueryBus.QueryAsync(query, default);

            var query2 = new PendingDocumentQuery(null, "IN");
            await _fixture.QueryBus.QueryAsync(query2, default);
        }

        [Fact]
        public async Task SuspendedUsersEmptyQuery_Ok()
        {
            var query = new SuspendedUsersQuery("IL");
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        //[Fact]
        //public async Task FictivePendingQuestionEmptyQuery_Ok()
        //{
        //    var query = new AdminFictivePendingQuestionEmptyQuery();
        //    await fixture.QueryBus.QueryAsync(query, default);
        //}

        [Fact]
        public async Task PendingQuestionsEmptyQuery_Ok()
        {
            var query = new AdminPendingQuestionsQuery("IL");
            await _fixture.QueryBus.QueryAsync(query, default);
        }
        [Fact]
        public async Task AdminCashOutEmptyQuery_Ok()
        {
            var query = new AdminCashOutQuery("IL");
            await _fixture.QueryBus.QueryAsync(query, default);
        }
        
         [Fact]
        public async Task AdminChatConversationByIdQuery_Ok()
        {
            var query = new AdminChatConversationByIdQuery("159489_160171", 0, null);
            await _fixture.QueryBus.QueryAsync(query, default);
        }


        [Fact]
        public async Task AdminAssignToQuery_Ok()
        {
            var query = new AdminAssignToQuery();
            await _fixture.QueryBus.QueryAsync(query, default);
        }
    }
}
