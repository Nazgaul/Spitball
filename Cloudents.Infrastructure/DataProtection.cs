using System;
using Cloudents.Core.Storage;
using Microsoft.AspNetCore.DataProtection;

namespace Cloudents.Infrastructure
{


    public class DataProtection : IDataProtect
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public DataProtection(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        public string Protect(string purpose, string plaintext, DateTimeOffset expiration)
        {
            var dataProtector = _dataProtectionProvider.CreateProtector(purpose).ToTimeLimitedDataProtector();
            return dataProtector.Protect(plaintext, expiration);
        }

        public string Unprotect(string purpose, string protectedData)
        {
            var dataProtector = _dataProtectionProvider.CreateProtector(purpose).ToTimeLimitedDataProtector();
            return dataProtector.Unprotect(protectedData);
        }
    }
}
