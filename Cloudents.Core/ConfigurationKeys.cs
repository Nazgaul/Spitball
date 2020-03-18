using Cloudents.Core.Interfaces;

namespace Cloudents.Core
{
    public class ConfigurationKeys : IConfigurationKeys
    {
        public ConfigurationKeys()
        {
            SiteEndPoint = new SiteEndPoints();

        }
        //public ConfigurationKeys(string siteEndPoint)
        //{
        //    SiteEndPoint = siteEndPoint;

        //}

        public DbConnectionString Db { get; set; }
        public string MailGunDb { get; set; }
        public SearchServiceCredentials Search { get; set; }
        public string Redis { get; set; }
        public string Storage { get; set; }

        public PayPalCredentials PayPal { get; set; }

        public LocalStorageData LocalStorageData { get; set; }
        //public string FunctionEndpoint { get; set; }


        public SiteEndPoints SiteEndPoint { get; set; }
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

    public class SiteEndPoints
    {
        public string SpitballSite { get; set; }
        public string FunctionSite { get; set; }
        public string IndiaSite { get; set; }
    }


    public class PayMeCredentials
    {
        public PayMeCredentials()
        {
            EndPoint = "https://preprod.paymeservice.com/api/";
            SellerId = "MPL15546-31186SKB-53ES24ZG-WGVCBKO2";
            BuyerKey = "BUYER156-4564629H-GXBKSW7B-T3H2FF2F";
        }
        //Need not to be in private set
        public string EndPoint { get; set; }
        //Need not to be in private set
        public string SellerId { get; set; }
        //Need not to be in private set
        public string BuyerKey { get; set; }
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
        public DbConnectionString(string db, string? redis, DataBaseIntegration dataBaseIntegration)
        {
            Db = db;
            Redis = redis;
            Integration = dataBaseIntegration;
        }

        public string Db { get; }

        public string? Redis { get; }

        public DataBaseIntegration Integration { get; set; }


        public enum DataBaseIntegration
        {
            None,
            Validate,
            Update
        }
    }

    public class PayPalCredentials
    {
        public string ClientId { get; }
        public string ClientSecret { get; }
        public bool IsDevelop { get; }


        public PayPalCredentials(string clientId, string clientSecret, bool isDevelop)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            IsDevelop = isDevelop;
        }
        public string SpitballSite { get; set; }
        public string FunctionSite { get; set; }
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
