﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MelissaData;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure
{
    public class LocationProvider : ILocationProvider
    {
        private readonly ILogger m_Logger;

        public LocationProvider(ILogger logger)
        {
            m_Logger = logger;
        }

        public async Task<IPCheckEntity> GetLocationDataAsync(string ipAddress)
        {
            var client = new IPCheckContainer(
                new Uri("https://api.datamarket.azure.com/Data.ashx/MelissaData/IPCheck/v1/")
                )
            { Credentials = new NetworkCredential("accountKey", "Pjc3UHKP+iG3WO1n8GFmtqDpUgJiIFSOM2ludra9pxc=") };

            try
            {
                var marketData = client.SuggestIPAddresses(
                    ipAddress,
                    1,
                    0.7
                );
                var result = await Task.Factory.FromAsync(marketData.BeginExecute, marketData.EndExecute, null).ConfigureAwait(false);
                return result?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                m_Logger.Exception(ex, new Dictionary<string,string> {["service"] = "Location"});
                return null;
            }
        }
    }

    public interface ILocationProvider
    {
        Task<IPCheckEntity> GetLocationDataAsync(string ipAddress);
    }
}
