using Cloudents.Core.Message.Email;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Moq;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.FunctionsV2.Test
{
    public class CommunicationFunctionTests
    {
        private readonly ILogger _logger = TestFactory.CreateLogger();

        [Fact]
        public async Task EmailFunctionAsync_SomeText_JsonFailed()
        {
            var cloudMessage = new CloudQueueMessage("xxx");
            await Assert.ThrowsAsync<JsonReaderException>(() => CommunicationFunction.EmailFunctionAsync(cloudMessage, null, _logger, CancellationToken.None));
        }


        [Fact]
        public async Task EmailFunctionAsync_Some_JsonFailed()
        {
            var v = new GotAnswerEmail("some question text", "some text", "some answer text", "some link",
                CultureInfo.InvariantCulture);

            var jsonText = JsonConvert.SerializeObject(v, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            var emailProviderStub = new Mock<IAsyncCollector<SendGridMessage>>();
            var cloudMessage = new CloudQueueMessage(jsonText);
            await CommunicationFunction.EmailFunctionAsync(cloudMessage, emailProviderStub.Object, _logger, CancellationToken.None);
        }
    }
}
