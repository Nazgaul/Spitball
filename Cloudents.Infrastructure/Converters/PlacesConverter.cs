using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Models;
using Newtonsoft.Json.Linq;

namespace Cloudents.Infrastructure.Converters
{
    public class PlacesConverter : ITypeConverter<JObject, (string, IEnumerable<PlaceDto>)>
    {
        public (string, IEnumerable<PlaceDto>) Convert(JObject source, (string, IEnumerable<PlaceDto>) destination, ResolutionContext context)
        {
            //var placeConverter = new PlaceConverter();
            var retVal = source["results"].Select(json =>
            {
                //TODO : this is duplicate as place - need to fix that
                var photo = json["photos"]?[0]?["photo_reference"]?.Value<string>();
                string image = null;
                if (!string.IsNullOrEmpty(photo))
                {
                    image =
                        $"https://maps.googleapis.com/maps/api/place/photo?maxwidth={context.Items["width"]}&photoreference={photo}&key={context.Items["key"]}";
                }
                Core.Models.Location location = null;
                if (json["geometry"]?["location"] != null)
                {
                    location = new Core.Models.Location
                    {
                        Latitude = json["geometry"]["location"]["lat"].Value<double>(),
                        Longitude = json["geometry"]["location"]["lng"].Value<double>()
                    };
                }
                return new PlaceDto
                {
                    Address = json["vicinity"]?.Value<string>(),
                    Image = image,
                    Location = location,
                    Name = json["name"].Value<string>(),
                    Open = json["opening_hours"]?["open_now"].Value<bool?>() ?? false,
                    Rating = json["rating"]?.Value<double>() ?? 0,
                    PlaceId = json["place_id"]?.Value<string>()
                };
            });
            var nextPage = source["next_page_token"]?.Value<string>();
            return (nextPage, retVal);
        }
    }


    public class PlaceConverter : ITypeConverter<JObject, PlaceDto>
    {
        public PlaceDto Convert(JObject source, PlaceDto destination, ResolutionContext context)
        {
            var json = source["result"];
            var photo = json["photos"]?[0]?["photo_reference"]?.Value<string>();
            string image = null;
            if (!string.IsNullOrEmpty(photo))
            {
                image =
                    $"https://maps.googleapis.com/maps/api/place/photo?maxwidth={context.Items["width"]}&photoreference={photo}&key={context.Items["key"]}";
            }
            Location location = null;
            if (json["geometry"]?["location"] != null)
            {
                location = new Location
                {
                    Latitude = json["geometry"]["location"]["lat"].Value<double>(),
                    Longitude = json["geometry"]["location"]["lng"].Value<double>()
                };
            }
            return new PlaceDto
            {
                Address = json["vicinity"]?.Value<string>(),
                Image = image,
                Location = location,
                Name = json["name"].Value<string>(),
                Open = json["opening_hours"]?["open_now"].Value<bool?>() ?? false,
                Rating = json["rating"]?.Value<double>() ?? 0,
                PlaceId = json["place_id"]?.Value<string>()
            };
        }
    }
}
