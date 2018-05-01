using System.Collections.Generic;
using System.Reflection;
using Autofac.Extras.Moq;
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


        //[TestInitialize]
        public void Setup()
        {
            _searchIndexMock.Setup(s => s.Indexes.GetClient(TutorSearchWrite.IndexName));
        }


        //TODO: build failed because get client change to extension method. need to fix that.
        [TestMethod]
        public void ApplyFilter_filterNone_NoFilter()
        {
            using (var mock = AutoMock.GetLoose())
            {
                //ISearchIndexClient mockSearchIndexClient = null;
                //mock.Mock<ISearchServiceClient>().Setup(s => s.Indexes..GetClient(TutorSearchWrite.IndexName)).Returns(mockSearchIndexClient);
                var tutorSearch = mock.Create<TutorAzureSearch>();// new TutorAzureSearch(_searchIndexMock.Object, _searchMapper.Object);
                var obj = new PrivateObject(tutorSearch);
                var argument = new List<TutorRequestFilter> { TutorRequestFilter.None };
                var location = new GeoPoint(0, 0);
                var result = obj.Invoke("ApplyFilter", BindingFlags.Static | BindingFlags.NonPublic, argument, location);
                var resultCast = result as List<string>;

                CollectionAssert.AreEqual(resultCast, new List<string>());
            }

            //var tutorSearch = new TutorAzureSearch(_searchIndexMock.Object, _searchMapper.Object);
            //var obj = new PrivateObject(tutorSearch);
            //var argument = new List<TutorRequestFilter> { TutorRequestFilter.None };
            //var location = new GeoPoint(0, 0);
            //var result = obj.Invoke("ApplyFilter", BindingFlags.Static | BindingFlags.NonPublic, argument, location);
            //var resultCast = result as List<string>;

            //CollectionAssert.AreEqual(resultCast, new List<string>());
        }
    }
}
