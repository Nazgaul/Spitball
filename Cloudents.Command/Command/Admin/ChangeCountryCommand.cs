namespace Cloudents.Command.Command.Admin
{
    public class ChangeCountryCommand : ICommand
    {
        public ChangeCountryCommand(long id, string country)
        {
            Id = id;
            Country = country;
        }

        public long Id { get; }
        public string Country { get; }
    }
}
