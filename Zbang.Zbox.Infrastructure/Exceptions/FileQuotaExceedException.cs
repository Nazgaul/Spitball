using System;

namespace Zbang.Zbox.Infrastructure.Exceptions
{
    public class FileQuotaExceedException : Exception
    {
        public override string Message
        {
            get
            {
                return "File Size Exceeds Quota";
            }
        }
    }
}
