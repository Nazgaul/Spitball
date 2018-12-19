using System.Collections.Generic;
using Autofac;
using Autofac.Extras.Moq;
using Cloudents.Application.Query;
using FluentAssertions;
using Xunit;

namespace Cloudents.Core.Test
{
    public class WebSearchTests
    {
        [Fact]
        public void BuildSources_ExistingSource_RightDomain()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var parameter = new NamedParameter("api", CustomApiKey.Documents);
                var sut = mock.Create<WebSearch>(parameter);
                var privateObj = new PrivateObject(sut);

                var argument = new List<string> {"oneclass"};
                var result = (IEnumerable<string>)privateObj.Invoke("BuildSources", argument);

                result.Should().BeEquivalentTo(new[] { "oneclass.com/note" });
            }
        }
    }
}