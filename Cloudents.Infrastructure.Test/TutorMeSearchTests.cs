using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Search.Tutor;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Infrastructure.Test
{
    [TestClass]
    public class TutorMeSearchTests
    {
        private const string Term = "Some term";
        private const int Page = 0;

        [TestMethod]
        public async Task SearchAsync_InPersonFilter_NullAsync()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var tutuMeSearch = mock.Create<TutorMeSearch>();

                var result = await tutuMeSearch.SearchAsync("Some term", new[] { TutorRequestFilter.InPerson }, TutorRequestSort.Relevance,
                    new GeoPoint(0, 0), 0, false, default).ConfigureAwait(false);

                result.Should().BeNull();
            }
        }

        [TestMethod]
        public async Task SearchAsync_SeveralFilter_ResultAsync()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var tutuMeSearch = Init(mock);

                var result = await tutuMeSearch.SearchAsync("Some term", new[] { TutorRequestFilter.InPerson, TutorRequestFilter.Online }, TutorRequestSort.Relevance,
                    new GeoPoint(0, 0), 0, false, default).ConfigureAwait(false);

                result.Should().NotBeNull();
            }
        }

        [TestMethod]
        public async Task SearchAsync_NoFilter_ResultAsync()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var tutuMeSearch = Init(mock);
                var result = await tutuMeSearch.SearchAsync(Term, Array.Empty<TutorRequestFilter>(), TutorRequestSort.Relevance,
                    new GeoPoint(0, 0), Page, false, default).ConfigureAwait(false);
                result.Should().NotBeNull();
            }
        }

        private static TutorMeSearch Init(AutoMock mock)
        {
            var tutuMeSearch = mock.Create<TutorMeSearch>();

            var privateObject = new PrivateObject(tutuMeSearch);

            var nvc =
                privateObject.Invoke("BuildQueryString", BindingFlags.NonPublic | BindingFlags.Static, Term, Page) as
                    NameValueCollection;

            var url = privateObject.GetFieldOrProperty("UrlEndpoint", BindingFlags.NonPublic | BindingFlags.Static) as string;
            mock.Mock<IRestClient>().Setup(x =>
                x.GetAsync<TutorMeSearch.TutorMeResult>(new Uri(url), nvc, default)).Returns(Task.FromResult(
                new TutorMeSearch.TutorMeResult
                {
                    Group = new TutorMeSearch.Group(),
                    Results = Array.Empty<TutorMeSearch.Result>()
                }));
            return tutuMeSearch;
        }
    }
}