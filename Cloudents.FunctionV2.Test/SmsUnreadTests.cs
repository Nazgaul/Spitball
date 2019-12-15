using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Query;
using Cloudents.Query.Chat;
using FluentAssertions;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using SendGrid.Helpers.Mail;
using Twilio.Rest.Api.V2010.Account;
using Xunit;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2.Test
{
    public class SmsUnreadTests
    {
        private readonly ILogger _logger = TestFactory.CreateLogger();

        private readonly Mock<CloudBlockBlob> _mockBlob = new Mock<CloudBlockBlob>(new Uri("http://tempuri.org/blob"));
        private readonly Mock<ICommandBus> _mockCommandBus = new Mock<ICommandBus>();
        private readonly Mock<IUrlBuilder> _mockUriBuilder = new Mock<IUrlBuilder>();


        private readonly TestAsyncCollector<CreateMessageOptions> _mockedResultSms = new TestAsyncCollector<CreateMessageOptions>();
        private readonly TestAsyncCollector<SendGridMessage> _mockedResultEmail = new TestAsyncCollector<SendGridMessage>();
        //private readonly TestAsyncCollector<SmsUnread.RequestTutorEmailDto> _mockedEmailResult = new TestAsyncCollector<SmsUnread.RequestTutorEmailDto>();
        private readonly Mock<IQueryBus> _queryBusStub = new Mock<IQueryBus>();



        private readonly Uri _shortUrl = new Uri("http://www.somesite.com");
        private readonly Uri _shortUrlIndia = new Uri("http://www.somesite2.com?site=xxx");
        public SmsUnreadTests()
        {
            _mockUriBuilder.Setup(s => s.BuildChatEndpoint(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(new Uri("http://tempuri.org/blob"));

            _mockUriBuilder.Setup(s => s.BuildShortUrlEndpoint(It.IsAny<string>(),
                It.Is<string>(s2 => s2 != null && s2.Equals("IN", StringComparison.OrdinalIgnoreCase) == false)))
                .Returns(_shortUrl);

            _mockUriBuilder.Setup(s => s.BuildShortUrlEndpoint(It.IsAny<string>(),
                    null)).Returns(_shortUrl);

            _mockUriBuilder.Setup(s => s.BuildShortUrlEndpoint(It.IsAny<string>(),
                    It.Is<string>(s2 =>s2 != null && s2.Equals("IN", StringComparison.OrdinalIgnoreCase))))
                .Returns(_shortUrlIndia);
        }

        [Fact]
        public async Task SmsUnreadAsync_UserMultipleMessages_RequestTutor()
        {
           // var resultMoq = new Mock<UnreadMessageDto>();


            var result = new List<UnreadMessageDto>()
            {
                new UnreadMessageDto()
                {
                    UserId = 1,
                    Version = BitConverter.GetBytes(1L),
                    ChatMessagesCount = 1,
                    PhoneNumber = "+972523556456",
                    Email = "hadar@cloudents.com",
                    Country = "IL"
                },
                new UnreadMessageDto()
                {
                    UserId = 1,
                    Version = BitConverter.GetBytes(2L),
                    ChatMessagesCount = 1,
                     PhoneNumber = "+972523556456",
                    Email = "hadar@cloudents.com",
                    Country = "IL"

                }
            };
            _queryBusStub.Setup(s => s.QueryAsync(It.IsAny<UserUnreadMessageQuery>(), default)).ReturnsAsync(result);

            await SmsUnread.SmsUnreadAsync(null, _mockBlob.Object,
                _mockedResultSms, _mockedResultEmail, _queryBusStub.Object, _mockCommandBus.Object, new TestDataProtector(),
                _mockUriBuilder.Object, _logger, default);

            _mockedResultSms.Result.Should().HaveCount(1);
        }


        [Fact]
        public async Task SmsUnreadAsync_IndiaUser_OnlyEmail()
        {
            // var resultMoq = new Mock<UnreadMessageDto>();


            var result = new List<UnreadMessageDto>()
            {
                new UnreadMessageDto()
                {
                    UserId = 1,
                    Version = BitConverter.GetBytes(1L),
                    ChatMessagesCount = 1,
                    PhoneNumber = "+91523556456",
                    Email = "hadar@cloudents.com",
                    CultureInfo = new CultureInfo("en"),
                    Country = "IN"
                }
            };
            _queryBusStub.Setup(s => s.QueryAsync(It.IsAny<UserUnreadMessageQuery>(), default)).ReturnsAsync(result);

            await SmsUnread.SmsUnreadAsync(null, _mockBlob.Object,
                _mockedResultSms, _mockedResultEmail, _queryBusStub.Object, _mockCommandBus.Object, new TestDataProtector(),
                _mockUriBuilder.Object, _logger, default);

            _mockedResultSms.Result.Should().HaveCount(0);
            _mockedResultEmail.Result.Should().HaveCount(1);

            _mockedResultEmail.Result.First().From.Name.Should().BeEquivalentTo("frymo");
        }

        [Fact]
        public async Task SmsUnreadAsync_NotIndiaUser_EmailAndSms()
        {
            // var resultMoq = new Mock<UnreadMessageDto>();


            var result = new List<UnreadMessageDto>()
            {
                new UnreadMessageDto()
                {
                    UserId = 1,
                    Version = BitConverter.GetBytes(1L),
                    ChatMessagesCount = 1,
                    PhoneNumber = "+72523556456",
                    Email = "hadar@cloudents.com",
                    CultureInfo = new CultureInfo("en"),
                    Country = "Il"
                }
            };
            _queryBusStub.Setup(s => s.QueryAsync(It.IsAny<UserUnreadMessageQuery>(), default)).ReturnsAsync(result);

            await SmsUnread.SmsUnreadAsync(null, _mockBlob.Object,
                _mockedResultSms, _mockedResultEmail, _queryBusStub.Object, _mockCommandBus.Object, new TestDataProtector(),
                _mockUriBuilder.Object, _logger, default);

            _mockedResultSms.Result.Should().HaveCount(1);
            _mockedResultEmail.Result.Should().HaveCount(1);


            _mockedResultEmail.Result.First().From.Name.Should().BeEquivalentTo("spitball");

        }



        [Theory]
        [InlineData("en", "US")]
       // [InlineData("en", "IN")]
        [InlineData("he", "IL")]
        public async Task SmsUnreadAsync_FirstMessage_ResourceOk(string culture, string country)
        {
            var result = new List<UnreadMessageDto>()
            {
                new UnreadMessageDto()
                {
                    UserId = 1,
                    Version = BitConverter.GetBytes(1L),
                    ChatMessagesCount = 1,
                    PhoneNumber = "+972523556456",
                    Email = "hadar@cloudents.com",
                    CultureInfo = new CultureInfo(culture),
                    Country = country
                }

            };
            _queryBusStub.Setup(s => s.QueryAsync(It.IsAny<UserUnreadMessageQuery>(), default)).ReturnsAsync(result);

            await SmsUnread.SmsUnreadAsync(null, _mockBlob.Object,
                _mockedResultSms, _mockedResultEmail, _queryBusStub.Object, _mockCommandBus.Object, new TestDataProtector(),
                _mockUriBuilder.Object, _logger, default);

            var body = _mockedResultSms.Result.Single().Body;
            var expectedResult = ResourceWrapper.GetString("unread_message_first_message_tutor").InjectSingleValue("link", _shortUrl);
            body.Should().BeEquivalentTo(expectedResult);
            var emailExpectedResult = ResourceWrapper.GetString("unread_message_first_message_tutor_email").InjectSingleValue("link", _shortUrl);
            _mockedResultEmail.Result.Single().Contents.First().Value.Should().Contain(emailExpectedResult);
        }

        [Fact]
        public async Task SmsUnreadAsync_IndiaFirstMessage_ResourceOk()
        {
            var result = new List<UnreadMessageDto>()
            {
                new UnreadMessageDto()
                {
                    UserId = 1,
                    Version = BitConverter.GetBytes(1L),
                    ChatMessagesCount = 1,
                    // FirstMessageStudentName = "Ram",
                    PhoneNumber = "+972523556456",
                    Email = "hadar@cloudents.com",
                    CultureInfo = new CultureInfo("en"),
                    Country = "In"
                    // CourseName = "Ram"
                }

            };
            _queryBusStub.Setup(s => s.QueryAsync(It.IsAny<UserUnreadMessageQuery>(), default)).ReturnsAsync(result);

            await SmsUnread.SmsUnreadAsync(null, _mockBlob.Object,
                _mockedResultSms, _mockedResultEmail, _queryBusStub.Object, _mockCommandBus.Object, new TestDataProtector(),
                _mockUriBuilder.Object, _logger, default);

            var emailExpectedResult = ResourceWrapper.GetString("unread_message_first_message_tutor_email").InjectSingleValue("link", _shortUrlIndia);
            _mockedResultEmail.Result.Single().Contents.First().Value.Should().Contain(emailExpectedResult);
        }
    }
}
