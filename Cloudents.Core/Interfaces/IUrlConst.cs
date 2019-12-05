using System;

namespace Cloudents.Core.Interfaces
{
    public interface IUrlBuilder
    {
        // string BuildRedirectUrl(string url, string host, int? location);

        string BuildWalletEndPoint(string token);
        string BuildShareEndPoint(string token);

        string BuildCourseEndPoint(string courseName);

        string BuildQuestionEndPoint(long id, object parameters = null);
        // string BuildPayMeBuyerEndPoint(string token);

        Uri BuildChatEndpoint(string token, object parameters = null);

        Uri BuildShortUrlEndpoint(string identifier, object parameters = null);
      //  Uri BuildShortUrlEndpoint(string identifier, string country);


        string BuildDocumentEndPoint(long id, object parameters = null);
        string BuildDocumentThumbnailEndpoint(long id);
    }

}