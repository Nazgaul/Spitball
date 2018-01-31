using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
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

        [TestMethod]
        public void ConvertToResult_CourseHeroWebPage_ShareSaleUrl()
        {
            const string courseHeroLink =
                "https://www.coursehero.com/file/11150425/Calculus-Basics-you-should-know-class-notes/";
            //var bingClient = new BingSearch(_clientMock.Object, _mapperMock.Object);
            //var obj = new PrivateObject(bingClient);
            var bingConverter = new BingConverter(_keyGenerator.Object);
            var argument = new BingSearch.WebPage
            {
                Url = courseHeroLink

            };
            var result = bingConverter.Convert(argument, null, null);
            //var result = obj.Invoke("ConvertToResult", argument);
            const string resultLink =
                "http://shareasale.com/r.cfm?b=661825&u=1469379&m=55976&urllink=www.coursehero.com/file/11150425/Calculus-Basics-you-should-know-class-notes/&afftrack=";
            //if (result is SearchResult p)
            //{
            Assert.AreEqual(result.Url, resultLink);
            Assert.AreEqual(result.Source, "www.coursehero.com");
            return;
            //}
            //Assert.Fail("Should be searchResult");
        }


        [TestMethod]
        public void ConvertToResult_RegularWebPage_RegularUrl()
        {
            var bingConverter = new BingConverter(_keyGenerator.Object);
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
