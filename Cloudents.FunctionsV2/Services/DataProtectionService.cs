using System;
using Microsoft.AspNetCore.DataProtection;

namespace Cloudents.FunctionsV2.Services
{
    public class DataProtectionService: IDataProtectionService
    {
        private readonly ITimeLimitedDataProtector _dataProtector;

        public DataProtectionService(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtector = dataProtectionProvider.CreateProtector("Spitball")
                .ToTimeLimitedDataProtector();
        }

        public string ProtectData(string data, DateTimeOffset dateTimeOffset)
        {
            return _dataProtector.Protect(data, dateTimeOffset);
        }
    }

    public interface IDataProtectionService
    {
        string ProtectData(string data, DateTimeOffset dateTimeOffset);
    }
}
