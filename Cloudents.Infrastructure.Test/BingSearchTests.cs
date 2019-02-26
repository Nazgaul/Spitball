//using Autofac.Extras.Moq;
//using Cloudents.Infrastructure.Search;
//using FluentAssertions;
//using System.Collections.Generic;
//using System.Reflection;
//using Cloudents.Core.Test;
//using Xunit;

//namespace Cloudents.Infrastructure.Test
//{
//    public class BingSearchTests
//    {
//        [Fact]
//        public void BuildQuery_Nothing_ReturnDefaultTerm()
//        {
//            using (var mock = AutoMock.GetLoose())
//            {
//                var sut = mock.Create<BingSearch>();

//                var privateObj = new PrivateObject(sut);
//                var result = privateObj.Invoke("BuildQuery",
//                    BindingFlags.Static | BindingFlags.NonPublic, null, null, null, "hi");

//                Assert.Equal("hi", result);
//            }
//        }

//        [Fact]
//        public void BuildQuery_OnlyUniversity_ReturnWhatNeeded()
//        {
//            using (var mock = AutoMock.GetLoose())
//            {
//                var sut = mock.Create<BingSearch>();
//                var privateObj = new PrivateObject(sut);
//                var university = new[] { "uni1", "uni2" };
//                var result = privateObj.Invoke("BuildQuery",
//                    BindingFlags.Static | BindingFlags.NonPublic, university, null, null, "hi");

//                Assert.Equal(@"(""uni1"" OR ""uni2"")", result);
//            }
//        }

//        [Fact]
//        public void BuildQuery_OnlyCourse_ReturnWhatNeeded()
//        {
//            using (var mock = AutoMock.GetLoose())
//            {
//                var sut = mock.Create<BingSearch>();
//                var privateObj = new PrivateObject(sut);
//                string courses ="course1";
//                var result = privateObj.Invoke("BuildQuery",
//                    BindingFlags.Static | BindingFlags.NonPublic, null, courses, null, "hi");
//                Assert.Equal("(course1)", result);
//            }
//        }

//        [Fact]
//        public void BuildQuery_OnlyTerm_ReturnWhatNeeded()
//        {
//            using (var mock = AutoMock.GetLoose())
//            {
//                var sut = mock.Create<BingSearch>();
//                var privateObj = new PrivateObject(sut);
//                var subject = "sub1";
//                var result = privateObj.Invoke("BuildQuery",
//                    BindingFlags.Static | BindingFlags.NonPublic, null, null, subject, "hi");
//                Assert.Equal("(sub1)", result);
//            }
//        }

//        [Fact]
//        public void BuildQuery_AllParams_ReturnWhatNeeded()
//        {
//            using (var mock = AutoMock.GetLoose())
//            {
//                var sut = mock.Create<BingSearch>();
//                var privateObj = new PrivateObject(sut);
//                var university = new[] { "uni1", "uni2" };
//                string course =  "course1";
//                var subject = "sub1";
//                var result = privateObj.Invoke("BuildQuery",
//                    BindingFlags.Static | BindingFlags.NonPublic, university, course, subject, "hi");

//                Assert.Equal(@"(""uni1"" OR ""uni2"") AND (course1) AND (sub1)",
//                    result);
//            }
//        }

//        [Fact]
//        public void BuildQuery_TermWithNull_ReturnDefaultValue()
//        {
//            using (var mock = AutoMock.GetLoose())
//            {
//                var sut = mock.Create<BingSearch>();
//                var privateObj = new PrivateObject(sut);
//                var result = privateObj.Invoke("BuildQuery",
//                    BindingFlags.Static | BindingFlags.NonPublic, null, null, null, "hi");

//                Assert.Equal("hi", result);
//            }
//        }


//        [Fact]
//        public void BuildSources_SomeSource_sitePrefix()
//        {
//            using (var mock = AutoMock.GetLoose())
//            {
//                var sut = mock.Create<BingSearch>();
//                var privateObj = new PrivateObject(sut);
//                var subject = new List<string> { "someDomain.com" };
//                var result = (string)privateObj.Invoke("BuildSources",
//                    BindingFlags.Static | BindingFlags.NonPublic, subject);

//                result.Should().StartWith("(site:someDomain.com)");
//            }
//        }


//        [Fact]
//        public void BuildSources_EmptyLink_EmptyString()
//        {
//            using (var mock = AutoMock.GetLoose())
//            {
//                var sut = mock.Create<BingSearch>();
//                var privateObj = new PrivateObject(sut);
//                var subject = new List<string>();
//                var result = (string)privateObj.Invoke("BuildSources",
//                    BindingFlags.Static | BindingFlags.NonPublic, subject);

//                result.Should().BeEmpty();
//            }
//        }


//        [Fact]
//        public void BuildSources_SomeInput_RightInput()
//        {
//            using (var mock = AutoMock.GetLoose())
//            {
//                var sut = mock.Create<BingSearch>();
//                var privateObj = new PrivateObject(sut);
//                var subject = new List<string>() { "bbc.co.uk", "cnn.com" };
//                var result = (string)privateObj.Invoke("BuildSources",
//                    BindingFlags.Static | BindingFlags.NonPublic, subject);

//                result.Should().Be("(site:bbc.co.uk OR site:cnn.com)");
//            }
//        }


//    }
//}
