using System;
using Cloudents.Core.Query;

namespace Cloudents.Functions.Sync
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
                        return "university";
                    case SyncType.Course:
                        return "course";
                    case SyncType.Question:
                        return "question";

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public SyncAzureQuery SyncAzureQuery { get; set; }

    }

    

    public enum SyncType
    {
        University,
        Course,
        Question
    }
}