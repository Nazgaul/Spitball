namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetBoxLeaderboardQuery
    {
        public GetBoxLeaderboardQuery(long boxId, long userId)
        {
            BoxId = boxId;
            UserId = userId;
        }

        public long BoxId { get; private set; }

        public long UserId { get; private set; }

        //public string CacheKey => $"{BoxId}";

        //public string CacheRegion => "BoxLeaderBoard";

        //public TimeSpan Expiration => TimeSpan.FromMinutes(60);
    }
}
