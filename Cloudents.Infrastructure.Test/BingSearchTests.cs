using System.Collections.Generic;
using System.Reflection;
using Autofac.Extras.Moq;
using Cloudents.Infrastructure.Search;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Infrastructure.Test
{
    [TestClass]
    public class BingSearchTests
    {
        [TestMethod]
        public void BuildQuery_Nothing_ReturnDefaultTerm()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var sut = mock.Create<BingSearch>();
                var privateObj = new PrivateObject(sut);
                var result =  privateObj.Invoke("BuildQuery",
                    BindingFlags.Static | BindingFlags.NonPublic, null, null, null, null, "hi");

                Assert.AreEqual("hi", result);
            }
        }

        [TestMethod]
        public void BuildQuery_OnlyUniversity_ReturnWhatNeeded()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var sut = mock.Create<BingSearch>();
                var privateObj = new PrivateObject(sut);
                var university = new[] {"uni1", "uni2"};
                var result = privateObj.Invoke("BuildQuery",
                    BindingFlags.Static | BindingFlags.NonPublic, university, null, null, null, "hi");

                Assert.AreEqual(@"(""uni1"" OR ""uni2"")", result);
            }
        }

        [TestMethod]
        public void BuildQuery_OnlyCourse_ReturnWhatNeeded()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var sut = mock.Create<BingSearch>();
                var privateObj = new PrivateObject(sut);
                var courses = new[] {"course1", "course2"};
                var result = privateObj.Invoke("BuildQuery",
                    BindingFlags.Static | BindingFlags.NonPublic, null, courses, null, null, "hi");
                Assert.AreEqual("(course1) AND (course2)", result);
            }
        }

        [TestMethod]
        public void BuildQuery_OnlyTerm_ReturnWhatNeeded()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var sut = mock.Create<BingSearch>();
                var privateObj = new PrivateObject(sut);
                var subject = new[] {"sub1", "sub2"};
                var result = privateObj.Invoke("BuildQuery",
                    BindingFlags.Static | BindingFlags.NonPublic, null, null, subject, null, "hi");
                Assert.AreEqual("(sub1) AND (sub2)", result);
            }
        }

        [TestMethod]
        public void BuildQuery_AllParams_ReturnWhatNeeded()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var sut = mock.Create<BingSearch>();
                var privateObj = new PrivateObject(sut);
                var university = new[] {"uni1", "uni2"};
                var courses = new[] {"course1", "course2"};
                var subject = new[] {"sub1", "sub2"};
                const string docType = "doc1";
                var result = privateObj.Invoke("BuildQuery",
                    BindingFlags.Static | BindingFlags.NonPublic, university, courses, subject, docType, "hi");

                Assert.AreEqual(@"(""uni1"" OR ""uni2"") AND (course1) AND (course2) AND (sub1) AND (sub2) AND (doc1)",
                    result);
            }
        }

        [TestMethod]
        public void BuildQuery_TermWithNull_ReturnDefaultValue()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var sut = mock.Create<BingSearch>();
                var privateObj = new PrivateObject(sut);
                var subject = new string[] {null};
                var result = privateObj.Invoke("BuildQuery",
                    BindingFlags.Static | BindingFlags.NonPublic, null, null, subject, null, "hi");

                Assert.AreEqual("hi", result);
            }
        }


        [TestMethod]
        public void BuildSources_SomeSource_sitePrefix()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var sut = mock.Create<BingSearch>();
                var privateObj = new PrivateObject(sut);
                var subject = new List<string> { "someDomain.com" };
                var result = (string)privateObj.Invoke("BuildSources",
                    BindingFlags.Static | BindingFlags.NonPublic, subject);

                result.Should().StartWith("(site:someDomain.com)");
            }
        }


        [TestMethod]
        public void BuildSources_EmptyLink_EmptyString()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var sut = mock.Create<BingSearch>();
                var privateObj = new PrivateObject(sut);
                var subject = new List<string>();
                var result = (string)privateObj.Invoke("BuildSources",
                    BindingFlags.Static | BindingFlags.NonPublic, subject);

                result.Should().BeEmpty();
            }
        }


        [TestMethod]
        public void BuildSources_SomeInput_RightInput()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var sut = mock.Create<BingSearch>();
                var privateObj = new PrivateObject(sut);
                var subject = new List<string>() { "bbc.co.uk", "cnn.com" };
                var result = (string)privateObj.Invoke("BuildSources",
                    BindingFlags.Static | BindingFlags.NonPublic, subject);

                result.Should().Be("(site:bbc.co.uk OR site:cnn.com)");
            }
        }


    }
}
