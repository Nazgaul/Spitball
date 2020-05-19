using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace Cloudents.Core.Test.Entities
{
    public class DocumentPriceTests
    {
        [Fact]
        public void DocumentPrice_New_Subscriber()
        {
            var tutorMoq = new Mock<Tutor>();
            tutorMoq.Setup(s => s.SubscriptionPrice).Returns(new Money(30, "USD"));


            var documentPrice = new DocumentPrice(50, PriceType.Subscriber,tutorMoq.Object);
            documentPrice.Type.Should().Be(PriceType.Subscriber);
            documentPrice.Price.Should().Be(30);
        }
    }
}