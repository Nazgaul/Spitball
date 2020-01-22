using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Query;
using Cloudents.Query.General;
using Cloudents.Web.Controllers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Cloudents.Web.Test.UnitTests
{
    public class GoControllerTests
    {
        private readonly Mock<IQueryBus> _queryBusMoq = new Mock<IQueryBus>();
        private readonly Mock<IGoogleAnalytics> _googleAnalyticsMoq = new Mock<IGoogleAnalytics>();
       
      

        [Fact]
        public async Task GetAsync_NoResult_RedirectBaseAsync()
        {
            var controller = new GoController(_queryBusMoq.Object, _googleAnalyticsMoq.Object);
            var result = await controller.Index("xxx", null, null, null, null, default);
            result.Url.Should().Be("/");
        }


        [Fact]
        public async Task GetAsync_SomeIdentifier_OkAsync()
        {
            _queryBusMoq.Setup(s => s.QueryAsync(It.IsAny<ShortUrlQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ShortUrlDto() {Destination = "/test"});

            var controller = new GoController(_queryBusMoq.Object, _googleAnalyticsMoq.Object);
            var result = await controller.Index("xxx", null, null, null, null, default);
            result.Url.Should().Be("/test");
        }

        [Fact]
        public async Task GetAsync_SomeIdentifierWithoutQuerySite_OkAsync()
        {
            _queryBusMoq.Setup(s => s.QueryAsync(It.IsAny<ShortUrlQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ShortUrlDto() { Destination = "/test" });

            var controller = new GoController(_queryBusMoq.Object, _googleAnalyticsMoq.Object);
            var result = await controller.Index("xxx", "frymo", null, null, null, default);
            result.Url.Should().Be("/test?site=frymo");
        }

        [Fact]
        public async Task GetAsync_SomeIdentifierWithQuerySite_OkAsync()
        {
            _queryBusMoq.Setup(s => s.QueryAsync(It.IsAny<ShortUrlQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ShortUrlDto() { Destination = "/test?v=1" });

            var controller = new GoController(_queryBusMoq.Object, _googleAnalyticsMoq.Object);
            var result = await controller.Index("xxx", "frymo",null,null,null, default);
            result.Url.Should().Be("/test?v=1&site=frymo");
        }
    }
}
