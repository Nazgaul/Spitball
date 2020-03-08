using Cloudents.Core.Models;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IIpToLocation
    {
        Task<Location?> GetAsync(IPAddress ipAddress, CancellationToken token);
    }

    public interface ICountryProvider
    {
        string GetCallingCode(string countryCode);
        bool ValidateCountryCode(string countryCode);

        //decimal ConvertPointsToLocalCurrency(string countryCode, decimal points);
        //string ConvertPointsToLocalCurrencyWithSymbol(string countryCode, decimal points);
        // string ConvertToLocalCurrencyWithSymbol(string countryCode, decimal price);
    }
}
