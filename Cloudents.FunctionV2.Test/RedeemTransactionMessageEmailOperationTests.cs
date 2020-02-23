using Cloudents.Core.DTOs;
using Cloudents.Core.Message.Email;
using Cloudents.Query;
using Cloudents.Query.Email;
using FluentAssertions;
using Microsoft.Azure.WebJobs;
using Moq;
using SendGrid.Helpers.Mail;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.FunctionsV2.Operations;
using Xunit;
using Xunit.Abstractions;

namespace Cloudents.FunctionsV2.Test
{
    public class RedeemTransactionMessageEmailOperationTests
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly Mock<IQueryBus> _queryBusStub = new Mock<IQueryBus>();
        private readonly TestAsyncCollector<SendGridMessage> _mockedResult = new TestAsyncCollector<SendGridMessage>();
        private readonly Mock<IBinder> _mock;

        public RedeemTransactionMessageEmailOperationTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            _mock = new Mock<IBinder>();
            _mock
                .Setup(x => x.BindAsync<IAsyncCollector<SendGridMessage>>(It.IsAny<SendGridAttribute>(), CancellationToken.None))
                .ReturnsAsync(_mockedResult);
        }


        [Fact]
        public async Task DoOperationAsync_Hebrew()
        {
            var queryResult = new RedeemEmailDto()
            {
                Country = "IL"
            };
            _queryBusStub.Setup(x =>
                x.QueryAsync(It.IsAny<RedeemEmailQuery>(), CancellationToken.None))
                .ReturnsAsync(queryResult);

            var operation = new RedeemTransactionMessageEmailOperation(_queryBusStub.Object);
            var msg = new RedeemTransactionMessage(Guid.Empty);
            await operation.DoOperationAsync(msg, _mock.Object, default);
            var result = _mockedResult.Result.First();
            result.Personalizations[0].Tos[0].Email.Should().Be("support@spitball.co");
        }

        [Fact]
        public async Task DoOperationAsync_Frymo()
        {
            var queryResult = new RedeemEmailDto()
            {
                Country = "IN"
            };
            _queryBusStub.Setup(x =>
                    x.QueryAsync(It.IsAny<RedeemEmailQuery>(), CancellationToken.None))
                .ReturnsAsync(queryResult);

            var operation = new RedeemTransactionMessageEmailOperation(_queryBusStub.Object);
            var msg = new RedeemTransactionMessage(Guid.Empty);
            await operation.DoOperationAsync(msg, _mock.Object, default);

            CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = new CultureInfo("en-IN");
            var result = _mockedResult.Result.First();
            _outputHelper.WriteLine("The culture is {0}", CultureInfo.CurrentCulture);
            result.Personalizations[0].Tos[0].Email.Should().Be("support@frymo.com");
        }
    }
}
