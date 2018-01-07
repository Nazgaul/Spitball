using System;
using System.Threading;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Cache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cloudents.Infrastructure.Test
{
    [TestClass]
    public class CacheResultInterceptorTests
    {
        readonly Mock<ICacheProvider> _mock = new Mock<ICacheProvider>();
        [TestMethod]
        public void GetInvocationSignature_BookDetailPaging_Works()
        {
            //IEnumerable<string> term, int imageWidth, int page, CancellationToken token
            var bookRequest1 = new object[] { "make%20war", 150, 0, CancellationToken.None };
            var bookRequest2 = new object[] { "make%20war", 150, 1, CancellationToken.None };
            var result1 = CacheResultInterceptor.BuildArgument(bookRequest1);
            var result2 = CacheResultInterceptor.BuildArgument(bookRequest2);

            Assert.AreNotEqual(result1, result2);
        }
    }
}
