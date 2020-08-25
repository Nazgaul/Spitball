namespace Cloudents.Core.Interfaces
{
    public interface IConfigurationKeys
    {
        DbConnectionString Db { get; }
        SearchServiceCredentials Search { get; }


        string Redis { get; }
        StorageCredentials Storage { get; }

        SiteEndPoints SiteEndPoint { get; }

        string ServiceBus { get; }

        string Stripe { get; }

    }
}