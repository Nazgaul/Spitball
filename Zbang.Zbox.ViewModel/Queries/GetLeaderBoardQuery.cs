namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetLeaderBoardQuery
    {
        public GetLeaderBoardQuery(long boxId)
        {
            BoxId = boxId;
        }

        public long BoxId { get; private set; }

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
