using System;
using Cloudents.Core.Entities;

namespace Cloudents.Core.Interfaces
{
    public interface IUrlBuilder
    {
        string BuildWalletEndPoint(string token);
        string BuildShareEndPoint(string token);

        string BuildCourseEndPoint(string courseName);

        string BuildQuestionEndPoint(long id, object? parameters = null);
        string BuildProfileEndPoint(long id);


        Uri BuildChatEndpoint(string token,string identifier, object? parameters = null);

        Uri BuildShortUrlEndpoint(string identifier, object? parameters = null);

        Uri BuildShortUrlEndpoint(string identifier, Country? country);

        string BuildDocumentEndPoint(long id, object? parameters = null);
        string BuildDocumentThumbnailEndpoint(long id, object? parameters = null);
        string BuildUserImageEndpoint(long id, string? imageName, string userName, object? parameters = null);
        string BuildUserImageProfileShareEndpoint(long id, object? parameters = null);
        string BuildDocumentImageShareEndpoint(long id, object? parameters = null);
        string? BuildUserImageEndpoint(long id, string? imageName);
        Uri BuildTwilioWebHookEndPoint(Guid id);

        string BuildStudyRoomEndPoint(Guid id, object? parameters = null);
    }

}