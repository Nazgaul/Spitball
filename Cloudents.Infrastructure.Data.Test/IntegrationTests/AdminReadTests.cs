﻿using System.Linq;
using Cloudents.Core.Enum;
using System;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using FluentAssertions;
using Xunit;
using Cloudents.Query.Admin;
using NHibernate.Linq;

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

        [Theory]
        [InlineData("x", ItemState.Ok, "us")]
        [InlineData(null, null, null)]
        public async Task TutorSearchQuery_Ok(string term, ItemState? state, string countryStr)
        {
            var country = FromCountry(countryStr);
            var query = new TutorSearchQuery(term, state, country);
            var _ = await _fixture.QueryBus.QueryAsync(query, default);
        }


        [Theory]
        [InlineData(null)]
        [InlineData("IL")]
        public async Task AdminPendingTutorsQuery_Ok(string countryStr)
        {
            var country = FromCountry(countryStr);
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
        public async Task AdminStudyRoomQuery_Ok(string countryStr)
        {
            var country = FromCountry(countryStr);
            var query = new StudyRoomQuery(country);
            await _fixture.QueryBus.QueryAsync(query, default);
        }


        //[Fact]
        //public async Task AdminSubjectsQuery_Ok()
        //{
        //    var query = new SubjectsQuery(Guid.Parse("6942483C-4CEF-4256-B9A1-0CAE272C54B4"));
        //    await _fixture.QueryBus.QueryAsync(query, default);
        //}

        //[Fact]
        //public async Task AdminUniversitiesQuery_Ok()
        //{
        //    var query = new UniversitiesQuery("IL", ItemState.Ok);
        //    var query2 = new UniversitiesQuery(null, ItemState.Ok);
        //    var t1 = _fixture.QueryBus.QueryAsync(query, default);
        //    var t2 = _fixture.QueryBus.QueryAsync(query2, default);
        //    await Task.WhenAll(t1, t2);
        //}

        [Fact]
        public async Task AdminUserAnswersQuery_Ok()
        {
            var query = new UserAnswersQuery(159039, 0, FromCountry("IL"));
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData("")]
        [InlineData("US")]
        [InlineData("IL")]
        public async Task AdminPaymentsQuery_Ok(string country)
        {
            var q1 = new SessionPaymentsQuery(country);
            var _ = await _fixture.QueryBus.QueryAsync(q1, default);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("IL")]
        [InlineData("US")]
        public async Task SessionPaymentsQueryV2_Ok(string countryStr)
        {
            var country = FromCountry(countryStr);
            var query = new SessionPaymentsQueryV2(country);
            var _ = await _fixture.QueryBus.QueryAsync(query, default);
        }

        private Country? FromCountry(string countryStr)
        {
            if (countryStr == null)
            {
                return null;

            }
            return Country.FromCountry(countryStr);
        }

        [Theory]
        [InlineData("Hadar@cloudents.com", "IL")]
        [InlineData("0523556456", "IL")]
        [InlineData("0523556456", null)]
        [InlineData("638", "IN")]
        [InlineData("638", "IL")]
        [InlineData("638", null)]
        public async Task AdminUserDetailsQuery_Ok(string email, string country)
        {


            var q2 = new UserDetailsQuery(email, FromCountry(country));
            await _fixture.QueryBus.QueryAsync(q2, default);
        }

        [Theory]
        [InlineData(159039, 0, "IL")]
        [InlineData(159039, 0, null)]
        public async Task AdminUserDocumentsQuery_Ok(long userId, int page, string country)
        {
            var query = new UserDocumentsQuery(userId, page, FromCountry(country));
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData(3, 0, "IL")]
        [InlineData(2, 2, "IL")]
        public async Task AdminUserFlagsOthersQuery_Ok(int minFlags, int page, string country)
        {
            var q1 = new UserFlagsOthersQuery(minFlags, page, country);
            var x = await _fixture.QueryBus.QueryAsync(q1, default);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("IL")]
        public async Task AdminUserPurchasedDocsQuery_Ok(string countryStr)
        {
            var country = FromCountry(countryStr);
            var query = new UserPurchasedDocsQuery(159039, 0, country);
            await _fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task AdminUserQuestionsQuery_Ok()
        {
            var query = new UserQuestionsQuery(159039, 0, FromCountry("IL"));
            await _fixture.QueryBus.QueryAsync(query, default);
        }
        //[Fact]
        //public async Task AdminAllCoursesEmptyQuery_Ok()
        //{
        //    var query = new AllCoursesEmptyQuery();
        //    await _fixture.QueryBus.QueryAsync(query, default);
        //}
        //[Theory]
        //[InlineData("IL")]
        //[InlineData(null)]
        //public async Task AdminAllUniversitiesEmptyQuery_Ok(string country)
        //{
        //    var query = new AllUniversitiesQuery(country);
        //    await _fixture.QueryBus.QueryAsync(query, default);
        //}

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
        public async Task FlaggedAnswerEmptyQuery_Ok(string countryStr)
        {
            var country = FromCountry(countryStr);
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
            var query = new SuspendedUsersQuery(FromCountry("IL"));
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
        [InlineData(ItemState.Ok, null, null)]
        [InlineData(ItemState.Pending, null, null)]
        [InlineData(ItemState.Ok, "IL", null)]
        [InlineData(ItemState.Pending, "IL", null)]
        [InlineData(ItemState.Ok, null, "V")]
        [InlineData(ItemState.Pending, null, "V")]
        [InlineData(ItemState.Ok, "IL", "V")]
        [InlineData(ItemState.Pending, "IL", "V")]

        public async Task AdminCoursesQuery_Ok(ItemState state, string countryStr, string search)
        {
            var country = FromCountry(countryStr);
            var query = new CoursesQuery(state, country, search);
            var result = await _fixture.QueryBus.QueryAsync(query, default);


            //check distinct
            var v = from c in result
                    group c by c.Name
                into grp
                    where grp.Count() > 1
                    select grp.Key;
            v.Should().BeEmpty();


        }

        //[Theory]
        //[InlineData(159039, "", 0, "IL")]
        //[InlineData(0, null, 0, "IL")]
        //[InlineData(0, "Econ", 0, "IL")]
        //public async Task AdminCourseSearchQuery_Ok(long userId, string term, int page, string country)
        //{
        //    var query = new CourseSearchQuery(userId, term, page, country);
        //    await _fixture.QueryBus.QueryAsync(query, default);
        //}

        [Theory]
        [InlineData("IL")]
        [InlineData(null)]
        public async Task AdminLeadsQuery_Ok(string countryStr)
        {
            var country = FromCountry(countryStr);
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


        [Fact]
        public async Task PaymentBySessionIdV2Query_Ok()
        {

            var resultQuery = await _fixture.StatelessSession.Query<StudyRoomSessionUser>()
                .Fetch(f => f.StudyRoomPayment)
                .OrderByDescending(o => o.StudyRoomSession.Created)
                .Select(s => new
                {
                    SessionId = s.StudyRoomPayment.Id,
                    UserId = s.User.Id,
                    TutorId = s.StudyRoomPayment.Tutor.Id
                })

                .Take(1).SingleOrDefaultAsync();
            if (resultQuery == null)
            {
                return;
            }
            var query = new PaymentBySessionIdV2Query(resultQuery.SessionId, resultQuery.UserId, resultQuery.TutorId);
            var result = await _fixture.QueryBus.QueryAsync(query, default);
            result.Should().NotBeNull();
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

        [Theory]
        [InlineData("US")]
        [InlineData("In")]
        [InlineData("IL")]
        [InlineData(null)]
        public async Task SubjectsTranslationQuery_Ok(string countryStr)
        {
            var country = FromCountry(countryStr);
            var query = new SubjectsQuery(country);
            var _ = await _fixture.QueryBus.QueryAsync(query, default);
        }

        
    }
}
