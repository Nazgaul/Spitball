using System.Reflection;
using Autofac.Extras.Moq;
using Cloudents.Infrastructure.Search.Book;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Infrastructure.Test
{
    [TestClass]
    public class BookSearchTests
    {
        [TestInitialize]
        public void Setup()
        {
            
        }

        [TestMethod]
        public void ValidateSearchResult_NullInput_ReturnFalse()
        {
            using (var mock = AutoMock.GetLoose())
            {
                //mock.Mock<IRestClient>().Setup(x=>x.GetAsync())
                // The AutoMock class will inject a mock IDependency
                // into the SystemUnderTest constructor
                var sut = mock.Create<BookSearch>();
                var privateObj = new PrivateObject(sut);
                BookSearch.BookDetailResult input = null;
                var result = (bool)privateObj.Invoke("ValidateSearchResult", BindingFlags.Static | BindingFlags.NonPublic, 0, input);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public void ValidateSearchResult_InputWithNullBook_ReturnFalse()
        {
            using (var mock = AutoMock.GetLoose())
            {
                //mock.Mock<IRestClient>().Setup(x=>x.GetAsync())
                // The AutoMock class will inject a mock IDependency
                // into the SystemUnderTest constructor
                var sut = mock.Create<BookSearch>();
                var privateObj = new PrivateObject(sut);
                var input = new BookSearch.BookDetailResult
                {
                    Response = new BookSearch.Response
                    {
                        Page = new BookSearch.Page
                        {
                            Books = null
                        }
                    }
                };
                var result = (bool)privateObj.Invoke("ValidateSearchResult", BindingFlags.Static | BindingFlags.NonPublic, 0, input);
                Assert.IsFalse(result);
            }
        }
    }
}