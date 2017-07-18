using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Zbang.Zbox.WorkerRoleSearch
{
    class CrawlerUrlEntity : TableEntity
    {
        public CrawlerUrlEntity(string domain, string url)
        {
            this.PartitionKey = Crawler.CalculateMd5Hash(domain);
            this.RowKey = Crawler.CalculateMd5Hash(url);
        }

        public CrawlerUrlEntity() { }


        public string Url { get; set; }

        public DateTime CrawlDate { get; set; }

    }
}