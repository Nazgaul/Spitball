//using System.Reflection;
//using System.Threading;
//using Cloudents.Core.Enum;
//using Cloudents.Core.Models;
//using Cloudents.Core.Test;
//using Cloudents.Infrastructure.Interceptor;
//using Cloudents.Infrastructure.Search.Job;
//using Xunit;

//namespace Cloudents.Infrastructure.Test
//{
//    public class CacheResultInterceptorTests
//    {
//        //[Fact]
//        //public void GetInvocationSignature_BookDetailPaging_Works()
//        //{
//        //    //IEnumerable<string> term, int imageWidth, int page, CancellationToken token
//        //    var bookRequest1 = new object[] { "make%20war", 150, 0, CancellationToken.None };
//        //    var bookRequest2 = new object[] { "make%20war", 150, 1, CancellationToken.None };
//        //    var type = new PrivateType(typeof(CacheResultInterceptor));
//        //    var result1 = type.InvokeStatic("BuildArgument", BindingFlags.Static | BindingFlags.NonPublic, new object[] { bookRequest1 });
//        //    var result2 = type.InvokeStatic("BuildArgument", BindingFlags.Static | BindingFlags.NonPublic, new object[] { bookRequest2 });

//        //    Assert.NotEqual(result1, result2);
//        //}

//        //[Fact]
//        //public void GetInvocationSignature_BingDifferentTerm_Works()
//        //{
//        //    var searchModel1 = new SearchModel("biology", null,
//        //        CustomApiKey.Documents, null, null);
//        //    var searchModel2 = new SearchModel("chemistry", null,
//        //        CustomApiKey.Documents, null, null);
//        //    //IEnumerable<string> term, int imageWidth, int page, CancellationToken token
//        //    var bookRequest1 = new object[] { searchModel1, 0, CancellationToken.None };
//        //    var bookRequest2 = new object[] { searchModel2, 0, CancellationToken.None };
//        //    var type = new PrivateType(typeof(CacheResultInterceptor));
//        //    var result1 = type.InvokeStatic("BuildArgument", BindingFlags.Static | BindingFlags.NonPublic, new object[] { bookRequest1 });
//        //    var result2 = type.InvokeStatic("BuildArgument", BindingFlags.Static | BindingFlags.NonPublic, new object[] { bookRequest2 });

//        //    Assert.NotEqual(result1, result2);
//        //}

//        //[Fact]
//        //public void GetInvocationSignature_DifferentArrayOrder_SameResultWorks()
//        //{
//        //    var searchModel1 = new SearchModel("Linear Algebra", new[] { "spitball", "koofers" },
//        //        CustomApiKey.Documents, null, null);
//        //    var searchModel2 = new SearchModel("Linear Algebra", new[] { "koofers", "spitball", },
//        //        CustomApiKey.Documents, null, null);
//        //    //IEnumerable<string> term, int imageWidth, int page, CancellationToken token
//        //    var bookRequest1 = new object[] { searchModel1, 0, CancellationToken.None };
//        //    var bookRequest2 = new object[] { searchModel2, 0, CancellationToken.None };
//        //    var type = new PrivateType(typeof(CacheResultInterceptor));
//        //    var result1 = type.InvokeStatic("BuildArgument", BindingFlags.Static | BindingFlags.NonPublic, new object[] { bookRequest1 });
//        //    var result2 = type.InvokeStatic("BuildArgument", BindingFlags.Static | BindingFlags.NonPublic, new object[] { bookRequest2 });



//        //    result1.Should().Be(result2);
//        //    //Assert.NotEqual(result1, result2);
//        //}

//        //[Fact]
//        //public void GetInvocationSignature_BingDifferentKey_Works()
//        //{
//        //    var searchModel1 = new SearchModel ("biology" , null,
//        //        CustomApiKey.Documents, null, null);
//        //    var searchModel2 = new SearchModel("biology", null,
//        //        CustomApiKey.Flashcard, null, null);
//        //    var bookRequest1 = new object[] { searchModel1, 0, CancellationToken.None };
//        //    var bookRequest2 = new object[] { searchModel2, 0, CancellationToken.None };

//        //    var type = new PrivateType(typeof(CacheResultInterceptor));
//        //    var result1 = type.InvokeStatic("BuildArgument", BindingFlags.Static | BindingFlags.NonPublic, new object[] { bookRequest1 });
//        //    var result2 = type.InvokeStatic("BuildArgument", BindingFlags.Static | BindingFlags.NonPublic, new object[] { bookRequest2 });
//        //    Assert.NotEqual(result1, result2);
//        //}

//        //[Fact]
//        //public void GetInvocationSignature_DifferentLocation_DifferentResult()
//        //{
//        //    var location = new Location(new GeoPoint(41.878f, -87.629f), new Address("Chicago", "IL", "US"), "31.154.39.170", "972");
//        //    var location2 = new Location(new GeoPoint(42.878f, -86.629f), new Address("Chicago", "IL", "US"), "31.154.39.170", "972");
//        //    var model1 = new JobProviderRequest("marketing", JobRequestSort.Relevance, null, location, 0);
//        //    var model2 = new JobProviderRequest("marketing", JobRequestSort.Relevance, null, location2, 0);
//        //    //IEnumerable<string> term, int imageWidth, int page, CancellationToken token
//        //    var bookRequest1 = new object[] { model1, CancellationToken.None };
//        //    var bookRequest2 = new object[] { model2, CancellationToken.None };

//        //    var type = new PrivateType(typeof(CacheResultInterceptor));
//        //    var result1 = type.InvokeStatic("BuildArgument", BindingFlags.Static | BindingFlags.NonPublic, new object[] { bookRequest1 });
//        //    var result2 = type.InvokeStatic("BuildArgument", BindingFlags.Static | BindingFlags.NonPublic, new object[] { bookRequest2 });

//        //    Assert.NotEqual(result1, result2);
//        //}
//    }
//}
