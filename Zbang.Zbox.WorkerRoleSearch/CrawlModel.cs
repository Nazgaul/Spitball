using System;
using Newtonsoft.Json;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.WorkerRoleSearch
{
    //[SerializePropertyNamesAsCamelCase]
    [DocumentDbModel("Crawl-Url")]
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

        public string Url { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }


        public string University { get; set; }

        public string Course { get; set; }


        public string[] Tags { get; set; }

        public DateTime? UrlDate { get; set; }

        public int? Views { get; set; }

        public string MetaDescription { get; set; }

        public string Image { get; set; }

        public string[] MetaKeywords { get; set; }

        public string Domain { get; set; }

        public ItemType Type { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get ; set; }


        public override string ToString()
        {
            return Id;
        }

    }

    
}
