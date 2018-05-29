using System;
using System.Linq;
using AutoMapper;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Search.Places;

namespace Cloudents.Infrastructure.Converters
{
    internal class GoogleGeoConverter : ITypeConverter<GoogleGeoCodeDto, (Address address, GeoPoint point)>
    {
        public (Address address, GeoPoint point) Convert(GoogleGeoCodeDto source, (Address address, GeoPoint point) destination, ResolutionContext context)
        {
            if (!string.Equals(source.Status, "ok", StringComparison.OrdinalIgnoreCase))
                return (null, null);
            var result = source.Results[0];

            var point = new GeoPoint(result.Geometry.Location.Lng, result.Geometry.Location.Lat);

            var city = result.AddressComponents
                ?.FirstOrDefault(w => w.Types.Contains("locality", StringComparer.InvariantCultureIgnoreCase))
                ?.ShortName;
            var regionCode = result.AddressComponents?.FirstOrDefault(w =>
                    w.Types.Contains("administrative_area_level_1", StringComparer.InvariantCultureIgnoreCase))
                ?.ShortName;
            var countryCode = result.AddressComponents
                ?.FirstOrDefault(w => w.Types.Contains("country", StringComparer.InvariantCultureIgnoreCase))
                ?.ShortName;
            var address = new Address(city, regionCode, countryCode);
            return (address, point);
        }
    }
}