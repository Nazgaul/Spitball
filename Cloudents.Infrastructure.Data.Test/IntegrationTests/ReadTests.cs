﻿using Cloudents.Core.Entities;
using Cloudents.Query.Chat;
using Cloudents.Query.Documents;
using Cloudents.Query.Email;
using Cloudents.Query.HomePage;
using Cloudents.Query.SearchSync;
using Cloudents.Query.Tutor;
using FluentAssertions;
using System;
using System.Linq;
using System.Threading.Tasks;
using NHibernate.Linq;
using Xunit;
using Cloudents.Core.Enum;
using Cloudents.Query.Users;
using Cloudents.Query.Courses;
using Cloudents.Query.General;
using Cloudents.Query.Session;
using Cloudents.Query.StudyRooms;
using Cloudents.Query.Sync;

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
            //var query2 = new TutorSyncAzureSearchQuery(version);

            var _ = await fixture.QueryBus.QueryAsync(query, default);
            //await fixture.QueryBus.QueryAsync(query2, default);


        }


        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        public async Task DocumentSyncAzureSearchQuery_Ok(int version, int page)
        {
            var query = new SyncAzureQuery(version, page);
            //var query2 = new TutorSyncAzureSearchQuery(version);

            var _ = await fixture.QueryBus.QueryAsync(query, default);
            //await fixture.QueryBus.QueryAsync(query2, default);


        }

       

        [Theory]
        [InlineData(50084, 638)]
        [InlineData(45209, 638)]
        [InlineData(29106, 638)]
        [InlineData(29106, 0)]
        public async Task DocumentById_Ok(long documentId, long? userId)
        {
            var query = new DocumentById(documentId, userId);

            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        //StudyRoomByIdDetailsQuery
        [Theory]
        [InlineData("0F70AF05-BAD4-4299-8341-AA38007858CF")]
        [InlineData("30869fd9-f2f3-41ef-baee-ab9500b3832a")]
        public async Task StudyRoomByIdDetailsQuery_Ok(string id)
        {
            var query = new StudyRoomByIdDetailsQuery(Guid.Parse(id), 159039);

            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData("0F70AF05-BAD4-4299-8341-AA38007858CF")]
        [InlineData("30869fd9-f2f3-41ef-baee-ab9500b3832a")]
        public async Task StudyRoomQuery_Ok(string id)
        {
            var query = new StudyRoomQuery(Guid.Parse(id), 159039);

            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task UserProfileAboutQuery_Ok()
        {
            var query = new UserProfileAboutQuery(638);

            var result = await fixture.QueryBus.QueryAsync(query, default);
            result.Reviews.Should().BeInDescendingOrder(x => x.Created);
        }

        [Theory]
        [InlineData(638)]

        public async Task UserDocumentsQueryHandler_Ok(long userId)
        {
            var query = new UserDocumentsQuery(userId, 0);
            var result = await fixture.QueryBus.QueryAsync(query, default);
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
        [InlineData("ארה\"ב", 0)]
        [InlineData("ממ\"", 0)]
        [InlineData("אלגברה ל", 0)]
        public async Task CourseSearchWithTermQuery_Ok(string term, int page)
        {
            var userId = await fixture.StatelessSession.Query<User>().Where(w => w.Country == "IL").Select(s => s.Id).FirstAsync();

            var query = new CourseSearchWithTermQuery(userId, term, page);

            var z = await fixture.QueryBus.QueryAsync(query, default);

            z.Should().HaveCountGreaterOrEqualTo(1);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(638)]
        [InlineData(160634)]
        [InlineData(161777)]
        public async Task UserProfileTutorQuery_Ok(long userId)
        {
            var id = await fixture.StatelessSession.Query<Tutor>()
               .Select(s=>s.Id).Take(1).SingleAsync();
            var query = new UserProfileQuery(id, userId);

            var result = await fixture.QueryBus.QueryAsync(query, default);

            result.Should().NotBeNull();

        }

        [Theory]
        [InlineData(0)]
        [InlineData(638)]
        [InlineData(160634)]
        public async Task UserProfileNotTutorQuery_Ok(long userId)
        {
            var id = await fixture.StatelessSession.Query<User>()
                .Fetch(f=>f.Tutor)
                .Where(w => w.Tutor.State == null)
                .Select(s=>s.Id).Take(1).SingleAsync();
            var query = new UserProfileQuery(id, userId);

            var result = await fixture.QueryBus.QueryAsync(query, default);

            result.Should().BeNull();

        }

        [Theory]
        [InlineData(638)]
        [InlineData(164835)]
        [InlineData(159039)]
        public async Task AccountUserDataById_Ok(long id)
        {
            var query = new UserAccountQuery(id);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task UserCoursesQuery_Ok()
        {
            var query = new UserCoursesQuery(638);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        //[Fact]
        //public async Task UserCoursesNamesQuery_Ok()
        //{
        //    var query = new UserCoursesNamesQuery(638);
        //    var _ = await fixture.QueryBus.QueryAsync(query, default);
        //}



        [Fact]
        public async Task UserReferralsQuery_Ok()
        {
            var query = new UserReferralsQuery(159039);
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

        //[Fact]
        //public async Task UserPurchaseDocumentByIdQuery_Ok()
        //{
        //    var query = new UserPurchaseDocumentByIdQuery(638, 0);
        //    var _ = await fixture.QueryBus.QueryAsync(query, default);

        //}

        //[Fact]
        //public async Task QuestionDataByIdQuery_Ok()
        //{
        //    var query = new QuestionDataByIdQuery(10626);
        //    var _ = await fixture.QueryBus.QueryAsync(query, default);
        //}

        [Theory]
        [InlineData(638)]
        [InlineData(1696)]
        public async Task UserBalanceQuery_Ok(long id)
        {

            var query = new UserBalanceQuery(id);
            var _ = await fixture.QueryBus.QueryAsync(query, default);


        }

        [Fact]
        public async Task GetUpdatesEmailUsersQuery_Ok()
        {
            var query = new GetUpdatesEmailUsersQuery(DateTime.UtcNow.AddDays(-1), 0);
            var _ = await fixture.QueryBus.QueryAsync(query, default);

        }

        //[Theory]
        //[InlineData("IL")]
        //[InlineData("IN")]
        //public async Task TopTutorsQuery_Ok(string countryStr)
        //{
        //    var country = Country.FromCountry(countryStr);
        //    var query = new TopTutorsQuery(country, 12);
        //    var _ = await fixture.QueryBus.QueryAsync(query, default);
        //}


        //[Theory]
        //[InlineData("IL")]
        //[InlineData("IN")]
        //public async Task TopDocumentsQuery_Ok(string countryStr)
        //{
        //    var country = Country.FromCountry(countryStr);
        //    var query = new TopDocumentsQuery(country, 5);
        //    var _ = await fixture.QueryBus.QueryAsync(query, default);
        //}

        [Fact]
        public async Task StatsQuery_Ok()
        {
            var query = new StatsQuery();
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }


        [Theory]
        [InlineData("IL")]
        [InlineData("IN")]
        public async Task ReviewsQuery_Ok(string countryStr)
        {
            var country = Country.FromCountry(countryStr);
            var query = new ReviewsQuery(country, 5);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task RequestTutorEmailQuery_Ok()
        {
            var query = new RequestTutorEmailQuery(new Guid("DB8A3DB9-94B5-41AB-9377-AB0B00D81BCD"));
            var res = await fixture.QueryBus.QueryAsync(query, default);
           // res.Should().NotBeEmpty();
        }

       


       

        [Theory]
        [InlineData(638)]
        [InlineData(159039)]
        public async Task UserSalesByIdQuery_Ok(long id)
        {
            var query = new UserSalesByIdQuery(id);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData(159039)]
        [InlineData(638)]
        public async Task UserPurchasesByIdQuery_Ok(long userId)
        {
            var query = new UserPurchasesByIdQuery(userId);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData(159039)]
        [InlineData(638)]
        public async Task UserFollowersByIdQuery_Ok(long userId)
        {
            var query = new UserFollowersByIdQuery(userId);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }


        [Theory]
        [InlineData(638)]
        [InlineData(159039)]
        public async Task UserContentByIdQuery_Ok(long id)
        {
            var query = new UserContentByIdQuery(id);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task ChatConversationsQuery_Ok()
        {
            var query = new ChatConversationsQuery(159039);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task GetBannerQuery_Ok()
        {
            var query = new GetBannerQuery(new System.Globalization.CultureInfo("en"));
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }


        [Fact]
        public async Task ChatConversationByIdQuery_Ok()
        {
            var query = new ChatConversationByIdQuery("159039_160171", 0);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        //[Fact]
        //public async Task UserVotesByCategoryQuery_Ok()
        //{
        //    var query = new UserVotesByCategoryQuery(159039);
        //    var _ = await fixture.QueryBus.QueryAsync(query, default);
        //}

        [Fact]
        public async Task AboutTutorQuery_Ok()
        {
            var query = new AboutTutorQuery();
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task GetDocumentPurchasedEmailQuery_Ok()
        {
            var query = new GetDocumentPurchasedEmailQuery(Guid.Parse("FA99DD71-F925-4B53-A7AD-A9C600F2FAEE"));
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task GetEmailByEventQuery_Ok()
        {
            var query = new GetEmailByEventQuery("DocumentPurchased");
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData(159039)]
        [InlineData(638)]
        public async Task GetUpdatesEmailByUserQuery_Ok(long id)
        {
            var query = new GetUpdatesEmailByUserQuery(id, DateTime.UtcNow.AddDays(-30));
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task RedeemEmailQuery_Ok()
        {
            var query = new RedeemEmailQuery(Guid.Parse("C3760BED-ACDB-45ED-985A-A92E00905B80"));
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task RequestTutorAdminEmailQuery_Ok()
        {
            var query = new RequestTutorAdminEmailQuery(Guid.Parse("F339FFCE-7310-4C5B-B0FA-AB2E00EBA022"));
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }


        //[Fact]
        //public async Task StudyRoomVideoEmailQuery_Ok()
        //{
        //    var query = new StudyRoomVideoEmailQuery("d7140ca0-2b83-4ce5-b35c-ab190086fe18_1575469575");
        //    var _ = await fixture.QueryBus.QueryAsync(query, default);
        //}

        [Fact]
        public async Task ShortUrlQuery_Ok()
        {
            var query = new ShortUrlQuery("pIX5VcMSI");
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SiteMapQuery_Ok(bool country)
        {

            var query = new SiteMapQuery(country);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData(159039)]
        [InlineData(638)]
        public async Task UserDataByIdQuery_Ok(long userId)
        {
            var query = new UserDataByIdQuery(userId);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }


        //[Theory]
        //[InlineData(159039)]
        //[InlineData(638)]
        //public async Task UserTransactionQuery_Ok(long userId)
        //{
        //    var query = new UserTransactionQuery(userId);
        //    var _ = await fixture.QueryBus.QueryAsync(query, default);
        //}

        [Fact]
        public async Task CalendarEventsQuery_Ok()
        {
            var userId = await fixture.StatelessSession.Query<GoogleTokens>().Select(s => s.Id).FirstAsync();

            var query = new CalendarEventsQuery(long.Parse(userId), DateTime.UtcNow, DateTime.UtcNow.AddDays(5));
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task GetPhoneNumberQuery_Ok()
        {
            var query = new GetPhoneNumberQuery(159039);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task CountryByIpQuery_Ok()
        {
            var query = new CountryByIpQuery("112.209.166.71");
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }




        [Fact]
        public async Task UserCalendarByIdQuery_Ok()
        {
            var query = new UserCalendarByIdQuery(638);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        //[Theory]
        //[InlineData("Economics")]
        //[InlineData("31010-אנגלית רמה A למדעי הרוח")]
        //public async Task CourseSubjectQuery_Ok(string courseName)
        //{
        //    var query = new CourseSubjectQuery(courseName);
        //    var _ = await fixture.QueryBus.QueryAsync(query, default);
        //}

        //[Fact]
        //public async Task AdminFictivePendingQuestionEmptyQuery_Ok()
        //{
        //    var query = new FictivePendingQuestionEmptyQuery();
        //    var _ = await fixture.QueryBus.QueryAsync(query, default);
        //}

        [Theory]
        [InlineData(159039, 7)]
        [InlineData(159039, 30)]
        public async Task TutorStatsQuery_Ok(long userId, int days)
        {
            var query = new UserStatsQuery(userId, days);
            var result = (await fixture.QueryBus.QueryAsync(query, default)).ToList();
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
        }

        //[Fact]
        //public async Task AccountQuestionsQuery_Ok()
        //{
        //    var query = new AccountQuestionsQuery(159039, "IL");
        //    var result = await fixture.QueryBus.QueryAsync(query, default);
        //    result.Should().NotBeNull();
        //}

        [Fact]
        public async Task PaymentBySessionIdQuery_Ok()
        {
            var query = new PaymentBySessionIdQuery(Guid.Parse("29FA48E7-65E0-4E4F-9916-AB1E00A8BC8B"));
            var result = await fixture.QueryBus.QueryAsync(query, default);
            result.Should().NotBeNull();
        }


        [Fact]
        public async Task SessionApprovalQuery_Ok()
        {
            var resultQuery = await fixture.StatelessSession.Query<StudyRoomSessionUser>()
                .Fetch(f => f.StudyRoomSession)
                .ThenFetch(f => f.StudyRoom)
                .Select(s => new
                {
                    SessionId = s.Id,
                    UserId = s.User.Id,
                    TutorId = s.StudyRoomSession.StudyRoom.Tutor.Id
                })
                .Take(1).SingleOrDefaultAsync();
            if (resultQuery == null)
            {
                return;
            }

            var query = new SessionApprovalQuery(resultQuery.SessionId, /*resultQuery.UserId,*/ resultQuery.TutorId);
            var result = await fixture.QueryBus.QueryAsync(query, default);
            result.Should().NotBeNull();
        }

        [Theory]
        [InlineData(638,159039)]
        [InlineData(160171,159039)]
        public async Task TutorUpcomingBroadcastStudyRoomQuery_Ok(long tutorId, long userId)
        {
            var query = new TutorUpcomingBroadcastStudyRoomQuery(tutorId, userId);
            var result = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData("9f54280c-103e-46a6-8184-aabf00801beb")]
        [InlineData("0362bc01-9991-43a5-a7e3-aba600e2c4cc")]
        public async Task SeoStudyRoomQuery_Ok(string id)
        {
            var guidId = Guid.Parse(id);
            var query = new SeoStudyRoomQuery(guidId);
            var result = await fixture.QueryBus.QueryAsync(query, default);
        }

        


        [Fact]
        public async Task ChatConversationDetailQuery_Ok()
        {
            var identifier = await fixture.StatelessSession.Query<ChatUser>().Where(w => w.User.Id == 638).Select(s => s.ChatRoom.Identifier).FirstAsync();
            var query = new ChatConversationDetailQuery(identifier,638);
            var result = await fixture.QueryBus.QueryAsync(query, default);

        }

        [Fact]
        public async Task LiveClassesReminderQuery_Ok()
        {
            var query = new LiveClassesReminderQuery();
            var result = await fixture.QueryBus.QueryAsync(query);
        }


    }
}