using System;
using Cloudents.Core.Query.Sync;

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
                    case SyncType.Question:
                        return "question4";

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
                    case SyncType.Question:
                        return "QuestionSearchSync5";

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
        //Course,
        Question
    }
}