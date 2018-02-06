using System.Collections.Generic;

namespace Cloudents.Core.Interfaces
{
    public interface IUrlBuilder
    {
        string BuildRedirectUrl(string url, string host, int? location);
    }

    public interface IUrlRedirectBuilder<T> where T : IUrlRedirect
    {
        IEnumerable<T> BuildUrl(int page, int sizeOfPage, IEnumerable<T> result);
        IEnumerable<T> BuildUrl(IEnumerable<T> result);
    }

    public interface IConfigurationKeys
    {
        string Db { get; }
        SearchServiceCredentials Search { get; }
        string Redis { get; }
        string Storage { get; }

        string SystemUrl { get; }

        LocalStorageData LocalStorageData { get; }
    }
}