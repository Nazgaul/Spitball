using System;
using System.Threading;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Cache;
using Cloudents.Infrastructure.Search;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Infrastructure.Test
{
    [TestClass]
    public class CacheResultInterceptorTests
    {
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

        [TestMethod]
        public void GetInvocationSignature_BingDifferentTerm_Works()
        {
            var searchModel1 = new SearchModel(new[] {"biology"}, null, 0, SearchRequestSort.None,
                CustomApiKey.Documents, null, null, "biology", null);
            var searchModel2 = new SearchModel(new[] { "chemistry" }, null, 0, SearchRequestSort.None,
                CustomApiKey.Documents, null, null, "biology", null);
            //IEnumerable<string> term, int imageWidth, int page, CancellationToken token
            var bookRequest1 = new object[] { searchModel1, CancellationToken.None };
            var bookRequest2 = new object[] { searchModel2, CancellationToken.None };
            var result1 = CacheResultInterceptor.BuildArgument(bookRequest1);
            var result2 = CacheResultInterceptor.BuildArgument(bookRequest2);

            Assert.AreNotEqual(result1, result2);
        }

        [TestMethod]
        public void GetInvocationSignature_DifferentArrayOrder_SameResultWorks()
        {
            var searchModel1 = new SearchModel(new [] { "Linear Algebra" }, new [] { "spitball", "koofers" }, 0, SearchRequestSort.None,
                CustomApiKey.Documents, null, null, "biology", null);
            var searchModel2 = new SearchModel(new[] { "Linear Algebra" }, new[] { "koofers","spitball", }, 0, SearchRequestSort.None,
                CustomApiKey.Documents, null, null, "biology", null);
            //IEnumerable<string> term, int imageWidth, int page, CancellationToken token
            var bookRequest1 = new object[] { searchModel1, CancellationToken.None };
            var bookRequest2 = new object[] { searchModel2, CancellationToken.None };
            var result1 = CacheResultInterceptor.BuildArgument(bookRequest1);
            var result2 = CacheResultInterceptor.BuildArgument(bookRequest2);

            Assert.AreEqual(result1, result2);
        }

        [TestMethod]
        public void GetInvocationSignature_BingDifferentKey_Works()
        {
            var searchModel1 = new SearchModel(new[] { "biology" }, null, 0, SearchRequestSort.None,
                CustomApiKey.Documents, null, null, "biology", null);
            var searchModel2 = new SearchModel(new[] { "biology" }, null, 0, SearchRequestSort.None,
                CustomApiKey.Flashcard, null, null, "biology", null);
            //IEnumerable<string> term, int imageWidth, int page, CancellationToken token
            var bookRequest1 = new object[] { searchModel1, CancellationToken.None };
            var bookRequest2 = new object[] { searchModel2, CancellationToken.None };
            var result1 = CacheResultInterceptor.BuildArgument(bookRequest1);
            var result2 = CacheResultInterceptor.BuildArgument(bookRequest2);

            Assert.AreNotEqual(result1, result2);
        }
    }
}
