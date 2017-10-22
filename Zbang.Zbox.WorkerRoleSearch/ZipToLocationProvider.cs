using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Spatial;
using Microsoft.WindowsAzure.ServiceRuntime;
using Newtonsoft.Json.Linq;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class ZipToLocationProvider : IZipToLocationProvider
    {
        //private readonly Dictionary<string, GeographyPoint> m_ZipToLocationCache = new Dictionary<string, GeographyPoint>();

        private readonly ICache m_Cache;
        private readonly ILogger m_Logger;

        public ZipToLocationProvider(ILogger logger, ICache cache)
        {
            m_Logger = logger;
            m_Cache = cache;
        }

        public async Task<GeographyPoint> GetLocationViaZipAsync(string zip)
        {
            if (string.IsNullOrEmpty(zip))
            {
                return null;
            }
            var point = await m_Cache.GetFromCacheAsync<GeographyPoint>(CacheRegions.ZipToLocation, zip).ConfigureAwait(false);
            if (point != null)
            {
                return point;
            }
            using (var client = new HttpClient())
            {
                try
                {
                    var str = await client.GetStringAsync(
                        "https://maps.googleapis.com/maps/api/geocode/json?key=AIzaSyC7lF3qKA4N8Ej6ycTEIB08h8rWUlKqPKY&components=postal_code:" +
                        zip).ConfigureAwait(false);
                    var jsonObject = JObject.Parse(str);
                    if (jsonObject["status"].Value<string>() != "OK")
                    {
                        return null;
                    }
                    var location = jsonObject["results"].ToArray()[0]["geometry"]["location"];
                    var lat = location["lat"].Value<double>();
                    var lng = location["lng"].Value<double>();
                    point = GeographyPoint.Create(lat, lng);
                    await m_Cache.AddToCacheAsync(CacheRegions.ZipToLocation, zip, point, TimeSpan.FromDays(365)).ConfigureAwait(false);
                    return point;
                }
                catch (Exception ex)
                {
                    m_Logger.Exception(ex, new Dictionary<string, string>
                    {
                        ["service"] = "Jobs",
                        ["zip"] = zip
                    });
                    return null;
                }
            }
        }
    }

    public interface IZipToLocationProvider
    {
        Task<GeographyPoint> GetLocationViaZipAsync(string zip);
    }
}
