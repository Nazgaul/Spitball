using Autofac.Extras.Moq;
using Cloudents.Core.DTOs;
using Cloudents.Infrastructure.Search.Book;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Infrastructure.Test
{
    [TestClass]
    public class BookDetailConverterTests
    {
        [TestMethod]
        public void ChangeUrlIfNeeded_ValorBooks_ChangeUrl_Ok()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var sut = mock.Create<BookDetailConverter>();
                var privateObj = new PrivateObject(sut);

                var result = (string) privateObj.Invoke("ChangeUrlIfNeeded",
                     "ValoreBooks.com", "9781118128169", BookCondition.None);
                result.Should().NotBeNullOrEmpty();
            }
        }

        [TestMethod]
        public void ChangeUrlIfNeeded_ValorRental_ChangeUrl_Ok()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var sut = mock.Create<BookDetailConverter>();
                var privateObj = new PrivateObject(sut);

                var result = (string)privateObj.Invoke("ChangeUrlIfNeeded",
                    "ValoreBooks Rental", "9781118128169", BookCondition.None);
                result.Should().NotBeNullOrEmpty();
            }
        }


        [TestMethod]
        public void ChangeUrlIfNeeded_CheggRental_ChangeUrl_Ok()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var sut = mock.Create<BookDetailConverter>();
                var privateObj = new PrivateObject(sut);

                var result = (string)privateObj.Invoke("ChangeUrlIfNeeded",
                    "Chegg New and Used", "9781118128169", BookCondition.Rental);
                result.Should().NotBeNullOrEmpty();
            }
        }

        [TestMethod]
        public void ChangeUrlIfNeeded_NoAffiliate_ChangeUrl_Ok()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var sut = mock.Create<BookDetailConverter>();
                var privateObj = new PrivateObject(sut);

                var result = (string)privateObj.Invoke("ChangeUrlIfNeeded",
                    "Ram", "9781118128169", BookCondition.None);
                result.Should().BeNullOrEmpty();
            }
        }

    }
}