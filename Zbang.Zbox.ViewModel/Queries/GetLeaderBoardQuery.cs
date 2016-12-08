namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetLeaderBoardQuery
    {
        public GetLeaderBoardQuery(long boxId, long userId, bool myself)
        {
            BoxId = boxId;
            UserId = userId;
            Myself = myself;
        }

        public long BoxId { get; private set; }

        public long UserId { get; private set; }
        public bool Myself { get; private set; }

        //public string CacheKey
        //{
        //    get { return string.Format("{0}", BoxId); }
        //}

        //public string CacheRegion
        //{
        //    get { return "BoxLeaderBoard"; }
        //}

        //public TimeSpan Expiration
        //{
        //    get { return TimeSpan.FromMinutes(60); }
        //}
       
    }
}
