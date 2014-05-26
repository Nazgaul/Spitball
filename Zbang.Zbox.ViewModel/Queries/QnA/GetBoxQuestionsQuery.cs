namespace Zbang.Zbox.ViewModel.Queries.QnA
{
    public class GetBoxQuestionsQuery : QueryBase
    {
        public GetBoxQuestionsQuery(long boxId, long userId)
            : base(userId)
        {
            BoxId = boxId;
        }


        public long BoxId { get; private set; }

    }
}
