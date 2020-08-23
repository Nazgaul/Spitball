using Microsoft.AspNetCore.DataProtection;
using System;

namespace Cloudents.Web.Services
{
    public class DataProtection : IDataProtect
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public DataProtection(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        public string Protect(string plaintext, DateTimeOffset expiration)
        {
            var dataProtector = _dataProtectionProvider.CreateProtector("Spitball").ToTimeLimitedDataProtector();
            return dataProtector.Protect(plaintext, expiration);
        }

        public string Unprotect(string protectedData)
        {
            var dataProtector = _dataProtectionProvider.CreateProtector("Spitball")
                .ToTimeLimitedDataProtector();
            return dataProtector.Unprotect(protectedData);
        }
    }
}
