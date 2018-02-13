using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Search.Tutor;
using Cloudents.Infrastructure.Write.Tutor;
using Microsoft.Azure.Search;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cloudents.Infrastructure.Test
{
    [TestClass]
    public class TutorAzureSearchTests
    {
        private readonly Mock<ISearchServiceClient> _searchIndexMock = new Mock<ISearchServiceClient>();
        private readonly Mock<IMapper> _searchMapper = new Mock<IMapper>();


        [TestInitialize]
        public void Setup()
        {
            _searchIndexMock.Setup(s => s.Indexes.GetClient(TutorSearchWrite.IndexName));
        }

        [TestMethod]
        public void ApplyFilter_filterNone_NoFilter()
        {
            var tutorSearch = new TutorAzureSearch(_searchIndexMock.Object, _searchMapper.Object);
            var obj = new PrivateObject(tutorSearch);
            var argument = new List<TutorRequestFilter> { TutorRequestFilter.None };
            var location = new GeoPoint(0, 0);
            var result = obj.Invoke("ApplyFilter", BindingFlags.Static | BindingFlags.NonPublic, argument, location);
            var resultCast = result as List<string>;

            CollectionAssert.AreEqual(resultCast, new List<string>());
        }
    }
}
