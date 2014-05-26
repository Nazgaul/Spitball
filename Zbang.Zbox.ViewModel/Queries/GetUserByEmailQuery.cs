namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetUserByEmailQuery
    {
        public GetUserByEmailQuery(string email)
        {
            Email = email;
        }
        public string Email { get; private set; }
    }
}
