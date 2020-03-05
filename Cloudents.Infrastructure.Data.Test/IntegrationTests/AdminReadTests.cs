using System.Linq;
using Cloudents.Core.Enum;
using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Cloudents.Query.Admin;

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
            var query = new UserAnswersQuery(638, 0, "IL");
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData("638_159039", null)]
        [InlineData("638_159039", "IL")]
        public async Task AdminConversationDetailsQuery_Ok(string id, string country)
        {
            var query = new ConversationDetailsQuery(id, country);
            await _fixture.QueryBus.QueryAsync(query, default);
            //res.Should().HaveCount(2);
        }

        [Theory]
        [InlineData("638_159039", "IN")]
        public async Task AdminConversationDetailsQuery_WrongCountry_Ok(string id, string country)
        {
            var query = new ConversationDetailsQuery(id, country);
            var res = await _fixture.QueryBus.QueryAsync(query, default);
            res.Should().HaveCount(0);
        }
        [Fact]
        public async Task AdminConversationsQuery_Ok()
        {
            var query1 = new ConversationsQuery(159039, 0, null, null, null, null);
            var query2 = new ConversationsQuery(159039, 0, null, ChatRoomStatus.SessionScheduled, "", WaitingFor.All);

            var task1 = _fixture.QueryBus.QueryAsync(query1, default);
            var task2 = _fixture.QueryBus.QueryAsync(query2, default);
            await Task.WhenAll(task1, task2);
        }

        //[Theory]
        //[InlineData("he", ItemState.Pending, "IL", null)]
        //[InlineData("en", ItemState.Ok, "IL", null)]
        //[InlineData(null, null, null, null)]
        //public async Task AdminCoursesQuery_Ok(string language, ItemState? state, string country, string filter)
        //{
        //    var query = new AdminCoursesQuery(language, state.GetValueOrDefault(ItemState.Pending), country, filter);
        //    await _fixture.QueryBus.QueryAsync(query, default);
        //}

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


        //[Fact]
        //public async Task AdminPageQuery_QuestionWithoutCorrectAnswer_Ok()
        //{
        //    var query = new AdminQuestionWithoutCorrectAnswerPageQuery(0, "IL");
        //    await _fixture.QueryBus.QueryAsync(query, default);
        //}

        [Theory]
        [InlineData(null)]
        [InlineData("IL")]
        public async Task AdminPendingTutorsQuery_Ok(string country)
        {
            var query = new PendingTutorsQuery(country);
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData(159039, "IL")]
        [InlineData(159039, null)]
        public async Task AdminSessionsQuery_Ok(long userId, string country)
        {
            var query = new SessionsQuery(userId, country);
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData("IL")]
        [InlineData(null)]
        public async Task AdminStudyRoomQuery_Ok(string country)
        {
            var query = new StudyRoomQuery(country);
            await _fixture.QueryBus.QueryAsync(query, default);
        }


        [Fact]
        public async Task AdminSubjectsQuery_Ok()
        {
            var query = new SubjectsQuery(Guid.Parse("6942483C-4CEF-4256-B9A1-0CAE272C54B4"));
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminUniversitiesQuery_Ok()
        {
            var query = new UniversitiesQuery("IL", ItemState.Ok);
            var query2 = new UniversitiesQuery(null, ItemState.Ok);
            var t1 = _fixture.QueryBus.QueryAsync(query, default);
            var t2 = _fixture.QueryBus.QueryAsync(query2, default);
            await Task.WhenAll(t1, t2);
        }

        [Fact]
        public async Task AdminUserAnswersQuery_Ok()
        {
            var query = new UserAnswersQuery(159039, 0, "IL");
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData("")]
        [InlineData("IL")]
        public async Task AdminPaymentsQuery_Ok(string country)
        {
            var q1 = new PaymentsQuery(country);
            var _ = await _fixture.QueryBus.QueryAsync(q1, default);
        }

        [Theory]
        [InlineData("Hadar@cloudents.com","IL")]
        [InlineData("0523556456", "IL")]
        [InlineData("0523556456", null)]
        [InlineData("638", "IN")]
        [InlineData("638", "IL")]
        [InlineData("638", null)]
        public async Task AdminUserDetailsQuery_Ok(string email, string country)
        {
            var q2 = new UserDetailsQuery(email, country);
            await _fixture.QueryBus.QueryAsync(q2, default);
        }

        [Theory]
        [InlineData(159039, 0, "IL")]
        [InlineData(159039, 0, null)]
        public async Task AdminUserDocumentsQuery_Ok(long userId, int page, string country)
        {
            var query = new UserDocumentsQuery(userId, page, country);
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminUserFlagsOthersQuery_Ok()
        {
            var q1 = new UserFlagsOthersQuery(3, 0, "IL");
            var q2 = new UserFlagsOthersQuery(2, 2, "IL");
            var t1 = _fixture.QueryBus.QueryAsync(q1, default);
            var t2 = _fixture.QueryBus.QueryAsync(q2, default);
            await Task.WhenAll(t1, t2);
        }

        [Fact]
        public async Task AdminUserPurchasedDocsQuery_Ok()
        {
            var query = new UserPurchasedDocsQuery(159039, 0, "IL");
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminUserQuestionsQuery_Ok()
        {
            var query = new UserQuestionsQuery(159039, 0, "IL");
            await _fixture.QueryBus.QueryAsync(query, default);
        }
        //[Fact]
        //public async Task AdminAllCoursesEmptyQuery_Ok()
        //{
        //    var query = new AllCoursesEmptyQuery();
        //    await _fixture.QueryBus.QueryAsync(query, default);
        //}
        [Theory]
        [InlineData("IL")]
        [InlineData(null)]
        public async Task AdminAllUniversitiesEmptyQuery_Ok(string country)
        {
            var query = new AllUniversitiesQuery(country);
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task FlaggedQuestionEmptyQuery_Ok()
        {
            var query = new FlaggedQuestionQuery("IL");
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData("IL")]
        [InlineData(null)]
        public async Task FlaggedDocumentEmptyQuery_Ok(string country)
        {
            var query = new FlaggedDocumentQuery(country);
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData("IL")]
        [InlineData(null)]
        public async Task FlaggedAnswerEmptyQuery_Ok(string country)
        {
            var query = new FlaggedAnswerQuery(country);
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData(203300, "IL")]
        [InlineData(null, "IN")]
        public async Task PendingDocumentEmptyQuery_Ok(long? documentId, string country)
        {
            var query = new PendingDocumentQuery(documentId, country);
            await _fixture.QueryBus.QueryAsync(query, default);
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

        [Theory]
        [InlineData("IL")]
        [InlineData(null)]
        public async Task PendingQuestionsEmptyQuery_Ok(string country)
        {
            var query = new PendingQuestionsQuery(country);
            await _fixture.QueryBus.QueryAsync(query, default);
        }
        [Theory]
        [InlineData("IL")]
        [InlineData(null)]
        public async Task AdminCashOutEmptyQuery_Ok(string country)
        {
            var query = new CashOutQuery(country);
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminChatConversationByIdQuery_Ok()
        {
            var query = new ChatConversationByIdQuery("159489_160171", 0, null);
            await _fixture.QueryBus.QueryAsync(query, default);
        }


        [Fact]
        public async Task AdminAssignToQuery_Ok()
        {
            var query = new AssignToQuery();
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        //[Theory]
        
        //[InlineData("IN")]
        //public async Task AdminLeadsQuery_Ok(string country)
        //{
        //    var query = new AdminLeadsQuery(country);
        //    _ = await _fixture.QueryBus.QueryAsync(query, default);
        //}

        [Theory]
        [InlineData(null, ItemState.Ok, null, null)]
        [InlineData(null, ItemState.Pending, null, null)]
        [InlineData("he", ItemState.Ok, null, null)]
        [InlineData("en", ItemState.Pending, null, null)]
        [InlineData(null, ItemState.Ok, "IL", null)]
        [InlineData(null, ItemState.Pending, "IL", null)]
        [InlineData("he", ItemState.Ok, "IL", null)]
        [InlineData("en", ItemState.Pending, "IL", null)]
        [InlineData(null, ItemState.Ok, null, "V")]
        [InlineData(null, ItemState.Pending, null, "V")]
        [InlineData("he", ItemState.Ok, null, "V")]
        [InlineData("en", ItemState.Pending, null, "V")]
        [InlineData(null, ItemState.Ok, "IL", "V")]
        [InlineData(null, ItemState.Pending, "IL", "V")]
        [InlineData("he", ItemState.Ok, "IL", "V")]
        [InlineData("en", ItemState.Pending, "IL", "V")]

        public async Task AdminCoursesQuery_Ok(string language, ItemState state, string country, string filter)
        {
            var query = new CoursesQuery(language, state, country, filter);
            var result = await _fixture.QueryBus.QueryAsync(query, default);


            //check distinct
            var v = from c in result
                group c by c.Name
                into grp
                where grp.Count() > 1
                select grp.Key;
            v.Should().BeEmpty();


        }

        [Theory]
        [InlineData(159039, "", 0, "IL")]
        [InlineData(0, null, 0, "IL")]
        [InlineData(0, "Econ", 0, "IL")]
        public async Task AdminCourseSearchQuery_Ok(long userId, string term, int page, string country)
        {
            var query = new CourseSearchQuery(userId, term, page, country);
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData("IL")]
        [InlineData(null)]
        public async Task AdminLeadsQuery_Ok(string country)
        {
            var query = new LeadsQuery(country);
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData("BC6F84B9-7EFC-4B84-AA51-AAA700CCB406")]
        [InlineData("C59F60BC-29BD-4101-84D2-AAAA00B7C637")]
        public async Task AdminPaymentBySessionIdQuery_Ok(string sessionId)
        {
            var session = Guid.Parse(sessionId);
            var query = new PaymentBySessionIdQuery(session);
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData(159039, "IL")]
        [InlineData(159039, null)]
        public async Task AdminTutorSessionsQuery_Ok(long tutorId, string country)
        {
            var query = new TutorSessionsQuery(tutorId, country);
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData("IL")]
        [InlineData(null)]
        public async Task AdminTutorsWithStudyRoomsQuery_Ok(string country)
        {
            var query = new TutorsWithStudyRoomsQuery(country);
            await _fixture.QueryBus.QueryAsync(query, default);
        }


        [Theory]
        [InlineData(159039, 0, "IL")]
        [InlineData(159039, 0, null)]
        public async Task AdminUserSoldDocsQuery_Ok(long userId, int page, string country)
        {
            var query = new UserSoldDocsQuery(userId, page, country);
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task ValidateUserQuery_Ok()
        {
            var query = new ValidateUserQuery("elad@cloudents.com");
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminUserNotesQuery_Ok()
        {
            var query = new UserNotesQuery(159039);
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminCouponQuery_Ok()
        {
            var query = new CouponQuery();
            var _ = await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task SubjectsTranslationQuery_Ok()
        {
            var query = new SubjectsTranslationQuery();
            var _ = await _fixture.QueryBus.QueryAsync(query, default);
        }

    }
}
