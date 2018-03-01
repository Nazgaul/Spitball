using System.Collections.Generic;

namespace Cloudents.Core.Interfaces
{
    public interface IUrlBuilder
    {
        string BuildRedirectUrl(string url, string host, int? location);
    }

    public interface IUrlRedirectBuilder
    {
        IEnumerable<T> BuildUrl<T>(IEnumerable<T> result, int page = 0, int sizeOfPage = 0) where T : IUrlRedirect;
    }

    public interface IConfigurationKeys
    {
        string Db { get; }
        string MailGunDb { get; }
        SearchServiceCredentials Search { get; }
        string Redis { get; }
        string Storage { get; }


        LocalStorageData LocalStorageData { get; }
    }
}