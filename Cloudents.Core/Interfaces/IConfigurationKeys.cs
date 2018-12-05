namespace Cloudents.Core.Interfaces
{
    public interface IConfigurationKeys
    {
        DbConnectionString Db { get; }
        SearchServiceCredentials Search { get; }
        string Redis { get; }
        string Storage { get; }
        string BlockChainNetwork { get; }

        LocalStorageData LocalStorageData { get; }

        string SiteEndPoint { get; }

        string ServiceBus { get; }
    }
}