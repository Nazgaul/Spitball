using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Cloudents.Core.Extension;
using FluentAssertions;
using Xunit;

namespace Cloudents.Core.Test
{
    public class UriBuilderExtensionsTests
    {
        [Fact]
        public void AddQuery_WithQuery_QueryRemains()
        {
            var builder = new UriBuilder(new Uri("http://www.someUri.co?xxx=yyy"));
            builder.AddQuery(new
            {
                z = "p"
            });

            builder.ToString().Should().BeEquivalentTo("http://www.someUri.co:80/?xxx=yyy&z=p");
        }

        [Fact]
        public void AddQuery_BaseUrl_Ok()
        {
            var builder = new UriBuilder(new Uri("http://www.someUri.co"));

            builder.AddQuery(new
            {
                z = "p"
            });

            builder.ToString().Should().BeEquivalentTo("http://www.someUri.co:80/?z=p");
        }

        [Fact]
        public void AddQuery_WithQueryNVC_QueryRemains()
        {
            var nvc = new NameValueCollection()
            {
                ["z"] = "p"
            };
            var builder = new UriBuilder(new Uri("http://www.someUri.co?xxx=yyy"));
            builder.AddQuery(nvc);

            builder.ToString().Should().BeEquivalentTo("http://www.someUri.co:80/?xxx=yyy&z=p");
        }

        [Fact]
        public void AddQuery_BaseUrlNVC_Ok()
        {
            var builder = new UriBuilder(new Uri("http://www.someUri.co"));
            var nvc = new NameValueCollection()
            {
                ["z"] = "p"
            };
            builder.AddQuery(nvc);

            builder.ToString().Should().BeEquivalentTo("http://www.someUri.co:80/?z=p");
        }

    }
}
