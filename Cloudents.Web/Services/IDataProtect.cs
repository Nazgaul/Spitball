using System;

namespace Cloudents.Web.Services
{
    public interface IDataProtect
    {
        string Protect(string plaintext, DateTimeOffset expiration);

        string Unprotect(string protectedData);
        // Task InsertMessageAsync(ServiceBusMessageBase message, CancellationToken token);
    }
}