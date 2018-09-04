using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;


namespace Cloudents.Core.Command.Admin
{
    public class GiveTokensCommand : ICommand
    {
      
        public long UserId { get; set; }
        public decimal Price { get; set; }
        public TransactionType TypeId { get; set; }
    }
}
