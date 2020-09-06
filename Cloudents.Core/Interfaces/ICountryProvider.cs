namespace Cloudents.Core.Interfaces
{
    public interface ICountryProvider
    {
        string GetCallingCode(string countryCode);
        bool ValidateCountryCode(string countryCode);

        (string iso2Code, string country,string region ,string subregion)? GetCountryParams(string countryCode);
    }
}