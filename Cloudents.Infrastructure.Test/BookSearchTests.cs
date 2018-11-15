using System.Reflection;
using Autofac.Extras.Moq;
using Cloudents.Core.Test;
using Cloudents.Infrastructure.Search.Book;
using Xunit;

namespace Cloudents.Infrastructure.Test
{
    public class BookSearchTests
    {
      

        [Fact]
        public void ValidateSearchResult_NullInput_ReturnFalse()
        {
            using (var mock = AutoMock.GetLoose())
            {
                //mock.Mock<IRestClient>().Setup(x=>x.GetAsync())
                // The AutoMock class will inject a mock IDependency
                // into the SystemUnderTest constructor
                var sut = mock.Create<BookSearch>();
                var privateObj = new PrivateObject(sut);
                var result = (bool)privateObj.Invoke("ValidateSearchResult", BindingFlags.Static | BindingFlags.NonPublic, 0, null);
                Assert.False(result);
            }
        }

        [Fact]
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
                Assert.False(result);
            }
        }
    }
}