using Cloudents.Core.DTOs;
using Cloudents.FunctionsV2.Services;
using Cloudents.Query;
using Cloudents.Query.Email;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.FunctionsV2.Test
{
    public class EmailUpdateFunctionTests
    {
        private readonly Mock<IQueryBus> _queryBusStub;
        private readonly Mock<IDataProtectionService> _dataProtectProviderStub;
        private readonly Mock<IHostUriService> _hostUriService = new Mock<IHostUriService>();


        public EmailUpdateFunctionTests()
        {
            _queryBusStub = new Mock<IQueryBus>();
            _dataProtectProviderStub = new Mock<IDataProtectionService>();
        }

        [Fact]
        public async Task SendEmail_UserWithHebrew_EmailIsRtl()
        {
            var user = new UpdateUserEmailDto()
            {
                UserId = 159039,
                UserName = "Some name",
                Language = new CultureInfo("he"),
                ToEmailAddress = "some email"

            };

            List<DocumentUpdateEmailDto> update = new List<DocumentUpdateEmailDto>();
            update.Add(new DocumentUpdateEmailDto()
            {
                Id = 11,
                Name = "Load Stress Testing Multimi2.docx",
                Course = "Box Read for hotmail user",
                UserId = 2641,
                UserName = "eapelbaum.6791",
                UserImage = null,

            });

            _queryBusStub.Setup(s => s.QueryAsync(It.IsAny<GetUpdatesEmailByUserQuery>(), default)).ReturnsAsync(update);

            var asyncCollector = new TestAsyncCollector<SendGridMessage>();

            await EmailUpdateFunction.SendEmail(user,
                asyncCollector,
                 _queryBusStub.Object,
                 null,
                 _dataProtectProviderStub.Object,
                 _hostUriService.Object,
                 CancellationToken.None);

            var message = asyncCollector.Result.First();
           
            var firstObject = message.Personalizations.First();
            var email = (UpdateEmail)firstObject.TemplateData;
            message.TemplateId.Should().Be(EmailUpdateFunction.HebrewTemplateId);
            email.Direction.Should().Be("rtl");
            
        }

        [Fact]
        public async Task SendEmail_NoUpdates_ReturnNull()
        {
            var user = new UpdateUserEmailDto()
            {
                UserId = 0,
                UserName = "Some name",
                Language = new CultureInfo("he"),
                ToEmailAddress = "some email"

            };

            
            var asyncCollector = new TestAsyncCollector<SendGridMessage>();

            await EmailUpdateFunction.SendEmail(user,
                asyncCollector,
                 _queryBusStub.Object,
                 null,
                 _dataProtectProviderStub.Object,
                 _hostUriService.Object,
                 CancellationToken.None);

            var message = asyncCollector.Result.FirstOrDefault();
            message.Should().BeNull();
        }

        [Fact]
        public async Task SendEmail_UserWithEnglish_EmailIsLtr()
        {
            var user = new UpdateUserEmailDto()
            {
                UserName = "Some name",
                Language = new CultureInfo("en"),
                ToEmailAddress = "some email"

            };

            List<DocumentUpdateEmailDto> update = new List<DocumentUpdateEmailDto>();
            update.Add(new DocumentUpdateEmailDto()
            {
                Id = 11,
                Name = "Load Stress Testing Multimi2.docx",
                Course = "Box Read for hotmail user",
                UserId = 2641,
                UserName = "eapelbaum.6791",
                UserImage = null,

            });

            _queryBusStub.Setup(s => s.QueryAsync(It.IsAny<GetUpdatesEmailByUserQuery>(), default)).ReturnsAsync(update);

            var asyncCollector = new TestAsyncCollector<SendGridMessage>();
            await EmailUpdateFunction.SendEmail(user,
                asyncCollector,
                _queryBusStub.Object,
                null,
                _dataProtectProviderStub.Object,
                _hostUriService.Object,
                CancellationToken.None);

            var message = asyncCollector.Result.First();
            var firstObject = message.Personalizations.First();
            var email = (UpdateEmail)firstObject.TemplateData;
            message.TemplateId.Should().Be(EmailUpdateFunction.EnglishTemplateId);
            email.Direction.Should().Be("ltr");
        }


        [Fact]
        public void RunOrchestrator_UpdateUserEmailDto_SerializeOk()
        {
            var user = new UpdateUserEmailDto()
            {
                UserName = "Some name",
                Language = new CultureInfo("he"),
                ToEmailAddress = "some email"
            };

            var json = JsonConvert.SerializeObject(user);
            var user2 = JsonConvert.DeserializeObject<UpdateUserEmailDto>(json);

            user2.Should().BeEquivalentTo(user);
        }
    }
}
