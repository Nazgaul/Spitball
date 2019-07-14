using System;

namespace Cloudents.Core.Interfaces
{
    public interface IUrlBuilder
    {
       // string BuildRedirectUrl(string url, string host, int? location);

        string BuildWalletEndPoint(string token);
        string BuildShareEndPoint(string token);

        string BuildQuestionEndPoint(long id, object parameters = null);
       // string BuildPayMeBuyerEndPoint(string token);

        Uri BuildChatEndpoint(string token, object parameters = null);

        Uri BuildShortUrlEndpoint(string identifier);
    }

    //public interface IUrlRedirectBuilder
    //{
    //    IEnumerable<T> BuildUrl<T>(IEnumerable<T> result, int page = 0, int sizeOfPage = 0) where T : IUrlRedirect;
    //}
}