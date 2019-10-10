using System;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Cloudents.Admin2.Api;
using Cloudents.Admin2.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Cloudents.Admin.Test.UnitTests.Api
{

    public class ShotUrlControllerTest 
    {
        [Theory]
        [InlineData("xxx","xxx", null)]
        //[InlineData("www.google.com", "zxc", null)]
       public async Task Post_Create_Url(string destination, string identifier, DateTime? date)
        {

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IConfiguration>().Setup(x => x["Site"]).Returns("www.hadarTest");
                var sut = mock.Create<AdminShortUrlController>();
                var result = await sut.AddShortUrlAsync(new AddShortUrlRequest { Destination = destination,
                                                                                    Identifier  = identifier,
                                                                                    Expiration = date
                                                                                }, default);
                result.Value.Destination.Should().Be("/xxx");
            }
        }
     
    }
}
