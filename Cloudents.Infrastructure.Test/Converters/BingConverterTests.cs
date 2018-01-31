using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Infrastructure.Converters;
using Cloudents.Infrastructure.Search;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cloudents.Infrastructure.Test.Converters
{
    [TestClass]
    public class BingConverterTests
    {
        private readonly Mock<IKeyGenerator> _keyGenerator = new Mock<IKeyGenerator>();
//        private readonly Mock<IBlobProvider<SpitballContainer>> _blobMock = new Mock<IBlobProvider<SpitballContainer>>();
        private readonly Mock<ReplaceImageProvider> _replaceImageMock = new Mock<ReplaceImageProvider>(new Mock<IBlobProvider<SpitballContainer>>().Object);

        [TestMethod]
        public void ConvertToResult_CourseHeroWebPage_ShareSaleUrl()
        {
            const string courseHeroLink =
                "https://www.coursehero.com/file/11150425/Calculus-Basics-you-should-know-class-notes/";
            var bingConverter = new BingConverter(_keyGenerator.Object, _replaceImageMock.Object);
            var argument = new BingSearch.WebPage
            {
                Url = courseHeroLink

            };
            var result = bingConverter.Convert(argument, null, null);
            const string resultLink =
                "http://shareasale.com/r.cfm?b=661825&u=1469379&m=55976&urllink=www.coursehero.com/file/11150425/Calculus-Basics-you-should-know-class-notes/&afftrack=";
            Assert.AreEqual(result.Url, resultLink);
            Assert.AreEqual(result.Source, "www.coursehero.com");
        }

        [TestMethod]
        public void ConvertToResult_RegularWebPage_RegularUrl()
        {
            var bingConverter = new BingConverter(_keyGenerator.Object, _replaceImageMock.Object);
            var argument = new BingSearch.WebPage
            {
                Url = "https://www.spitball.co"

            };
            var result = bingConverter.Convert(argument, null, null);
            Assert.AreEqual(result.Url, "https://www.spitball.co");
            Assert.AreEqual(result.Source, "www.spitball.co");
        }
    }
}
