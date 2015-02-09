
namespace Zbang.Zbox.Infrastructure.Consts
{
    public class TimeConsts
    {
        public const int Second = 1;
        public const int Minute = Second * 60;
        public const int Hour = Minute * 60;
        public const int Day = Hour * 24;
        public const int Week = Day * 7;
        public const int Month = Day * 30;
        public const int Year = Day * 365;
    }

    public class StorageConsts
    {
        public const string ContentMetaDataKey = "content";
    }

    public class ClaimConsts
    {
        public const string UserIdClaim = "userId";
        public const string UniversityIdClaim = "universityId";
        public const string UniversityDataClaim = "universityData";
    }
}
