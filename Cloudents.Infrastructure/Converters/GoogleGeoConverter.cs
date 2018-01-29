using System;
using System.Linq;
using AutoMapper;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Search.Places;

namespace Cloudents.Infrastructure.Converters
{
    public class GoogleGeoConverter : ITypeConverter<GoogleGeoCodeDto, Location>
    {
        public Location Convert(GoogleGeoCodeDto source, Location destination, ResolutionContext context)
        {
            if (!string.Equals(source.Status, "ok", StringComparison.InvariantCultureIgnoreCase))
                return null;
            var result = source.Results[0];

            return new Location
            {
                Point = new GeoPoint
                {
                    Longitude = result.Geometry.Location.Lng,
                    Latitude = result.Geometry.Location.Lat
                },

                City = result.AddressComponents
                    ?.FirstOrDefault(w => w.Types.Contains("locality", StringComparer.InvariantCultureIgnoreCase))
                    ?.ShortName,
                RegionCode = result.AddressComponents?.FirstOrDefault(w =>
                        w.Types.Contains("administrative_area_level_1", StringComparer.InvariantCultureIgnoreCase))
                    ?.ShortName,
                CountryCode = result.AddressComponents
                    ?.FirstOrDefault(w => w.Types.Contains("country", StringComparer.InvariantCultureIgnoreCase))
                    ?.ShortName
            };
        }
    }
}