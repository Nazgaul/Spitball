using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Zbang.Zbox.WorkerRoleSearch
{
    class CrawlerUrlEntity : TableEntity
    {
        public CrawlerUrlEntity(string domainMd5, string url)
        {
            this.PartitionKey = domainMd5;
            this.RowKey = Crawler.CalculateMd5Hash(url);
        }

        public CrawlerUrlEntity() { }


        public string Url { get; set; }


    }
}