﻿using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Search.Places
{
    public class GooglePlaceDto
    {
        //[JsonProperty("html_attributions")]
        //public object[] HtmlAttributions { get; set; }
        [JsonProperty("result")]
        public Result2 Result { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }

        public class Result2
        {
           // [JsonProperty("address_components")]
            //public AddressComponents[] AddressComponents { get; set; }
            //[JsonProperty("adr_address")]
           // public string AdrAddress { get; set; }
            //[JsonProperty("formatted_address")]
           // public string FormattedAddress { get; set; }
            //[JsonProperty("formatted_phone_number")]
            //public string FormattedPhoneNumber { get; set; }
            [JsonProperty("geometry")]
            public Geometry Geometry { get; set; }
            //[JsonProperty("icon")]
            //public string Icon { get; set; }
            //[JsonProperty("id")]
            //public string Id { get; set; }
            //[JsonProperty("international_phone_number")]
            //public string InternationalPhoneNumber { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("opening_hours")]
            public OpeningHours OpeningHours { get; set; }
            [JsonProperty("photos")]
            public Photo[] Photos { get; set; }
            [JsonProperty("place_id")]
            public string PlaceId { get; set; }
            [JsonProperty("rating")]
            public float Rating { get; set; }
            //[JsonProperty("reference")]
            //public string Reference { get; set; }
            //[JsonProperty("reviews")]
            //public Review[] Reviews { get; set; }
            //[JsonProperty("scope")]
            //public string Scope { get; set; }
            //[JsonProperty("types")]
            //public string[] Types { get; set; }
            //[JsonProperty("url")]
            //public string Url { get; set; }
            //[JsonProperty("utc_offset")]
            //public int UtcOffset { get; set; }
            [JsonProperty("vicinity")]
            public string Vicinity { get; set; }
            //[JsonProperty("website")]
            //public string Website { get; set; }
        }

        public class Geometry
        {
            [JsonProperty("location")]
            public Location Location { get; set; }
            //[JsonProperty("viewport")]
            //public Viewport Viewport { get; set; }
        }

        public class Location
        {
            [JsonProperty("lat")]
            public float Lat { get; set; }
            [JsonProperty("lng")]
            public float Lng { get; set; }
        }

        //public class Viewport
        //{
        //    [JsonProperty("northeast")]
        //    public Northeast Northeast { get; set; }
        //    [JsonProperty("southwest")]
        //    public Southwest Southwest { get; set; }
        //}

        //public class Northeast
        //{
        //    [JsonProperty("lat")]
        //    public float Lat { get; set; }
        //    [JsonProperty("lng")]
        //    public float Lng { get; set; }
        //}

        //public class Southwest
        //{
        //    [JsonProperty("lat")]
        //    public float Lat { get; set; }
        //    [JsonProperty("lng")]
        //    public float Lng { get; set; }
        //}

        public class OpeningHours
        {
            [JsonProperty("open_now")]
            public bool OpenNow { get; set; }
            //[JsonProperty("periods")]
            //public Period[] Periods { get; set; }
           // [JsonProperty("weekday_text")]
            //public string[] WeekdayText { get; set; }
        }

        //public class Period
        //{
        //    [JsonProperty("close")]
        //    public Close Close { get; set; }
        //    [JsonProperty("open")]
        //    public Open Open { get; set; }
        //}

        //public class Close
        //{
        //    [JsonProperty("day")]
        //    public int Day { get; set; }
        //    [JsonProperty("time")]
        //    public string Time { get; set; }
        //}

        //public class Open
        //{
        //    [JsonProperty("day")]
        //    public int Day { get; set; }
        //    [JsonProperty("time")]
        //    public string Time { get; set; }
        //}

        //public class AddressComponents
        //{
        //    [JsonProperty("long_name")]
        //    public string LongName { get; set; }
        //    [JsonProperty("short_name")]
        //    public string ShortName { get; set; }
        //    [JsonProperty("types")]
        //    public string[] Types { get; set; }
        //}

        public class Photo
        {
            //[JsonProperty("height")]
           // public int Height { get; set; }
            //[JsonProperty("html_attributions")]
            //public string[] HtmlAttributions { get; set; }
            [JsonProperty("photo_reference")]
            public string PhotoReference { get; set; }
           // [JsonProperty("width")]
           // public int Width { get; set; }
        }

        //public class Review
        //{
        //    [JsonProperty("author_name")]
        //    public string AuthorName { get; set; }
        //    [JsonProperty("author_url")]
        //    public string AuthorUrl { get; set; }
        //    [JsonProperty("language")]
        //    public string Language { get; set; }
        //    [JsonProperty("profile_photo_url")]
        //    public string ProfilePhotoUrl { get; set; }
        //    [JsonProperty("rating")]
        //    public int Rating { get; set; }
        //    [JsonProperty("relative_time_description")]
        //    public string RelativeTimeDescription { get; set; }
        //    [JsonProperty("text")]
        //    public string Text { get; set; }
        //    [JsonProperty("time")]
        //    public int Time { get; set; }
        //}
    }

    public class GooglePlacesDto
    {
        //[JsonProperty("html_attributions")]
        //public object[] HtmlAttributions { get; set; }
        [JsonProperty("next_page_token")]
        public string NextPageToken { get; set; }
        [JsonProperty("results")]
        public Result[] Results { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }

        public class Result
        {
            [JsonProperty("geometry")]
            public Geometry Geometry { get; set; }
            //[JsonProperty("icon")]
            //public string Icon { get; set; }
            //[JsonProperty("id")]
            //public string Id { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("opening_hours")]
            public OpeningHours OpeningHours { get; set; }
            [JsonProperty("photos")]
            public Photo[] Photos { get; set; }
            [JsonProperty("place_id")]
            public string PlaceId { get; set; }
            //[JsonProperty("scope")]
            //public string Scope { get; set; }
            //[JsonProperty("price_level")]
            //public int PriceLevel { get; set; }
            [JsonProperty("rating")]
            public float Rating { get; set; }
            //[JsonProperty("reference")]
            //public string Reference { get; set; }
            //[JsonProperty("types")]
            //public string[] Types { get; set; }
            [JsonProperty("vicinity")]
            public string Vicinity { get; set; }
            //[JsonProperty("aspects")]
            //public Aspect[] Aspects { get; set; }
        }

        public class Geometry
        {
            [JsonProperty("location")]
            public Location Location { get; set; }
        }

        public class Location
        {
            [JsonProperty("lat")]
            public float Lat { get; set; }
            [JsonProperty("lng")]
            public float Lng { get; set; }
        }

        public class OpeningHours
        {
            [JsonProperty("open_now")]
            public bool OpenNow { get; set; }
        }

        public class Photo
        {
            //[JsonProperty("height")]
            //public int Height { get; set; }
            //[JsonProperty("html_attributions")]
            //public string[] HtmlAttributions { get; set; }
            [JsonProperty("photo_reference")]
            public string PhotoReference { get; set; }
            //[JsonProperty("width")]
            //public int Width { get; set; }
        }

        //public class Aspect
        //{
        //    [JsonProperty("rating")]
        //    public int Rating { get; set; }
        //    [JsonProperty("type")]
        //    public string Type { get; set; }
        //}
    }
}