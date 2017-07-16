using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsoleAppCrawler
{
    //[SerializePropertyNamesAsCamelCase]
    public class CrawlModel
    {
        protected CrawlModel()
        {

        }
        public CrawlModel(string url, string name, string content, string university,
            string course, string[] tags, DateTime? urlDate, int? views,
            string metaDescription, string image, string[] metaKeywords, string domain, string id)
        {
            Url = url;
            Name = name;
            Content = content;
            University = university;
            Course = course;
            Tags = tags;
            UrlDate = urlDate;
            Views = views;
            MetaDescription = metaDescription;
            Image = image;
            MetaKeywords = metaKeywords;
            Domain = domain;
            Id = id;
        }

       // [ProtoMember(1)]
        public string Url { get; set; }

       // [ProtoMember(2)]
        public string Name { get; set; }

       // [ProtoMember(3)]
        public string Content { get; set; }


       // [ProtoMember(4)]
        public string University { get; set; }

       // [ProtoMember(5)]
        public string Course { get; set; }


       // [ProtoMember(6)]
        public string[] Tags { get; set; }

       // [ProtoMember(7)]
        public DateTime? UrlDate { get; set; }

       // [ProtoMember(8)]
        public int? Views { get; set; }

       // [ProtoMember(9)]
        public string MetaDescription { get; set; }

       // [ProtoMember(10)]
        public string Image { get; set; }

       // [ProtoMember(11)]
        public string[] MetaKeywords { get; set; }

        //[ProtoMember(12)]
        public string Domain { get; set; }

       // [ProtoMember(13)]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

    }

    public class WebPage
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        //public string Name { get; set; }

        //public string Content { get; set; }

        //public string University { get; set; }

    }
}
