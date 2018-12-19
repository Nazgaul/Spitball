using Autofac.Extras.Moq;
using Cloudents.Application.DTOs;
using Cloudents.Core.Test;
using Cloudents.Infrastructure.Search.Book;
using FluentAssertions;
using Xunit;

namespace Cloudents.Infrastructure.Test
{
    public class BookDetailConverterTests
    {
        [Fact]
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

        [Fact]
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


        [Fact]
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

        [Fact]
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