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

    public class TwilioCredentials
    {
        
        private const string AccountSid = "AC1796f09281da07ec03149db53b55db8d";
        private const string AuthToken = "c4cdf14c4f6ca25c345c3600a72e8b49";
    }
}