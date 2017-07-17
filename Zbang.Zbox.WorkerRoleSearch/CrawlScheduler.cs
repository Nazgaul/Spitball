using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Abot.Core;
using Abot.Poco;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Zbang.Zbox.Infrastructure.Trace;

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



    }

    public class CrawlScheduler2 : IScheduler
    {
        private readonly CloudTable m_Table;

        private readonly CloudQueue m_Queue;
        private int? m_Count = null;
        bool _allowUriRecrawling;

        readonly JsonSerializerSettings m_Settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None

        };


        public CrawlScheduler2(bool allowUriRecrawling)
        {
            var storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var tableClient = storageAccount.CreateCloudTableClient();
            m_Table = tableClient.GetTableReference("crawlerUrls");
            m_Table.CreateIfNotExists();

            var queueClient = storageAccount.CreateCloudQueueClient();
            m_Queue = queueClient.GetQueueReference("crawler-urls");
            _allowUriRecrawling = allowUriRecrawling;
        }
        public void Dispose()
        {
            //m_Table.DeleteIfExists();
            //m_Queue.Clear();
        }

        public void Add(PageToCrawl page)
        {
            void InsertToQueue()
            {
                var json = JsonConvert.SerializeObject(page, m_Settings);
                var message = new CloudQueueMessage(json);
                m_Queue.AddMessage(message);
                m_Count = null;
            }

            if (page == null)
                throw new ArgumentNullException(nameof(page));

            if (page.Uri.PathAndQuery.ToLowerInvariant().Contains("xml"))
            {
                InsertToQueue();
                return;
            }
            if (_allowUriRecrawling || page.IsRetry)
            {
                InsertToQueue();
            }
            else
            {
                try
                {
                    var entity = new CrawlerUrlEntity(page.Uri.Host, page.Uri.AbsoluteUri);
                    m_Table.Execute(TableOperation.Insert(entity));
                    InsertToQueue();
                }
                catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == (int) HttpStatusCode.Conflict)
                {
                    TraceLog.WriteError($"site crawl Scheduler add {page}", ex);
                }
              

            }
        }

        public void Add(IEnumerable<PageToCrawl> pages)
        {
            if (pages == null)
                throw new ArgumentNullException(nameof(pages));

            foreach (PageToCrawl page in pages)
                Add(page);
        }

        public PageToCrawl GetNext()
        {
            var message = m_Queue.GetMessage();

            if (message == null)
            {
                m_Count = 0;
                return null;
            }
            m_Queue.DeleteMessage(message);

            var m = JsonConvert.DeserializeObject<PageToCrawl>(message.AsString, m_Settings);
            return m;
        }

        public void Clear()
        {
            m_Count = 0;
            m_Queue.Clear();
            //m_Table.DeleteIfExists();
        }

        public void AddKnownUri(Uri uri)
        {
            try
            {
                var entity = new CrawlerUrlEntity(uri.Host, uri.AbsoluteUri);
                m_Table.Execute(TableOperation.Insert(entity));
            }
            catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict)
            {
                TraceLog.WriteError($"site crawl Scheduler AddKnownUri {uri}", ex);
            }
        }

        public bool IsUriKnown(Uri uri)
        {
            var operation = TableOperation.Retrieve<CrawlerUrlEntity>(Crawler.CalculateMd5Hash(uri.Host), Crawler.CalculateMd5Hash(uri.AbsoluteUri));
            var result = m_Table.Execute(operation);
            return result.Result != null;
        }

        public int Count
        {
            get
            {
                if (m_Count.HasValue)
                {
                    return m_Count.Value;
                }
                m_Queue.FetchAttributes();
                m_Count = m_Queue.ApproximateMessageCount;
                return m_Count ?? 0;
            }
        }
    }

    //public class CrawlScheduler : Scheduler
    //{
    //    public CrawlScheduler(bool allowUriRecrawling) : base(
    //        allowUriRecrawling, new CompactCrawledUrlRepository(), new PageToCrawlRepository())
    //    {

    //    }
    //}

    
    //public class CrawledUrlRepository : ICrawledUrlRepository
    //{
    //    private readonly CloudTable m_Table;
    //    public CrawledUrlRepository()
    //    {
    //        var storageAccount = CloudStorageAccount.Parse(
    //            CloudConfigurationManager.GetSetting("StorageConnectionString"));
    //        var tableClient = storageAccount.CreateCloudTableClient();
    //        m_Table = tableClient.GetTableReference("crawler-urls");
    //        m_Table.CreateIfNotExists();
    //    }
    //    public void Dispose()
    //    {
            
    //    }

    //    public bool Contains(Uri uri)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool AddIfNew(Uri uri)
    //    {
    //        var retrieveOperation = TableOperation.Retrieve<CrawlerUrlEntity>(uri.Host, uri.AbsoluteUri);
    //        TableResult retrievedResult = m_Table.Execute(retrieveOperation);

    //        // Print the phone number of the result.
    //        if (retrievedResult.Result != null)
    //        {
    //            Console.WriteLine(((CustomerEntity)retrievedResult.Result).PhoneNumber);
    //        }
    //        else
    //        {
    //            Console.WriteLine("The phone number could not be retrieved.");
    //        }

    //        var entity = new CrawlerUrlEntity(uri.Host, uri.AbsoluteUri);
    //        throw new NotImplementedException();
    //    }
    //}

    //public class PageToCrawlRepository : IPagesToCrawlRepository
    //{
    //    private readonly CloudQueue m_Queue;
    //    private int? m_Count = null;

    //    readonly JsonSerializerSettings m_Settings = new JsonSerializerSettings
    //    {
    //        TypeNameHandling = TypeNameHandling.All,
    //        NullValueHandling = NullValueHandling.Ignore,
    //        Formatting = Formatting.None

    //    };

    //    public PageToCrawlRepository()
    //    {
    //        var storageAccount = CloudStorageAccount.Parse(
    //            CloudConfigurationManager.GetSetting("StorageConnectionString"));
    //        var queueClient = storageAccount.CreateCloudQueueClient();
    //        m_Queue = queueClient.GetQueueReference("crawler-urls");
    //    }
    //    public void Dispose()
    //    {

    //    }

    //    public void Add(PageToCrawl page)
    //    {
            
    //        var json = JsonConvert.SerializeObject(page, m_Settings);
    //        var message = new CloudQueueMessage(json);
    //        m_Queue.AddMessage(message);
    //        m_Count = null;
    //    }

    //    public PageToCrawl GetNext()
    //    {
    //        var message = m_Queue.GetMessage();

    //        if (message == null)
    //        {
    //            m_Count = 0;
    //            return null;
    //        }
    //        m_Queue.DeleteMessage(message);
            
    //        var m = JsonConvert.DeserializeObject<PageToCrawl>(message.AsString, m_Settings);
    //        return m;
    //    }

    //    public void Clear()
    //    {
    //        m_Count = 0;
    //        m_Queue.Clear();
    //    }

    //    public int Count()
    //    {
    //        if (m_Count.HasValue)
    //        {
    //            return m_Count.Value;
    //        }
    //        m_Queue.FetchAttributes();
    //        m_Count = m_Queue.ApproximateMessageCount;
    //        return m_Count ?? 0;
    //    }
    //}
}
