using Cloudents.Core.Interfaces;


namespace Cloudents.Core.Command.Admin
{
    public class CreateSendTokensCommand : ICommand
    {
      
        public int UserId { get; set; }
        public decimal Price { get; set; }
        public int TypeId { get; set; }
    }
}
