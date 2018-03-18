using System.Collections.Generic;
using Autofac.Extras.Moq;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Infrastructure.Converters;
using Cloudents.Infrastructure.Domain;
using Cloudents.Infrastructure.Search;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cloudents.Infrastructure.Test.Converters
{
    [TestClass]
    public class BingConverterTests
    {
        //private readonly Mock<IKeyGenerator> _keyGenerator = new Mock<IKeyGenerator>();
        //private readonly Mock<IMappingOperationOptions> _mockOptions = new Mock<IMappingOperationOptions>();
        private readonly Mock<IRuntimeMapper> _mockMapping = new Mock<IRuntimeMapper>();

        private BingConverter _bingConverter;
        private ResolutionContext _context;

        //private readonly Mock<ReplaceImageProvider> _replaceImageMock = new Mock<ReplaceImageProvider>(new Mock<IBlobProvider<SpitballContainer>>().Object);

        [TestInitialize]
        public void Setup()
        {
            using (var autofackMock = AutoMock.GetLoose())
            {
                var mockOptions = autofackMock.Mock<IMappingOperationOptions>();
                var domainParser = autofackMock.Mock<IDomainParser>();
                domainParser.Setup(s => s.GetDomain("www.coursehero.com")).Returns("coursehero");
                domainParser.Setup(s => s.GetDomain("www.spitball.co")).Returns("spitball");
                domainParser.Setup(s => s.GetDomain("www.someurl.com")).Returns("someUrl");

                IReadOnlyDictionary<string, PrioritySource> priorities = new Dictionary<string, PrioritySource>();

                mockOptions.Setup(s => s.Items).Returns(new Dictionary<string, object>
                {
                    [BingConverter.KeyPriority] = priorities
                });
                _context = new ResolutionContext(mockOptions.Object, _mockMapping.Object);
                _bingConverter = autofackMock.Create<BingConverter>();
            }
        }

       

        [TestMethod]
        public void ConvertToResult_CourseHeroWebPage_ShareSaleUrl()
        {
            const string courseHeroLink =
                "https://www.coursehero.com/file/11150425/Calculus-Basics-you-should-know-class-notes/";

            var argument = new BingSearch.WebPage
            {
                Url = courseHeroLink

            };
            var result = _bingConverter.Convert(argument, null, _context);
            const string resultLink =
                "http://shareasale.com/r.cfm?b=661825&u=1469379&m=55976&urllink=www.coursehero.com/file/11150425/Calculus-Basics-you-should-know-class-notes/&afftrack=";
            Assert.AreEqual(result.Url, resultLink);
            result.Source.Should().BeEquivalentTo("coursehero");

        }

        [TestMethod]
        public void ConvertToResult_RegularWebPage_RegularUrl()
        {
            var argument = new BingSearch.WebPage
            {
                Url = "https://www.spitball.co"

            };
            var result = _bingConverter.Convert(argument, null, _context);
            Assert.AreEqual(result.Url, "https://www.spitball.co");
            Assert.AreEqual(result.Source, "spitball");
        }

        [TestMethod]
        public void ConvertToResult_SomeWebPageWithNullDomain_HasPriority()
        {
            var argument = new BingSearch.WebPage
            {
                Url = "https://www.someUrl2.com"

            };
            var result = _bingConverter.Convert(argument, null, _context);
            result.PrioritySource.Should().NotBeNull();
        }


        [TestMethod]
        public void ConvertToResult_SomeWebPage_HasPriority()
        {
            var argument = new BingSearch.WebPage
            {
                Url = "https://www.someUrl.com"

            };
            var result = _bingConverter.Convert(argument, null, _context);
            result.PrioritySource.Should().NotBeNull();
        }
    }
}
