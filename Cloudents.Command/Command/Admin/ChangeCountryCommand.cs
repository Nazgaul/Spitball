using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command.Admin
{
    public class ChangeCountryCommand : ICommand
    {
        public ChangeCountryCommand(long id, string country)
        {
            Id = id;
            Country = country;
        }

        public long Id { get; set; }
        public string Country { get; set; }
    }
}
