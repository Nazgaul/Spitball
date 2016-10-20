using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MelissaData;

namespace Zbang.Zbox.Infrastructure
{
    public class LocationProvider : ILocationProvider
    {
        public async Task<IPCheckEntity> GetLocationDataAsync(string ipAddress)
        {
            var client = new IPCheckContainer(
                new Uri("https://api.datamarket.azure.com/Data.ashx/MelissaData/IPCheck/v1/")
                )
            { Credentials = new NetworkCredential("accountKey", "Pjc3UHKP+iG3WO1n8GFmtqDpUgJiIFSOM2ludra9pxc=") };


            var marketData = client.SuggestIPAddresses(
                ipAddress,
                1,
                0.7
                );
            var result = await Task.Factory.FromAsync(marketData.BeginExecute, marketData.EndExecute, null);
            return result?.FirstOrDefault();
        }
    }

    public interface ILocationProvider
    {
        Task<IPCheckEntity> GetLocationDataAsync(string ipAddress);
    }
}
