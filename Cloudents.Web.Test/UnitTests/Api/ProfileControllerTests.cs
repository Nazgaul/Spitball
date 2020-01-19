using Autofac.Extras.Moq;
using Cloudents.Core.DTOs;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Web.Api;
using Cloudents.Web.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.UnitTests.Api
{
    public class ProfileControllerTests
    {

        public interface IExtensionMethodsWrapper
        {
            string DocumentUrlWrapper<T>(IUrlHelper myObj, string course, long id, string name);
        }

        public class ExtensionMethodsWrapper : IExtensionMethodsWrapper
        {
            public string DocumentUrlWrapper<T>(IUrlHelper myObj,  string course, long id, string name)
            {
                return myObj.DocumentUrl( course, id, name);
            }
        }

        [Fact]
        public async Task GetDocumentsAsync_ReturnUrlAsync()
        {

            using (var mock = AutoMock.GetLoose())
            {
                var id = 159039L;
                var page = 0;
                var pageSize = 20;
                var cancellationToken = CancellationToken.None;
                var result = new ListWithCountDto<DocumentFeedDto>()
                {
                    Result = new[] {new DocumentFeedDto()
                    {
                        Id = 159039,
                        University = "SOME UNIVERSITY",
                        Course = "some course",
                        Title = "some name"
                    }},
                    Count = 1
                };


                // The AutoMock class will inject a mock IDependency
                // into the SystemUnderTest constructor




                var mockUrlHelper = new Mock<IUrlHelper>();


                mockUrlHelper.Setup(o => o.RouteUrl(It.IsAny<UrlRouteContext>())).Returns("a/mock/url/for/testing");

                // mockUrlHelper.SetupGet(o => o.ActionContext.HttpContext.RequestServices.GetRequiredService<IBinarySerializer>())
                //    .Returns(new BinarySerializer(, ));




                mock.Mock<IDocumentDirectoryBlobProvider>().Setup(o => o.GetPreviewImageLink(It.IsAny<long>(), It.IsAny<int>()))
                        .Returns(new Uri("https://spitball.co/test"));




                //mockUrlHelper.Setup(x => x.Action(
                //    It.IsAny<UrlActionContext>()
                //)).Returns("a/mock/url/for/testing").Verifiable();

                mock.Mock<IQueryBus>()
                    .Setup(s => s.QueryAsync(It.IsAny<UserDocumentsQuery>(), cancellationToken))
                    .ReturnsAsync(result);

                var sut = mock.Create<ProfileController>();
                sut.Url = mockUrlHelper.Object;
                sut.ControllerContext.HttpContext = new DefaultHttpContext();


                var retVal = await sut.GetDocumentsAsync(new Models.ProfileDocumentsRequest()
                {
                    Id = id, 
                    Page = page, 
                    PageSize = pageSize
                },
                    cancellationToken);
                mock.Mock<IQueryBus>().Verify(x => x.QueryAsync(It.IsAny<UserDocumentsQuery>(), cancellationToken));

                retVal.Result.First().Url.Should().NotBeNullOrEmpty();

            }
        }
    }
}
