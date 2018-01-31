using AutoMapper;
using Cloudents.Infrastructure.Search;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cloudents.Infrastructure.Test
{
    [TestClass]
    public class BingSearchTests
    {
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
            Assert.AreEqual("(course1) AND (course2)", result);
        }

        [TestMethod]
        public void BuildQuery_OnlyTerm_ReturnWhatNeeded()
        {
            var subject = new[] { "sub1", "sub2" };
            var result = BingSearch.BuildQuery(null, null, subject, null, "hi");
            Assert.AreEqual("(sub1) AND (sub2)", result);
        }

        [TestMethod]
        public void BuildQuery_AllParams_ReturnWhatNeeded()
        {
            var university = new[] {"uni1", "uni2"};
            var courses = new[] {"course1", "course2"};
            var subject = new[] {"sub1", "sub2"};
            const string docType = "doc1";
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
