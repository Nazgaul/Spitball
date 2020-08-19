using System;

namespace Cloudents.FunctionsV2.Services
{
    public interface IDataProtectionService
    {
        string ProtectData(string data, DateTimeOffset dateTimeOffset);
    }
}