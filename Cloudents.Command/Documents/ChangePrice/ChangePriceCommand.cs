using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Command.Documents.ChangePrice
{
    public class ChangePriceCommand : ICommand
    {
        public ChangePriceCommand(long documentId, long userId, decimal price)
        {
            DocumentId = documentId;
            UserId = userId;
            Price = price;
        }

        public long DocumentId { get; private set; }
        public long UserId { get; set; }
        public decimal Price { get; private set; }
    
    }
}
