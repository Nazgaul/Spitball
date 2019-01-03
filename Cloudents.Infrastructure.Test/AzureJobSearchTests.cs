//using System.Collections.Generic;
//using System.Reflection;
//using Autofac.Extras.Moq;
//using Cloudents.Core.Enum;
//using Cloudents.Core.Test;
//using Cloudents.Infrastructure.Search.Job;
//using FluentAssertions;
//using Xunit;

//namespace Cloudents.Infrastructure.Test
//{
//    public class AzureJobSearchTests
//    {
//        [Fact]
//        public void BuildFilter_EmptyEnumerableFilter_ReturnEmpty()
//        {
//            using (var mock = AutoMock.GetLoose())
//            {
//                //ISearchIndexClient mockSearchIndexClient = null;
                
//                //mock.Mock<IIndexesOperations>().Setup(x => x.GetClient(JobSearchWrite.IndexName)).Returns(mockSearchIndexClient);
//                var sut = mock.Create<AzureJobSearch>();
//                var privateObj = new PrivateObject(sut);
//                var list = new List<JobFilter>();
//                var result = (IList<string>)privateObj.Invoke("BuildFilter",
//                    BindingFlags.Static | BindingFlags.NonPublic, list, null);

//                result.Should().BeEmpty();
//            }
//        }
//    }
//}