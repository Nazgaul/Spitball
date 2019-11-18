using Cloudents.Core.DTOs;
using Cloudents.Core.Message.Email;
using Cloudents.FunctionsV2.System;
using Cloudents.Query;
using Cloudents.Query.Email;
using FluentAssertions;
using Microsoft.Azure.WebJobs;
using Moq;
using SendGrid.Helpers.Mail;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.FunctionsV2.Test
{
    public class RedeemTransactionMessageEmailOperationTests
    {

        Mock<IQueryBus> queryBusStub = new Mock<IQueryBus>();
        TestAsyncCollector<SendGridMessage> mockedResult = new TestAsyncCollector<SendGridMessage>();
        private Mock<IBinder> mock;

        public RedeemTransactionMessageEmailOperationTests()
        {
            mock = new Mock<IBinder>();
            mock
                .Setup(x => x.BindAsync<IAsyncCollector<SendGridMessage>>(It.IsAny<SendGridAttribute>(), CancellationToken.None))
                .ReturnsAsync(mockedResult);
        }


        [Fact]
        public async Task DoOperationAsync_Hebrew()
        {
            var queryResult = new RedeemEmailDto()
            {
                Country = "IL"
            };
            queryBusStub.Setup(x =>
                x.QueryAsync(It.IsAny<RedeemEmailQuery>(), CancellationToken.None))
                .ReturnsAsync(queryResult);

            var operation = new RedeemTransactionMessageEmailOperation(queryBusStub.Object);
            var msg = new RedeemTransactionMessage(Guid.Empty);
            await operation.DoOperationAsync(msg, mock.Object, default);
            var result = mockedResult.Result.First();
            result.Personalizations[0].Tos[0].Email.Should().Be("support@spitball.co");
        }

        [Fact]
        public async Task DoOperationAsync_Frymo()
        {
            var queryResult = new RedeemEmailDto()
            {
                Country = "IN"
            };
            queryBusStub.Setup(x =>
                    x.QueryAsync(It.IsAny<RedeemEmailQuery>(), CancellationToken.None))
                .ReturnsAsync(queryResult);

            var operation = new RedeemTransactionMessageEmailOperation(queryBusStub.Object);
            var msg = new RedeemTransactionMessage(Guid.Empty);
            await operation.DoOperationAsync(msg, mock.Object, default);
            var result = mockedResult.Result.First();
            result.Personalizations[0].Tos[0].Email.Should().Be("support@frymo.com");
        }
    }
}
