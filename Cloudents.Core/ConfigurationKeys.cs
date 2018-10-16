using Cloudents.Core.Interfaces;

namespace Cloudents.Core
{
    public class ConfigurationKeys : IConfigurationKeys
    {
        private string _prodStorage;
        public ConfigurationKeys(string siteEndPoint)
        {
            SiteEndPoint = siteEndPoint;
            
        }

        public DbConnectionString Db { get; set; }
        public string MailGunDb { get; set; }
        public SearchServiceCredentials Search { get; set; }
        public string Redis { get; set; }
        public string Storage { get; set; }

        public string ProdStorage
        {
            get
            {
                if (_prodStorage != null)
                {
                    return _prodStorage;
                }

                return Storage;
            }
            set => _prodStorage = value;
        }
        //public string SystemUrl { get; set; }

        public LocalStorageData LocalStorageData { get; set; }
        //public string FunctionEndpoint { get; set; }

        public string BlockChainNetwork { get; set; }

        public string ServiceBus { get; set; }
        public string SiteEndPoint { get; }
    }

    public class SearchServiceCredentials
    {
        public SearchServiceCredentials(string name, string key, bool isDevelop)
        {
            Name = name;
            Key = key;
            IsDevelop = isDevelop;
        }

        public string Name { get; }
        public string Key { get; }

        public bool IsDevelop { get; }
    }

    public class DbConnectionString
    {
        public DbConnectionString(string db, string redis)
        {
            Db = db;
            Redis = redis;
        }

        public string Db { get;  }

        public string Redis { get;  }
    }

    public class LocalStorageData
    {
        /// <summary>
        /// Class represent location storage
        /// </summary>
        /// <param name="path">path of the temp storage</param>
        /// <param name="size">the size in megabytes</param>
        public LocalStorageData(string path, int size)
        {
            Path = path;
            Size = size;
        }

        public string Path { get; }
        public int Size { get; }
    }
}
