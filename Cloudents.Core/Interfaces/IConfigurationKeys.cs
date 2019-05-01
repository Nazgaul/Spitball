namespace Cloudents.Core.Interfaces
{
    public interface IConfigurationKeys
    {
        DbConnectionString Db { get; }
        SearchServiceCredentials Search { get; }

        PayPalCredentials PayPal { get; }

        string Redis { get; }
        string Storage { get; }

        string SiteEndPoint { get; }

        string ServiceBus { get; }

    }
}