namespace Cloudents.Core.Interfaces
{
    public interface IConfigurationKeys
    {
        string Db { get; }
        string MailGunDb { get; }
        SearchServiceCredentials Search { get; }
        string Redis { get; }
        string Storage { get; }
        string FunctionEndpoint { get; }
        string BlockChainNetwork { get; }

        LocalStorageData LocalStorageData { get; }
        string ServiceBus { get;  }

    }
}