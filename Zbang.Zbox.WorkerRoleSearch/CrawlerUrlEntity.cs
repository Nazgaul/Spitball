using System;
using Microsoft.WindowsAzure.Storage.Table;
using Zbang.Zbox.Infrastructure;

namespace Zbang.Zbox.WorkerRoleSearch
{
    class CrawlerUrlEntity : TableEntity
    {
        public CrawlerUrlEntity(string domainMd5, string url)
        {
            this.PartitionKey = domainMd5;
            this.RowKey = Md5HashGenerator.GenerateKey(url);
        }

        public CrawlerUrlEntity() { }


        public string Url { get; set; }


    }
}