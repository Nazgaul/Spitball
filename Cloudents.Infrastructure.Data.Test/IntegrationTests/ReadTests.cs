using Cloudents.Core.Entities;
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

namespace Cloudents.Infrastructure.Data.Test.IntegrationTests
{
    [Collection("Database collection")]
    public class ReadTests
    {
        DatabaseFixture fixture;

        public ReadTests(DatabaseFixture fixture)
        {
            this.fixture = fixture;

        }

     

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task TutorSyncAzureSearchQuery_Ok(int version)
        {
            var query = new TutorSyncAzureSearchQuery(version);

            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }



        [Theory]
        [InlineData(50084, 638)]
        [InlineData(45209, 638)]
        [InlineData(29106, 638)]
        [InlineData(29106, 0)]
        [InlineData(53262, 638)]
        public async Task DocumentById_Ok(long documentId, long? userId)
        {
            var query = new DocumentById(documentId, userId);

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
            var query = new UserProfileReviewsQuery(638);

            var result = await fixture.QueryBus.QueryAsync(query, default);
            result.Reviews.Should().BeInDescendingOrder(x => x.Created);
        }


        [Theory]
        [InlineData(StudyRoomType.Broadcast)]
        [InlineData(StudyRoomType.Private)]
        public async Task UserStudyRoomQuery_Ok(StudyRoomType type)
        {
            var query = new UserStudyRoomQuery(638, type);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }


        [Theory]
        [InlineData(null)]

        [InlineData("t")]
        [InlineData("ממ\"")]
        [InlineData("אלגברה ל")]
        public async Task CourseSearchQuery_Ok(string term)
        {
            var query = new CourseSearchQuery(term);
            var x = await fixture.QueryBus.QueryAsync(query, default);
        }



        [Theory]
        [InlineData(0)]
        [InlineData(638)]
        [InlineData(160634)]
        [InlineData(161777)]
        public async Task UserProfileTutorQuery_Ok(long userId)
        {
            var id = await fixture.StatelessSession.Query<Tutor>()
               .Select(s => s.Id).Take(1).SingleAsync();
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
                .Fetch(f => f.Tutor)
                .Where(w => w.Tutor.State == null)
                .Select(s => s.Id).Take(1).SingleAsync();
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

        [Fact]
        public async Task CourseByIdQuery_Ok()
        {
            var query = new CourseByIdQuery(1, 0);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task CourseByIdEditQuery_Ok()
        {
           var data = await  fixture.StatelessSession.Query<Course>()
                .Select(s => new
                {
                    TutorId= s.Tutor.Id,
                    s.Id
                }).FirstAsync();
            var query = new CourseByIdEditQuery(data.Id, data.TutorId);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }




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

        

        [Fact]
        public async Task GetUpdatesEmailUsersQuery_Ok()
        {
            var query = new GetUpdatesEmailUsersQuery(DateTime.UtcNow.AddDays(-1), 0);
            var _ = await fixture.QueryBus.QueryAsync(query, default);

        }

     

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
            var _ = await fixture.QueryBus.QueryAsync(query, default);
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
            var query = new UserCoursesByIdQuery(id);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task ChatConversationsQuery_Ok()
        {
            var query = new ChatConversationsQuery(159039);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        
        [Fact]
        public async Task ChatConversationByIdQuery_Ok()
        {
            var query = new ChatConversationByIdQuery("159039_160171", 0);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

       
        [Fact]
        public async Task AboutTutorQuery_Ok()
        {
            var query = new AboutTutorQuery();
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
            var resultQuery = await fixture.StatelessSession.Query<StudyRoomPayment>()
                .Select(s => new
                {
                    SessionId = s.Id,
                    UserId = s.User.Id,
                    TutorId = s.Tutor.Id
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
            var query = new ChatConversationDetailQuery(identifier, 638);
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