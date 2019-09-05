using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.FunctionsV2.Services;
using Cloudents.Query;
using FluentAssertions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Azure.WebJobs;
using Moq;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
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
                UserName = "Some name",
                Language = new CultureInfo("he"),
                ToEmailAddress = "some email"

            };

            var asyncCollector = new TestAsyncCollector<SendGridMessage>();
            await EmailUpdateFunction.SendEmail(user,
                asyncCollector,
                 _queryBusStub.Object,
                 null,
                 null,
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
        public async Task SendEmail_UserWithEnglish_EmailIsLtr()
        {
            var user = new UpdateUserEmailDto()
            {
                UserName = "Some name",
                Language = new CultureInfo("en"),
                ToEmailAddress = "some email"

            };

            var asyncCollector = new TestAsyncCollector<SendGridMessage>();
            await EmailUpdateFunction.SendEmail(user,
                asyncCollector,
                _queryBusStub.Object,
                null,
                null,
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
            var user2 = JsonConvert.DeserializeObject< UpdateUserEmailDto>(json);

            user2.Should().BeEquivalentTo(user);
        }
    }
}
