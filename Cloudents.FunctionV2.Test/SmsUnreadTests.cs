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


        private readonly TestAsyncCollector<CreateMessageOptions> _mockedResult = new TestAsyncCollector<CreateMessageOptions>();
        //private readonly TestAsyncCollector<SmsUnread.RequestTutorEmailDto> _mockedEmailResult = new TestAsyncCollector<SmsUnread.RequestTutorEmailDto>();
        private readonly Mock<IQueryBus> _queryBusStub = new Mock<IQueryBus>();



        private readonly Uri _shortUrl = new Uri("http://somesite.com");
        public SmsUnreadTests()
        {
            _mockUriBuilder.Setup(s => s.BuildChatEndpoint(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(new Uri("http://tempuri.org/blob"));

            _mockUriBuilder.Setup(s => s.BuildShortUrlEndpoint(It.IsAny<string>()))
                .Returns(_shortUrl);
        }

        [Fact]
        public async Task SmsUnreadAsync_UserMultipleMessages_RequestTutor()
        {
            var resultMoq = new Mock<UnreadMessageDto>();


            var result = new List<UnreadMessageDto>()
            {
                new UnreadMessageDto()
                {
                    UserId = 1,
                    Version = BitConverter.GetBytes(1L),
                    ChatMessagesCount = 1,
                    PhoneNumber = "+972542642202"
                },
                new UnreadMessageDto()
                {
                    UserId = 1,
                    Version = BitConverter.GetBytes(2L),
                    ChatMessagesCount = 1,
                    PhoneNumber = "+972542642202"

                }
            };
            _queryBusStub.Setup(s => s.QueryAsync(It.IsAny<UserUnreadMessageQuery>(), default)).ReturnsAsync(result);

            await SmsUnread.SmsUnreadAsync(null, _mockBlob.Object,
                _mockedResult,  _queryBusStub.Object, _mockCommandBus.Object, new TestDataProtector(),
                _mockUriBuilder.Object, _logger, default);

            _mockedResult.Result.Should().HaveCount(1);
        }


        //[Fact]
        //public async Task SmsUnreadAsync_FirstMessage_WithoutCourse_ResourceOk()
        //{
        //    var result = new List<UnreadMessageDto>()
        //    {
        //        new UnreadMessageDto()
        //        {
        //            UserId = 1,
        //            Version = BitConverter.GetBytes(1L),
        //            ChatMessagesCount = 1,
        //            //FirstMessageStudentName = "Ram",
        //            PhoneNumber = "+972542642202"
        //        }
               
        //    };
        //    _queryBusStub.Setup(s => s.QueryAsync(It.IsAny<UserUnreadMessageQuery>(), default)).ReturnsAsync(result);

        //    await SmsUnread.SmsUnreadAsync(null, _mockBlob.Object,
        //        _mockedResult,  _queryBusStub.Object, _mockCommandBus.Object, new TestDataProtector(),
        //        _mockUriBuilder.Object, _logger, default);

        //    var body = _mockedResult.Result.Single().Body;
        //    var expectedResult = ResourceWrapper.GetString("unread_message_request_without_course").Inject(result.First()).InjectSingleValue("link", _shortUrl);
        //    body.Should().BeEquivalentTo(expectedResult);
        //    //body.Should().Contain(result.First().FirstMessageStudentName);
        //}

        [Fact]
        public async Task SmsUnreadAsync_FirstMessage_ResourceOk()
        {
            var result = new List<UnreadMessageDto>()
            {
                new UnreadMessageDto()
                {
                    UserId = 1,
                    Version = BitConverter.GetBytes(1L),
                    ChatMessagesCount = 1,
                   // FirstMessageStudentName = "Ram",
                    PhoneNumber = "+972542642202",
                   // CourseName = "Ram"
                }

            };
            _queryBusStub.Setup(s => s.QueryAsync(It.IsAny<UserUnreadMessageQuery>(), default)).ReturnsAsync(result);

            await SmsUnread.SmsUnreadAsync(null, _mockBlob.Object,
                _mockedResult,  _queryBusStub.Object, _mockCommandBus.Object, new TestDataProtector(),
                _mockUriBuilder.Object, _logger, default);

            var body = _mockedResult.Result.Single().Body;
            var expectedResult = ResourceWrapper.GetString("unread_message_first_message_tutor").InjectSingleValue("link", _shortUrl);
            body.Should().BeEquivalentTo(expectedResult);
            //body.Should().Contain(result.First().FirstMessageStudentName);

            //_mockedEmailResult.Result.Count().Should().Be(1);
        }


        [Fact]
        public async Task SmsUnreadAsync_UserWithHebrew_ResourceOk()
        {
            var result = new List<UnreadMessageDto>()
            {
                new UnreadMessageDto()
                {
                    UserId = 1,
                    Version = BitConverter.GetBytes(1L),
                    ChatMessagesCount = 1,
                    CultureInfo = new CultureInfo("he"),
                    //FirstMessageStudentName = "Ram",
                    PhoneNumber = "+972542642202",
                   // CourseName = "Ram"
                }

            };
            _queryBusStub.Setup(s => s.QueryAsync(It.IsAny<UserUnreadMessageQuery>(), default)).ReturnsAsync(result);

            await SmsUnread.SmsUnreadAsync(null, _mockBlob.Object,
                _mockedResult,  _queryBusStub.Object, _mockCommandBus.Object, new TestDataProtector(),
                _mockUriBuilder.Object, _logger, default);

            var body = _mockedResult.Result.Single().Body;
            var expectedResult = ResourceWrapper.GetString("unread_message_first_message_tutor").InjectSingleValue("link", _shortUrl);
            body.Should().BeEquivalentTo(expectedResult);
           // body.Should().Contain(result.First().FirstMessageStudentName);
        }

        //[Fact]
        //public async Task SmsUnreadAsync_FirstMessage_EmailTrigger()
        //{
        //    var result = new List<UnreadMessageDto>()
        //    {
        //        new UnreadMessageDto()
        //        {
        //            UserId = 1,
        //            Version = BitConverter.GetBytes(1L),
        //            ChatMessagesCount = 1,
        //            CultureInfo = new CultureInfo("he"),
        //           // FirstMessageStudentName = "Ram",
        //            PhoneNumber = "+972542642202",
        //           // CourseName = "Ram"
        //        }

        //    };
        //    _queryBusStub.Setup(s => s.QueryAsync(It.IsAny<UserUnreadMessageQuery>(), default)).ReturnsAsync(result);

        //    await SmsUnread.SmsUnreadAsync(null, _mockBlob.Object,
        //        _mockedResult,  _queryBusStub.Object, _mockCommandBus.Object, new TestDataProtector(),
        //        _mockUriBuilder.Object, _logger, default);

        //    //_mockedEmailResult.Result.Count().Should().Be(1);
        //}

        //[Fact]
        //public async Task SmsUnreadAsync_Conversation_NoEmail()
        //{
        //    var result = new List<UnreadMessageDto>()
        //    {
        //        new UnreadMessageDto()
        //        {
        //            UserId = 1,
        //            Version = BitConverter.GetBytes(1L),
        //            ChatMessagesCount =2,
        //            CultureInfo = new CultureInfo("he"),
        //            //FirstMessageStudentName = "Ram",
        //            PhoneNumber = "+972542642202",
        //           // CourseName = "Ram"
        //        }

        //    };
        //    _queryBusStub.Setup(s => s.QueryAsync(It.IsAny<UserUnreadMessageQuery>(), default)).ReturnsAsync(result);

        //    await SmsUnread.SmsUnreadAsync(null, _mockBlob.Object,
        //        _mockedResult,  _queryBusStub.Object, _mockCommandBus.Object, new TestDataProtector(),
        //        _mockUriBuilder.Object, _logger, default);

        //   // _mockedEmailResult.Result.Count().Should().Be(0);
        //}
    }
}
