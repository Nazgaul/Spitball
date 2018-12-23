using System.Collections.Generic;

namespace Cloudents.Core.Interfaces
{
    public interface IUrlBuilder
    {
        string BuildRedirectUrl(string url, string host, int? location);


        string BuildWalletEndPoint(object parameters = null);

        string BuildQuestionEndPoint(long id, object parameters = null);
        //string BuildDocumentEndPoint(long id, object parameters = null);

        //string BuildDocumentEndPoint
        //    (long id, object parameters = null);
    }

    public interface IUrlRedirectBuilder
    {
        IEnumerable<T> BuildUrl<T>(IEnumerable<T> result, int page = 0, int sizeOfPage = 0) where T : IUrlRedirect;
    }
}