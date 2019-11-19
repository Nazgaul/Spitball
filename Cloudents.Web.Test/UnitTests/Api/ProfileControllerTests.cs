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
using System.Collections.Generic;
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
            string DocumentUrlWrapper<T>(IUrlHelper myObj, string university, string course, long id, string name);
        }

        public class ExtensionMethodsWrapper : IExtensionMethodsWrapper
        {
            public string DocumentUrlWrapper<T>(IUrlHelper myObj, string university, string course, long id, string name)
            {
                return myObj.DocumentUrl(university, course, id, name);
            }
        }

        [Fact]
        public async Task GetDocumentsAsync_ReturnUrl()
        {

            using (var mock = AutoMock.GetLoose())
            {
                var id = 159039L;
                var page = 0;
                var cancellationToken = CancellationToken.None;
                var result = new[] {new DocumentFeedDto()
                {
                    Id = 159039,
                    University = "SOME UNIVERSITY",
                    Course = "some course",
                    Title = "some name"
                }};


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
                    .Setup(s => s.QueryAsync<IEnumerable<DocumentFeedDto>>(It.IsAny<UserDataPagingByIdQuery>(), cancellationToken))
                    .ReturnsAsync(result);

                var sut = mock.Create<ProfileController>();
                sut.Url = mockUrlHelper.Object;
                sut.ControllerContext.HttpContext = new DefaultHttpContext();


                var retVal = await sut.GetDocumentsAsync(id, page, cancellationToken);
                mock.Mock<IQueryBus>().Verify(x => x.QueryAsync<IEnumerable<DocumentFeedDto>>(It.IsAny<UserDataPagingByIdQuery>(), cancellationToken));

                retVal.First().Url.Should().NotBeNullOrEmpty();

            }
        }
    }
}
