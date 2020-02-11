using FluentAssertions;
using Xunit;

namespace Cloudents.Infrastructure.Test
{
    public class CountryProviderTests
    {
        private readonly CountryProvider _countryProvider = new CountryProvider();

        [Theory]
        [InlineData("string")]
        [InlineData("xx")]
        public void ValidateCountry_NotValid_False(string country)
        {
            var result = _countryProvider.ValidateCountryCode(country);
            result.Should().BeFalse();
        }


        [Theory]
        [InlineData("IL")]
        [InlineData("il")]
        [InlineData("IN")]
        [InlineData("in")]
        [InlineData("US")]
        [InlineData("Us")]
        public void ValidateCountry_Valid_Ok(string country)
        {
            var result = _countryProvider.ValidateCountryCode(country);
            result.Should().BeTrue();
        }
    }
}