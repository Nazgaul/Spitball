using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.DTOs.SearchSync
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Name of db column - easy to use")]
    public class AzureSyncBaseDto<T>
    {
        public string Id { get; set; }

        public string SYS_CHANGE_OPERATION { get; set; }

        public long? SYS_CHANGE_VERSION { get; set; }

        public T Data { get; set; }
    }

}