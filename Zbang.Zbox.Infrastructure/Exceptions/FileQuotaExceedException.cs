using System;

namespace Zbang.Zbox.Infrastructure.Exceptions
{
    [Serializable]
    public class FileQuotaExceedException : Exception
    {
        public override string Message => "File Size Exceeds Quota";
    }
}
