using System.Collections.Generic;
using Autofac;
using Autofac.Extras.Moq;
using Cloudents.Core.Read;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Core.Test
{
    [TestClass]
    public class WebSearchTests
    {
        [TestMethod]
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