using System;
using Microsoft.AspNetCore.DataProtection;

namespace Cloudents.FunctionsV2.Test
{
    public class TestDataProtector : ITimeLimitedDataProtector
    {
        IDataProtector IDataProtectionProvider.CreateProtector(string purpose)
        {
            return CreateProtector(purpose);
        }

        public byte[] Protect(byte[] plaintext, DateTimeOffset expiration)
        {
            return plaintext;
            
        }

        public byte[] Unprotect(byte[] protectedData, out DateTimeOffset expiration)
        {
            expiration = DateTimeOffset.UtcNow;
            return protectedData;
        }

        public byte[] Protect(byte[] plaintext)
        {
            return plaintext;
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return protectedData;
        }

        public ITimeLimitedDataProtector CreateProtector(string purpose)
        {
            return this;
        }

      

    }

    public static class TestDataProtectorExtensions
    {
        public static ITimeLimitedDataProtector ToTimeLimitedDataProtector(this IDataProtector protector)
        {
            return new TestDataProtector();
        }
    }
}