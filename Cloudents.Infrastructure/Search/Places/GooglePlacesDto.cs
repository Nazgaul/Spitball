namespace Cloudents.Infrastructure.Search.Places
{
    public class GooglePlacesDto
    {


        public object[] html_attributions { get; set; }
        public string next_page_token { get; set; }
        public Result[] results { get; set; }
        public string status { get; set; }


        public class Result
        {
            public Geometry geometry { get; set; }
            public string icon { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public Opening_Hours opening_hours { get; set; }
            public Photo[] photos { get; set; }
            public string place_id { get; set; }
            public string scope { get; set; }
            public int price_level { get; set; }
            public float rating { get; set; }
            public string reference { get; set; }
            public string[] types { get; set; }
            public string vicinity { get; set; }
            public Aspect[] aspects { get; set; }
        }

        public class Geometry
        {
            public Location location { get; set; }
        }

        public class Location
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Opening_Hours
        {
            public bool open_now { get; set; }
        }

        public class Photo
        {
            public int height { get; set; }
            public string[] html_attributions { get; set; }
            public string photo_reference { get; set; }
            public int width { get; set; }
        }

        public class Aspect
        {
            public int rating { get; set; }
            public string type { get; set; }
        }

    }
}