//using System.Collections.Generic;
//using Autofac.Extras.Moq;
//using AutoMapper;
//using Cloudents.Core;
//using Moq;
//using Xunit;

//namespace Cloudents.Infrastructure.Test.Converters
//{
//    public class BingConverterTests
//    {
//        //private readonly Mock<IKeyGenerator> _keyGenerator = new Mock<IKeyGenerator>();
//        //private readonly Mock<IMappingOperationOptions> _mockOptions = new Mock<IMappingOperationOptions>();
//        private readonly Mock<IRuntimeMapper> _mockMapping = new Mock<IRuntimeMapper>();

//        private BingConverter _bingConverter;
//        private ResolutionContext _context;

//        //private readonly Mock<ReplaceImageProvider> _replaceImageMock = new Mock<ReplaceImageProvider>(new Mock<IBlobProvider<SpitballContainer>>().Object);

//        public BingConverterTests()
//        {
//            using (var autofackMock = AutoMock.GetLoose())
//            {
//                var mockOptions = autofackMock.Mock<IMappingOperationOptions>();
//                var domainParser = autofackMock.Mock<IDomainParser>();
//                domainParser.Setup(s => s.GetDomain("www.coursehero.com")).Returns("coursehero");
//                domainParser.Setup(s => s.GetDomain("www.spitball.co")).Returns("spitball");
//                domainParser.Setup(s => s.GetDomain("www.someurl.com")).Returns("someUrl");

//                IReadOnlyDictionary<string, PrioritySource> priorities = new Dictionary<string, PrioritySource>();

//                mockOptions.Setup(s => s.Items).Returns(new Dictionary<string, object>
//                {
//                    [BingConverter.KeyPriority] = priorities
//                });
//                _context = new ResolutionContext(mockOptions.Object, _mockMapping.Object);
//                _bingConverter = autofackMock.Create<BingConverter>();
//            }
//        }



//        [Fact]
//        public void ConvertToResult_CourseHeroWebPage_ShareSaleUrl()
//        {
//            const string courseHeroLink =
//                "https://www.coursehero.com/file/11150425/Calculus-Basics-you-should-know-class-notes/";

//            var argument = new BingWebPage
//            {
//                Url = courseHeroLink

//            };
//            var result = _bingConverter.Convert(argument, null, _context);
//            result.Source.Should().BeEquivalentTo("coursehero");
//        }

//        [Fact]
//        public void ConvertToResult_RegularWebPage_RegularUrl()
//        {
//            const string someDomainUrl = "https://www.someUrl.com";
//            var argument = new BingWebPage
//            {
//                Url = someDomainUrl

//            };
//            var result = _bingConverter.Convert(argument, null, _context);
//            Assert.Equal(result.Url, someDomainUrl);
//            result.Source.Should().BeEquivalentTo("someurl");
//        }


//        [Fact]
//        public void ConvertToResult_SpitballWebPage_CloudentsSource()
//        {
//            const string spitballDomainUrl = "https://www.spitball.co";
//            var argument = new BingWebPage
//            {
//                Url = spitballDomainUrl

//            };
//            var result = _bingConverter.Convert(argument, null, _context);
//            Assert.Equal(result.Url, spitballDomainUrl);
//            result.Source.Should().BeEquivalentTo("cloudents");
//            //Assert.AreEqual(result.Source, "cloudents");
//        }

//        [Fact]
//        public void ConvertToResult_SomeWebPageWithNullDomain_HasPriority()
//        {
//            var argument = new BingWebPage
//            {
//                Url = "https://www.someUrl2.com"

//            };
//            var result = _bingConverter.Convert(argument, null, _context);
//            result.PrioritySource.Should().NotBeNull();
//        }


//        [Fact]
//        public void ConvertToResult_SomeWebPage_HasPriority()
//        {
//            var argument = new BingWebPage
//            {
//                Url = "https://www.someUrl.com"

//            };
//            var result = _bingConverter.Convert(argument, null, _context);
//            result.PrioritySource.Should().NotBeNull();
//        }
//    }
//}
