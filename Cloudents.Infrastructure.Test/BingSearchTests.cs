using System;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Search;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cloudents.Infrastructure.Test
{
    [TestClass]
    public class BingSearchTests
    {
        private readonly Mock<IRestClient> _clientMock = new Mock<IRestClient>();
        private readonly Mock<IKeyGenerator> _keyMock = new Mock<IKeyGenerator>();

        [TestMethod]
        public void ConvertToResult_RegularWebPage_RegularUrl()
        {
            var bingClient = new BingSearch(_clientMock.Object, _keyMock.Object);
            var obj = new PrivateObject(bingClient);
            var argument = new BingSearch.WebPage
            {
                Url = "https://www.spitball.co"

            };
            var result = obj.Invoke("ConvertToResult", argument);
            if (result is SearchResult p)
            {
                Assert.AreEqual(p.Url ,"https://www.spitball.co");
                Assert.AreEqual(p.Source, "www.spitball.co");
                return;
            }
            Assert.Fail("Should be searchResult");
        }

        [TestMethod]
        public void ConvertToResult_CourseHeroWebPage_ShareSaleUrl()
        {
            const string courseHeroLink =
                "https://www.coursehero.com/file/11150425/Calculus-Basics-you-should-know-class-notes/";
            var bingClient = new BingSearch(_clientMock.Object, _keyMock.Object);
            var obj = new PrivateObject(bingClient);
            var argument = new BingSearch.WebPage
            {
                Url = courseHeroLink

            };
            var result = obj.Invoke("ConvertToResult", argument);
            const string resultLink =
                "http://shareasale.com/r.cfm?b=661825&u=1469379&m=55976&urllink=www.coursehero.com/file/11150425/Calculus-Basics-you-should-know-class-notes/&afftrack=";
            if (result is SearchResult p)
            {
                Assert.AreEqual(p.Url, resultLink);
                Assert.AreEqual(p.Source, "www.coursehero.com");
                return;
            }
            Assert.Fail("Should be searchResult");
        }

        [TestMethod]
        public void BuildQuery_Nothing_ReturnDefaultTerm()
        {
            var result = BingSearch.BuildQuery(null, null, null, null, "hi");

            Assert.AreEqual("hi",result);
        }

        [TestMethod]
        public void BuildQuery_OnlyUniversity_ReturnWhatNeeded()
        {
            var university = new[] { "uni1", "uni2" };
            var result = BingSearch.BuildQuery(university, null, null, null, "hi");
            Assert.AreEqual(@"(""uni1"" OR ""uni2"")", result);
        }

        [TestMethod]
        public void BuildQuery_OnlyCourse_ReturnWhatNeeded()
        {
            var courses = new[] { "course1", "course2" };
            var result = BingSearch.BuildQuery(null, courses, null, null, "hi");
            Assert.AreEqual(@"(course1) AND (course2)", result);
        }

        [TestMethod]
        public void BuildQuery_OnlyTerm_ReturnWhatNeeded()
        {
            var subject = new[] { "sub1", "sub2" };
            var result = BingSearch.BuildQuery(null, null, subject, null, "hi");
            Assert.AreEqual(@"(sub1) AND (sub2)", result);
        }

        [TestMethod]
        public void BuildQuery_AllParams_ReturnWhatNeeded()
        {
            var university = new[] {"uni1", "uni2"};
            var courses = new[] {"course1", "course2"};
            var subject = new[] {"sub1", "sub2"};
            var docType = "doc1";
            var result = BingSearch.BuildQuery(university, courses, subject, docType, "hi");

            Assert.AreEqual(@"(""uni1"" OR ""uni2"") AND (course1) AND (course2) AND (sub1) AND (sub2) AND (doc1)", result);
        }

        [TestMethod]
        public void BuildQuery_TermWithNull_ReturnDefaultValue()
        {
            var subject = new string[] { null };
            var result = BingSearch.BuildQuery(null, null, subject, null, "hi");

            Assert.AreEqual("hi", result);
        }
    }
}
