using System.Linq;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure
{
    public class CountryProvider : ICountryProvider
    {
        private static readonly Nager.Country.ICountryProvider Item = new Nager.Country.CountryProvider();
        public string GetCallingCode(string countryCode)
        {
            
            var country = Item.GetCountry(countryCode);
            return country.CallingCodes.First();
        }

        public bool ValidateCountryCode(string countryCode)
        {
            var country = Item.GetCountry(countryCode);
            return country != null;
        }

        public (string iso2Code, string country,string region ,string subregion)? GetCountryParams(string countryCode)
        {
            var country = Item.GetCountry(countryCode);
            if (country == null)
            {
                return null;
            }

            return (country.Alpha2Code.ToString("G"), country.OfficialName, country.Region.ToString("G"), country.SubRegion.ToString("G"));
        }

        //public decimal ConvertPointsToLocalCurrency(string countryCode, decimal points)
        //{
        //    var country = Item.GetCountry(countryCode);

        //    if (country.Alpha2Code == Alpha2Code.IN)
        //    {
        //        return points;
        //    }

        //    return points / 25;
        //}

        //public string ConvertPointsToLocalCurrencyWithSymbol(string countryCode, decimal points)
        //{
        //    var country = Item.GetCountry(countryCode);
        //    if (country.Alpha2Code == Alpha2Code.IN)
        //    {
        //        var culture = CultureInfo.CurrentCulture.ChangeCultureBaseOnCountry(Alpha2Code.IN.ToString());
        //        return points.ToString("C", culture);
        //    }
        //    var culture2 = CultureInfo.CurrentCulture.ChangeCultureBaseOnCountry(Alpha2Code.IL.ToString());
        //    return (points / 25).ToString("C", culture2);
        //}

        //public string ConvertToLocalCurrencyWithSymbol(string countryCode, decimal price)
        //{
        //    var country = Item.GetCountry(countryCode);
        //    var culture = CultureInfo.CurrentCulture.ChangeCultureBaseOnCountry(country.Alpha2Code.ToString());
        //    return price.ToString("C", culture);
        //}
    }
}