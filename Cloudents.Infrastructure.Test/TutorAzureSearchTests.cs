namespace Cloudents.Infrastructure.Test
{
    public class TutorAzureSearchTests
    {
        //private readonly Mock<IMapper> _searchMapper = new Mock<IMapper>();


        //[TestInitialize]
        public void Setup()
        {
        }


        //TODO: build failed because get client change to extension method. need to fix that.
        //[Fact]
        //public void ApplyFilter_filterNone_NoFilter()
        //{
        //    using (var mock = AutoMock.GetLoose())
        //    {
        //        //ISearchIndexClient mockSearchIndexClient = null;
        //        //mock.Mock<ISearchServiceClient>().Setup(s => s.Indexes..GetClient(TutorSearchWrite.IndexName)).Returns(mockSearchIndexClient);
        //        var tutorSearch = mock.Create<TutorAzureSearch>();// new TutorAzureSearch(_searchIndexMock.Object, _searchMapper.Object);
        //        var obj = new PrivateObject(tutorSearch);
        //        var argument = new List<TutorRequestFilter> { TutorRequestFilter.InPerson };
        //        var location = new GeoPoint(0, 0);
        //        var result = obj.Invoke("ApplyFilter", BindingFlags.Static | BindingFlags.NonPublic, argument, location);
        //        var resultCast = result as List<string>;

        //        resultCast.Should().BeEmpty();
        //    }

          
        //}
    }
}
