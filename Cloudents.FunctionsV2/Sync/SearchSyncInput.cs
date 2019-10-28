using System;
using Cloudents.Query.Query.Sync;

namespace Cloudents.FunctionsV2.Sync
{
    public class SearchSyncInput
    {
        public SearchSyncInput(SyncType syncType)
        {
            SyncType = syncType;
        }

        public SyncType SyncType { get; set; }

        public string BlobName
        {
            get
            {
                switch (SyncType)
                {
                    case SyncType.University:
                        return "university3";
                    case SyncType.Document:
                        return "document1";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public string InstanceId
        {
            get
            {
                switch (SyncType)
                {
                    case SyncType.University:
                        return "UniversitySearchSync3";
                    case SyncType.Document:
                        return "DocumentSearchSync1";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public SyncAzureQuery SyncAzureQuery { get; set; }

        public override string ToString()
        {
            return $"{nameof(SyncType)}: {SyncType}, {nameof(BlobName)}: {BlobName}, {nameof(InstanceId)}: {InstanceId}, {nameof(SyncAzureQuery)}: {SyncAzureQuery}";
        }
    }

    

    public enum SyncType
    {
        University,
        Document,
    }
}