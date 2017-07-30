using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using Abot.Core;
using Abot.Poco;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class CrawlScheduler2 : IScheduler
    {

        private readonly CloudTable m_Table;
        private readonly CloudQueue m_Queue;
        private int? m_Count;
        readonly bool m_AllowUriRecrawling;

        readonly ConcurrentDictionary<string, byte> m_TempTable = new ConcurrentDictionary<string, byte>();
        private readonly ConcurrentDictionary<string, string> m_HostMd5 = new ConcurrentDictionary<string, string>();

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
            m_AllowUriRecrawling = allowUriRecrawling;
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


            if (m_AllowUriRecrawling || page.IsRetry)
            {
                InsertToQueue();
            }
            else
            {
                if (page.Uri.PathAndQuery.ToLowerInvariant().Contains("xml"))
                {
                    m_TempTable.TryAdd(page.Uri.AbsoluteUri, 0);
                    InsertToQueue();
                    return;
                }
                try
                {

                    var entity = new CrawlerUrlEntity(GetHostMd5(page.Uri), page.Uri.AbsoluteUri)
                    {
                        Url = page.Uri.AbsoluteUri,
                    };
                    m_Table.Execute(TableOperation.Insert(entity));
                    m_TempTable.TryAdd(page.Uri.AbsoluteUri, 0);
                    InsertToQueue();
                }
                catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict)
                {

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
            if (m_TempTable.ContainsKey(uri.AbsoluteUri))
            {
                return;
            }
            try
            {
                m_TempTable.TryAdd(uri.AbsoluteUri, 0);
                var entity = new CrawlerUrlEntity(GetHostMd5(uri), uri.AbsoluteUri);
                m_Table.Execute(TableOperation.Insert(entity));
                m_TempTable.TryAdd(uri.AbsoluteUri, 0);
            }
            catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict)
            {
            }
        }

        public bool IsUriKnown(Uri uri)
        {
            var t = m_TempTable.ContainsKey(uri.AbsoluteUri);
            if (t)
            {
                return true;
            }
            if (uri.AbsoluteUri.ToLowerInvariant().Contains("xml"))
            {
                //We want at least one time xml pass per machine. if it doesn't work we take this down
                //TraceLog.WriteInfo($"{uri} is not known uri");
                //m_TempTable.TryAdd(uri.AbsoluteUri, 0);
                return false;
            }
            var operation = TableOperation.Retrieve<CrawlerUrlEntity>(GetHostMd5(uri), Md5HashGenerator.GenerateKey(uri.AbsoluteUri));
            var result = m_Table.Execute(operation);
            var isUriKnown = result.Result != null;
            if (isUriKnown)
            {
                m_TempTable.TryAdd(uri.AbsoluteUri, 0);
            }
            return isUriKnown;
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


        private string GetHostMd5(Uri uri)
        {
            if (m_HostMd5.TryGetValue(uri.Host, out string md5Value))
            {
                return md5Value;
            }
            md5Value = Md5HashGenerator.GenerateKey(uri.Host);
            m_HostMd5.TryAdd(uri.Host, md5Value);
            return md5Value;
        }
    }


}
