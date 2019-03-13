using Cloudents.Core.Interfaces;

namespace Cloudents.Core
{
    public class ConfigurationKeys : IConfigurationKeys
    {
        public ConfigurationKeys(string siteEndPoint)
        {
            SiteEndPoint = siteEndPoint;

        }

        //public static ConfigurationKeys WebSite(string sqlConnection, string redis, string searchServiceName,
        //    string searchServiceKey, bool isProduction, string storage,
        //    string serviceBus, string payPal)
        //{
        //    return new ConfigurationKeys()
        //    {
        //        Db = new DbConnectionString(sqlConnection,redis),
        //        Redis = redis,
        //        Search = new SearchServiceCredentials(searchServiceName, searchServiceKey, isProduction),
        //        Storage = storage,
        //        ServiceBus = serviceBus,
        //        PayPal = new PayPalCredentials()
        //    };

        //}



        public DbConnectionString Db { get; set; }
        public string MailGunDb { get; set; }
        public SearchServiceCredentials Search { get; set; }
        public PayPalCredentials PayPal { get; set; }
        public string Redis { get; set; }
        public string Storage { get; set; }



        public LocalStorageData LocalStorageData { get; set; }
        //public string FunctionEndpoint { get; set; }

        public string BlockChainNetwork { get; set; }

        public string SiteEndPoint { get; }
        public string ServiceBus { get; set; }
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

    public class PayPalCredentials
    {
        public string ClientId { get; }
        public string ClientSecret { get; }
        public bool IsDevelop { get; }


        public PayPalCredentials( string clientId, string clientSecret, bool isDevelop)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            IsDevelop = isDevelop;
        }
    }

    public class TwilioCredentials
    {
        /// <summary>
        /// The primary Twilio account SID, displayed prominently on your twilio.com/console dashboard.
        /// </summary>
        public string AccountSid { get; set; }

        /// <summary>
        /// The auth token for your primary Twilio account, hidden on your twilio.com/console dashboard.
        /// </summary>
        public string AuthToken { get; set; }

        /// <summary>
        /// Signing Key SID, also known as the API SID or API Key.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The API Secret that corresponds to the <see cref="ApiKey"/>.
        /// </summary>
        public string ApiSecret { get; set; }
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
