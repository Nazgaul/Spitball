using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using FluentAssertions;
using Xunit;

namespace Cloudents.Core.Test.Entities
{
    public class DocumentPriceTests
    {
        [Fact]
        public void DocumentPrice_New_Subscriber()
        {
            var documentPrice = new DocumentPrice(50, PriceType.Subscriber);
            documentPrice.Type.Should().Be(PriceType.Subscriber);
            documentPrice.Price.Should().Be(0);
        }
    }
}