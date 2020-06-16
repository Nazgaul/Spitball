using Cloudents.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IIpToLocation
    {
        Task<Location?> GetLocationAsync(string ipAddress, CancellationToken token);
    }

    public interface ICountryProvider
    {
        string GetCallingCode(string countryCode);
        bool ValidateCountryCode(string countryCode);

        //(string iso2Code, string country,string region ,string subregion)? GetCountryParams(string countryCode);
    }
}
