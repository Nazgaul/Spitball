﻿using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Query;
using Cloudents.Query.Chat;
using Cloudents.Query.Documents;
using Cloudents.Query.Email;
using Cloudents.Query.HomePage;
using Cloudents.Query.Query;
using Cloudents.Query.SearchSync;
using Cloudents.Query.Tutor;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Query.Query.Admin;
using Xunit;

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
            var query2 = new TutorSyncAzureSearchQuery(version);

            var _ = await fixture.QueryBus.QueryAsync(query, default);
            await fixture.QueryBus.QueryAsync(query2, default);


        }

        [Theory]
        [InlineData(638, 0, null, "IL", null)]
        [InlineData(638, 0, new[] { "x", "y" }, "IL", null)]
        [InlineData(638, 0, new[] { "בסיסי נתונים" }, "IL", null)]
        [InlineData(0, 0, new[] { "x", "y" }, "IL", null)]
        [InlineData(0, 0, new[] { "x", "y" }, "IL", "economics")]
        [InlineData(638, 0, new[] { "x", "y" }, "IL", "economics")]
        [InlineData(638, 1, new[] { "x", "y" }, "IL", null)]

        public async Task DocumentAggregateQuery_Ok(long userId, int page, string[] filter, string country, string course)
        {
            var query = new FeedAggregateQuery(userId, page, filter, country, course, 18);

            var result = (await fixture.QueryBus.QueryAsync(query, default)).ToList();
            result.Should().NotBeNullOrEmpty();

            var p = result.OfType<QuestionFeedDto>().ToList();
            if (p.Count > 0)
            {
                p.Should().Contain(c => c.User.Id > 0);
            }
        }

        [Theory]
        [InlineData(50084, 638)]
        [InlineData(45209,638)]
        public async Task DocumentById_Ok(long documentId, long? userId)
        {
            var query = new DocumentById(documentId, userId);

            var result = await fixture.QueryBus.QueryAsync(query, default);
        }




        [Fact]
        public async Task StudyRoomQuery_Ok()
        {
            var query = new StudyRoomQuery(Guid.Parse("0F70AF05-BAD4-4299-8341-AA38007858CF"), 159039);

            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task UserProfileAboutQuery_Ok()
        {
            var query = new UserProfileAboutQuery(638);

            var result = await fixture.QueryBus.QueryAsync(query, default);
            result.Reviews.Should().BeInDescendingOrder(x => x.Created);
        }

        [Fact]
        public async Task UserDocumentsQueryHandler_Ok()
        {
            var query = new UserDocumentsQuery(638, 0, 20);
            var _ = await fixture.QueryBus.QueryAsync<ListWithCountDto<DocumentFeedDto>>(query, default);
        }


        [Fact]
        public async Task UserQuestionFeedDtoQueryHandler_Ok()
        {
            var query = new UserDataPagingByIdQuery(638, 0);
            var _ = await fixture.QueryBus.QueryAsync<IEnumerable<QuestionFeedDto>>(query, default);
        }

        [Fact]
        public async Task UserAnswerFeedDtoQueryHandler_Ok()
        {
            var query = new UserAnswersByIdQuery(638, 0);
            var _ = await fixture.QueryBus.QueryAsync<IEnumerable<QuestionFeedDto>>(query, default);
        }


        [Theory]
        [InlineData(1)]
        [InlineData(50084)]
        [InlineData(50050)]
        public async Task DocumentsQueryHandler_Ok(long documentId)
        {
            var query = new IdsDocumentsQuery(new[] { documentId });
            var _ = await fixture.QueryBus.QueryAsync(query, default);

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
        [InlineData(638, "ארה\"ב", 0)]
        [InlineData(638, "ממ\"", 0)]
        [InlineData(638, "אלגברה ל", 0)]
        public async Task CourseSearchWithTermQuery_Ok(long userId, string term, int page)
        {
            var query = new CourseSearchWithTermQuery(userId, term, page);

            var _ = await fixture.QueryBus.QueryAsync(query, default);

            _.Should().HaveCountGreaterOrEqualTo(1);
        }

        [Theory]
        [InlineData(638,0)]
        [InlineData(11,0)]
        [InlineData(605,638)]
        [InlineData(36, 638)]
        [InlineData(36, 0)]
        [InlineData(160105, 638)]
        [InlineData(150713, 638)]
        [InlineData(160446, 638)]
        [InlineData(161238, 638)]
        [InlineData(159039, 160634)]
        [InlineData(159039, 160468)]
        [InlineData(160336, 160468)]
        [InlineData(1697,0)]

        public async Task UserProfileQuery_Ok(long id, long userId)
        {
            var query = new UserProfileQuery(id, userId);

            var result = await fixture.QueryBus.QueryAsync(query, default);

            result.Should().NotBeNull();

        }

        [Fact]
        public async Task AccountUserDataById_Ok()
        {
            var query = new UserAccountQuery(638);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task UserCoursesQuery_Ok()
        {
            var query = new UserCoursesQuery(638);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task UniversityQuery_Ok()
        {
            var query = new UniversityQuery(Guid.Parse("80B226AE-94A1-4240-8796-A98200E81A54"));
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
        public async Task UserPurchaseDocumentByIdQuery_Ok()
        {
            var query = new UserPurchaseDocumentByIdQuery(638, 0);
            var _ = await fixture.QueryBus.QueryAsync(query, default);

        }

        [Fact]
        public async Task QuestionDataByIdQuery_Ok()
        {
            var query = new QuestionDataByIdQuery(10626);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

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

        [Theory]
        [InlineData("IL")]
        [InlineData("IN")]
        public async Task TopTutorsQuery_Ok(string country)
        {
            var query = new TopTutorsQuery(country, 12);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }


        [Theory]
        [InlineData("IL")]
        [InlineData("IN")]
        public async Task TopDocumentsQuery_Ok(string country)
        {
            var query = new TopDocumentsQuery(country, 5);
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
        public async Task ReviewsQuery_Ok(string country)
        {
            var query = new ReviewsQuery(country, 5);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task RequestTutorEmailQuery_Ok()
        {
            var query = new RequestTutorEmailQuery(new Guid("DB8A3DB9-94B5-41AB-9377-AB0B00D81BCD"));
            var res = await fixture.QueryBus.QueryAsync(query, default);
            res.Should().NotBeEmpty();
        }

        //[Theory]
        //[InlineData(160468L)]
        //public async Task SessionRecordingQuery_Ok(long userId)
        //{
        //    var query = new SessionRecordingQuery(userId);
        //    var _ = await fixture.QueryBus.QueryAsync(query, default);
        //}


        [Theory]
        [InlineData(1L)]
        public async Task SimilarDocumentsQuery_Ok(long documentId)
        {
            var query = new SimilarDocumentsQuery(documentId);
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
        public async Task ChatConversationQuery_Ok()
        {
            var query = new ChatConversationQuery("159039_160171", 159039);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task ChatConversationByIdQuery_Ok()
        {
            var query = new ChatConversationByIdQuery("159039_160171", 0);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task UserVotesByCategoryQuery_Ok()
        {
            var query = new UserVotesByCategoryQuery(159039);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

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


        [Fact]
        public async Task StudyRoomVideoEmailQuery_Ok()
        {
            var query = new StudyRoomVideoEmailQuery("d7140ca0-2b83-4ce5-b35c-ab190086fe18_1575469575");
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
            var result = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData(159039)]
        [InlineData(638)]
        public async Task UserDataByIdQuery_Ok(long userId)
        {
            var query = new UserDataByIdQuery(userId);
            var _ = await fixture.QueryBus.QueryAsync<User>(query, default);
            await fixture.QueryBus.QueryAsync<IEnumerable<TransactionDto>> (query, default);
        }

        [Fact]
        public async Task CalendarEventsQuery_Ok()
        {
            var query = new CalendarEventsQuery(159039, DateTime.UtcNow, DateTime.UtcNow.AddDays(5));
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
        public async Task AdminCouponQuery_Ok()
        {
            var query = new AdminCouponQuery();
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }


        [Fact]
        public async Task UserCalendarByIdQuery_Ok()
        {
            var query = new UserCalendarByIdQuery(638);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }

        [Theory]
        [InlineData("Economics")]
        [InlineData("31010-אנגלית רמה A למדעי הרוח")]
        public async Task CourseSubjectQuery_Ok(string courseName)
        {
            var query = new CourseSubjectQuery(courseName);
            var _ = await fixture.QueryBus.QueryAsync(query, default);
        }
    }
}