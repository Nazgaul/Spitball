using System;

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
                    //case SyncType.University:
                    //    break;
                    //case SyncType.Course:
                    //    break;
                    case SyncType.Question:
                        return "question";

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

    }

    public enum SyncType
    {
        University,
        Course,
        Question
    }
}