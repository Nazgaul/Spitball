using System.Reflection;
using System.Threading;
using Cloudents.Core.Read;
using Cloudents.Infrastructure.Interceptor;
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
            var type = new PrivateType(typeof(CacheResultInterceptor));
            var result1 = type.InvokeStatic("BuildArgument", BindingFlags.Static | BindingFlags.NonPublic, new object[] { bookRequest1});
            var result2 = type.InvokeStatic("BuildArgument", BindingFlags.Static | BindingFlags.NonPublic, new object[] { bookRequest2 });

            Assert.AreNotEqual(result1, result2);
        }

        [TestMethod]
        public void GetInvocationSignature_BingDifferentTerm_Works()
        {
            var searchModel1 = new SearchModel(new[] { "biology" }, null, 
                CustomApiKey.Documents, null, null,  null);
            var searchModel2 = new SearchModel(new[] { "chemistry" }, null, 
                CustomApiKey.Documents, null, null,  null);
            //IEnumerable<string> term, int imageWidth, int page, CancellationToken token
            var bookRequest1 = new object[] { searchModel1, 0, CancellationToken.None };
            var bookRequest2 = new object[] { searchModel2, 0, CancellationToken.None };
            var type = new PrivateType(typeof(CacheResultInterceptor));
            var result1 = type.InvokeStatic("BuildArgument", BindingFlags.Static | BindingFlags.NonPublic, new object[] { bookRequest1 });
            var result2 = type.InvokeStatic("BuildArgument", BindingFlags.Static | BindingFlags.NonPublic, new object[] { bookRequest2 });

            Assert.AreNotEqual(result1, result2);
        }

        [TestMethod]
        public void GetInvocationSignature_DifferentArrayOrder_SameResultWorks()
        {
            var searchModel1 = new SearchModel(new[] { "Linear Algebra" }, new[] { "spitball", "koofers" }, 
                CustomApiKey.Documents, null, null,  null);
            var searchModel2 = new SearchModel(new[] { "Linear Algebra" }, new[] { "koofers", "spitball", }, 
                CustomApiKey.Documents, null, null,  null);
            //IEnumerable<string> term, int imageWidth, int page, CancellationToken token
            var bookRequest1 = new object[] { searchModel1, 0, CancellationToken.None };
            var bookRequest2 = new object[] { searchModel2, 0, CancellationToken.None };
            var type = new PrivateType(typeof(CacheResultInterceptor));
            var result1 = type.InvokeStatic("BuildArgument", BindingFlags.Static | BindingFlags.NonPublic, new object[] { bookRequest1 });
            var result2 = type.InvokeStatic("BuildArgument", BindingFlags.Static | BindingFlags.NonPublic, new object[] { bookRequest2 });

            Assert.AreEqual(result1, result2);
        }

        [TestMethod]
        public void GetInvocationSignature_BingDifferentKey_Works()
        {
            var searchModel1 = new SearchModel(new[] { "biology" }, null,
                CustomApiKey.Documents, null, null,  null);
            var searchModel2 = new SearchModel(new[] { "biology" }, null, 
                CustomApiKey.Flashcard, null, null,  null);
            var bookRequest1 = new object[] { searchModel1, 0, CancellationToken.None };
            var bookRequest2 = new object[] { searchModel2, 0, CancellationToken.None };

            var type = new PrivateType(typeof(CacheResultInterceptor));
            var result1 = type.InvokeStatic("BuildArgument", BindingFlags.Static | BindingFlags.NonPublic, new object[] { bookRequest1 });
            var result2 = type.InvokeStatic("BuildArgument", BindingFlags.Static | BindingFlags.NonPublic, new object[] { bookRequest2 });


            Assert.AreNotEqual(result1, result2);
        }
    }
}
