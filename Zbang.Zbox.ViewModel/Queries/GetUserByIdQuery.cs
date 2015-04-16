namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetUserByIdQuery
    {
        public GetUserByIdQuery(long id)
        {
            Id = id;
        }

        public long Id  { get; private set; }
    }
}